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

#nullable enable

using System.Collections.Generic;
using Nighthollow.Utils;
using UnityEngine;

namespace Nighthollow.Services
{
  public sealed class AssetService : MonoBehaviour
  {
    readonly Dictionary<string, Object> _assets = new Dictionary<string, Object>();

    public Sprite GetImage(string address)
    {
      Errors.CheckArgument(_assets.ContainsKey(address), $"Asset not found: {address}");
      return (Sprite) _assets[address];
    }

    public T InstantiatePrefab<T>(string address, Transform? parent = null) where T : Component
    {
      Errors.CheckArgument(_assets.ContainsKey(address), $"Asset not found: {address}");
      return ComponentUtils.InstantiateGameObject<T>((GameObject) _assets[address], parent);
    }
  }
}