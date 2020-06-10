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
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Nighthollow
{
  public sealed class Asset : IEquatable<Asset>
  {
    public readonly string Address;
    public readonly string Type;

    public Asset(string address, string type)
    {
      Address = address;
      Type = type;
    }

    public override string ToString()
    {
      return $"{nameof(Address)}: {Address}, {nameof(Type)}: {Type}";
    }

    public bool Equals(Asset other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return Address == other.Address && Type == other.Type;
    }

    public override bool Equals(object obj)
    {
      return ReferenceEquals(this, obj) || obj is Asset other && Equals(other);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return ((Address != null ? Address.GetHashCode() : 0) * 397) ^ (Type != null ? Type.GetHashCode() : 0);
      }
    }

    public static bool operator ==(Asset left, Asset right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(Asset left, Asset right)
    {
      return !Equals(left, right);
    }
  }

  public sealed class Root : MonoBehaviour
  {
    public Camera MainCamera;
    public Canvas MainCanvas;
    public RectTransform DrawCardsPosition;
    public Hand Hand;
    public RectTransform DebugPanel;

    readonly Dictionary<string, Object> _assetCache = new Dictionary<string, Object>();

    public GameObject GetPrefab(Asset asset) => Get<GameObject>(asset, "prefab");

    public Sprite GetSprite(Asset asset) => Get<Sprite>(asset, "sprite");

    public void LoadAssets(IEnumerable<Asset> assets, Action onComplete = null)
    {
      StartCoroutine(LoadAssetsAsync(assets, onComplete));
    }

    T Get<T>(Asset asset, string expectedType) where T : Object
    {
      if (!asset.Type.Equals(expectedType))
      {
        throw new ArgumentException($"Expected asset of type {expectedType} but got type {asset.Type}");
      }

      _assetCache.TryGetValue(asset.Address, out var value);
      if (value)
      {
        return value as T;
      }

      throw new ArgumentException($"Asset not found: ${asset.Address}");
    }

    IEnumerator<YieldInstruction> LoadAssetsAsync(IEnumerable<Asset> assets, Action onComplete)
    {
      var list = assets.ToList();
      var requests = list.Select(asset => Resources.LoadAsync(asset.Address, TypeForAsset(asset))).ToList();

      foreach (var request in requests)
      {
        yield return request;
      }

      for (var i = 0; i < requests.Count; ++i)
      {
        var asset = requests[i].asset;
        if (asset)
        {
          _assetCache[list[i].Address] = asset;
        }
        else
        {
          throw new InvalidOperationException($"Asset not found: {list[i].Address}");
        }
      }

      onComplete?.Invoke();
    }

    Type TypeForAsset(Asset asset)
    {
      switch (asset.Type)
      {
        case "prefab":
          return typeof(GameObject);
        case "sprite":
          return typeof(Sprite);
        default:
          throw new ArgumentException($"Unknown asset type: {asset.Type}");
      }
    }
  }
}