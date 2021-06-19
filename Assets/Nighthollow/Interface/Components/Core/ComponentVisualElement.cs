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
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface.Components.Core
{
  public sealed class ComponentVisualElement : VisualElement
  {
    readonly Dictionary<Type, object> _callbacks = new();
    ImmutableHashSet<string> _classNames = ImmutableHashSet<string>.Empty;

    public IDragReceiver? DragReceiver { get; set; }

    public void SetClassNames(ImmutableHashSet<string> classNames)
    {
      foreach (var className in _classNames)
      {
        RemoveFromClassList(className);
      }

      foreach (var className in classNames)
      {
        AddToClassList(className);
      }

      _classNames = classNames;
    }

    public void RegisterExclusiveCallback<TEventType>(
      EventCallback<TEventType>? callback,
      TrickleDown useTrickleDown = TrickleDown.NoTrickleDown)
      where TEventType : EventBase<TEventType>, new()
    {
      var t = typeof(TEventType);
      if (_callbacks.ContainsKey(t))
      {
        UnregisterCallback((EventCallback<TEventType>) _callbacks[t]);
        _callbacks.Remove(t);
      }

      if (callback != null)
      {
        RegisterCallback(callback, useTrickleDown);
        _callbacks[t] = callback;
      }
    }
  }
}