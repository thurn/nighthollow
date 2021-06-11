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
using UnityEngine;

#nullable enable

namespace Nighthollow.Interface.Components
{
  public sealed class GlobalKey
  {
    readonly ComponentRoot _root;
    readonly string _key;
    int _currentHook;

    public static GlobalKey Root(ComponentRoot root) => new(root, "/");

    GlobalKey(ComponentRoot root, string key)
    {
      _root = root;
      _key = key;
    }

    public GlobalKey Child(string key) => new(_root, $"{_key}/{key}");

    public IState<T> UseState<T>(Type componentType, T initialValue) =>
      _root.UseState((componentType, $"{_key}:{_currentHook++}"), initialValue);

    public Sprite? UseSprite(string? address)
    {
      return _root.UseSprite(address);
    }
  }
}