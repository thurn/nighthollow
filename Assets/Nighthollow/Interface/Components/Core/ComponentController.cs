// Copyright © 2020-present Derek Thurn

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//    https://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Nighthollow.Rules;
using Nighthollow.Services;
using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface.Components.Core
{
  public interface IComponentController<TRoot> where TRoot : BaseComponent
  {
    void UpdateRoot(Func<TRoot, TRoot> function);

    void OnUpdate();
  }

  public abstract class ComponentController
  {
    readonly IStartCoroutine _runner;
    readonly VisualElement _parentElement;
    readonly Dictionary<(Type, string), object> _states = new();
    readonly List<(Type, string)> _toFetch = new();
    readonly Dictionary<string, object> _resources = new();
    readonly ServiceRegistry _registry;
    VisualElement? _lastRenderedElement;
    IMountComponent? _lastRenderedComponent;

    protected ComponentController(
      IStartCoroutine runner,
      ServiceRegistry registry,
      VisualElement parentElement)
    {
      _runner = runner;
      _registry = registry;
      _parentElement = parentElement;
    }

    protected abstract BaseComponent CurrentComponent { get; }

    protected bool UpdateRequired { get; set; } = true;

    public Scope Scope => _registry.Scope;

    public void OnUpdate()
    {
      if (UpdateRequired)
      {
        var tree = CurrentComponent.Reduce(GlobalKey.Root(this));
        var result = Reconciler.Update(GlobalKey.Root(this), _lastRenderedElement, _lastRenderedComponent, tree);
        _lastRenderedComponent = tree;

        if (result != null)
        {
          _parentElement.Clear();

          _parentElement.Add(result);
          _lastRenderedElement = result;
        }

        UpdateRequired = false;

        if (_toFetch.Count > 0)
        {
          _runner.StartCoroutine(FetchResources());
        }
      }
    }

    public IState<T> UseState<T>((Type, string) key, T initialValue)
    {
      if (!_states.ContainsKey(key))
      {
        SetState(key, initialValue);
      }

      return new ComponentState<T>(this, key);
    }

    public T? UseResource<T>(string address) where T : UnityEngine.Object
    {
      if (_resources.ContainsKey(address))
      {
        return (T) _resources[address];
      }
      else
      {
        _toFetch.Add((typeof(T), address));
        return null;
      }
    }

    IEnumerator<WaitUntil> FetchResources()
    {
      var requests = new Dictionary<string, ResourceRequest>();
      foreach (var (resourceType, address) in _toFetch)
      {
        requests[address] = Resources.LoadAsync(address, resourceType);
      }

      yield return new WaitUntil(() => requests.Values.All(r => r.isDone));

      foreach (var pair in requests)
      {
        _resources[pair.Key] = pair.Value.asset;
      }

      _toFetch.Clear();
      UpdateRequired = true;
    }

    T GetState<T>((Type, string) key)
    {
      Errors.CheckState(_states.ContainsKey(key), $"State not found for {key}");
      return (T) _states[key];
    }

    void SetState<T>((Type, string) key, T value)
    {
      if (value is null)
      {
        throw new NullReferenceException("Null state values are not allowed.");
      }

      UpdateRequired = true;
      _states[key] = value;
    }

    sealed class ComponentState<T> : IState<T>
    {
      readonly ComponentController _controller;
      readonly (Type, string) _key;

      public ComponentState(ComponentController controller, (Type, string) key)
      {
        _controller = controller;
        _key = key;
      }

      public T Value => _controller.GetState<T>(_key);

      public void Set(T value)
      {
        _controller.SetState(_key, value);
      }
    }
  }

  public sealed class ComponentController<TRoot> : ComponentController, IComponentController<TRoot>
    where TRoot : BaseComponent
  {
    public static IComponentController<TRoot> Create(
      IStartCoroutine runner,
      ServiceRegistry registry,
      VisualElement parentElement,
      TRoot component) => new ComponentController<TRoot>(runner, registry, parentElement, component);

    TRoot _component;

    ComponentController(
      IStartCoroutine runner,
      ServiceRegistry registry,
      VisualElement parentElement,
      TRoot component) : base(runner, registry, parentElement)
    {
      _component = component;
    }

    protected override BaseComponent CurrentComponent => _component;

    public void UpdateRoot(Func<TRoot, TRoot> function)
    {
      _component = function(_component);
      UpdateRequired = true;
    }
  }
}