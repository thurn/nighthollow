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
using System.Collections.Immutable;
using System.Linq;
using Nighthollow.Utils;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface.Components.Core
{
  public abstract record BaseComponent
  {
    protected GlobalKey? GlobalKey;

    public string? Key { get; init; }

    public abstract IMountComponent Reduce(GlobalKey globalKey);

    protected static IEnumerable<BaseComponent> List(params BaseComponent?[] children) => children.WhereNotNull();

    protected IState<T> UseState<T>(T initialValue)
    {
      if (GlobalKey == null)
      {
        throw new NullReferenceException($"Error: Invoked {nameof(UseState)}() outside of OnRender()/OnMount()");
      }

      return GlobalKey.UseState(GetType(), initialValue);
    }

    protected T? UseResource<T>(string? address) where T : UnityEngine.Object
    {
      if (address == null)
      {
        return null;
      }

      if (GlobalKey == null)
      {
        throw new NullReferenceException($"Error: Invoked {nameof(UseResource)}() outside of OnRender()/OnMount()");
      }

      return GlobalKey.UseResource<T>(address);
    }
  }

  public abstract record LayoutComponent : BaseComponent
  {
    protected abstract BaseComponent OnRender();

    public override IMountComponent Reduce(GlobalKey globalKey)
    {
      GlobalKey = globalKey;
      var child = OnRender();
      GlobalKey = null;
      return child.Reduce(globalKey.Child(child.Key ?? "0"));
    }
  }

  public interface IMountComponent
  {
    public string? Key { get; }

    string Type { get; }

    VisualElement CreateMountContent();

    void Mount(GlobalKey hooks, VisualElement element);

    void ReduceChildren(GlobalKey hooks);

    ImmutableList<IMountComponent> GetReducedChildren();
  }

  public abstract record MountComponent<TElement> : BaseComponent, IMountComponent where TElement : VisualElement
  {
    protected abstract TElement OnCreateMountContent();

    /// <summary>
    /// Updates the provided VisualElement to match the current component.
    /// </summary>
    /// <remarks>
    /// NOTE: because elements are reused across contexts, any kind of conditional logic inside OnMount is very
    /// unsafe. For example, if you write code like:
    /// <code>
    /// if (BackgroundColor != null)
    /// {
    ///   element.style.backgroundColor = BackgroundColor;
    /// }
    /// </code>
    /// then the previously-set background color will not be removed correctly!
    /// </remarks>
    /// <param name="element"></param>
    protected abstract void OnMount(TElement element);

    public abstract string Type { get; }

    public override IMountComponent Reduce(GlobalKey globalKey) => this;

    public VisualElement CreateMountContent() => OnCreateMountContent();

    public void Mount(GlobalKey globalKey, VisualElement element)
    {
      GlobalKey = globalKey;
      OnMount((TElement) element);
      GlobalKey = null;
    }

    public virtual void ReduceChildren(GlobalKey globalKey)
    {
    }

    public virtual ImmutableList<IMountComponent> GetReducedChildren() => ImmutableList<IMountComponent>.Empty;
  }

  public abstract record ContainerComponent<TElement> : MountComponent<TElement> where TElement : VisualElement
  {
    readonly IEnumerable<BaseComponent> _children = Enumerable.Empty<BaseComponent>();
    ImmutableList<IMountComponent>? _reduced;

    public IEnumerable<BaseComponent> Children
    {
      init => _children = value;
    }

    public override void ReduceChildren(GlobalKey globalKey)
    {
      Errors.CheckState(_reduced is null, "Error: Already reduced!");
      _reduced = _children.Select((c, i) => c.Reduce(globalKey.Child(c.Key ?? i.ToString()))).ToImmutableList();
    }

    public override ImmutableList<IMountComponent> GetReducedChildren() =>
      Errors.CheckNotNull(_reduced, "You must invoke ReduceChildren() first!");
  }
}