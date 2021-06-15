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
using Nighthollow.Rules;

#nullable enable

namespace Nighthollow.Interface.Components.Core
{
  public sealed class GlobalKey
  {
    readonly ComponentController _controller;
    readonly string _key;
    int _currentHook;

    public static GlobalKey Root(ComponentController controller) => new(controller, "/");

    GlobalKey(ComponentController controller, string key)
    {
      _controller = controller;
      _key = key;
    }

    public Scope Scope => _controller.Scope;

    string GetKey() => $"{_key}:{_currentHook++}";

    public GlobalKey Child(string key) => new(_controller, $"{_key}/{key}");

    public IState<T> UseState<T>(Type componentType, T initialValue) =>
      _controller.UseState((componentType, GetKey()), initialValue);

    public T? UseResource<T>(string address) where T : UnityEngine.Object
    {
      return _controller.UseResource<T>(address);
    }

    public override string ToString() => GetKey();
  }
}