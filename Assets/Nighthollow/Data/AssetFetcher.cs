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
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

#nullable enable

namespace Nighthollow.Data
{
  public sealed class AssetService
  {
    readonly ImmutableDictionary<string, Object> _assets;

    public AssetService(ImmutableDictionary<string, Object> assets)
    {
      _assets = assets;
    }

    public Sprite GetImage(string address)
    {
      Errors.CheckArgument(_assets.ContainsKey(address), $"Asset not found: {address}");
      return (Sprite) _assets[address];
    }

    public Tile GetTile(string address)
    {
      Errors.CheckArgument(_assets.ContainsKey(address), $"Asset not found: {address}");
      return (Tile) _assets[address];
    }

    public T InstantiatePrefab<T>(string address, Transform? parent = null) where T : Component
    {
      Errors.CheckArgument(_assets.ContainsKey(address), $"Asset not found: {address}");
      return ComponentUtils.InstantiateGameObject<T>((GameObject) _assets[address], parent);
    }
  }

  public static class AssetFetcher
  {
    public static IEnumerator<WaitUntil> FetchAssets(
      GameData gameData,
      bool synchronous,
      Action<AssetService> onComplete)
    {
      Debug.Log("Fetching Assets...");
      var requests = new Dictionary<string, ResourceRequest>();
      var result = ImmutableDictionary.CreateBuilder<string, Object>();

      foreach (var creature in gameData.CreatureTypes.Values)
      {
        Load<GameObject>(requests, result, synchronous, creature.PrefabAddress);
        if (creature.ImageAddress != null)
        {
          Load<Sprite>(requests, result, synchronous, creature.ImageAddress);
        }
      }

      foreach (var skill in gameData.SkillTypes.Values)
      {
        if (skill.Address != null)
        {
          Load<GameObject>(requests, result, synchronous, skill.Address);
        }
      }

      foreach (var statusEffect in gameData.StatusEffectTypes.Values)
      {
        if (statusEffect.ImageAddress != null)
        {
          Load<Sprite>(requests, result, synchronous, statusEffect.ImageAddress);
        }
      }

      foreach (var kingdom in gameData.Kingdoms.Values)
      {
        Load<Tile>(requests, result, synchronous, kingdom.TileImageAddress);
      }

      foreach (var resourceType in gameData.ResourceTypes.Values)
      {
        if (resourceType.ImageAddress != null)
        {
          Load<Sprite>(requests, result, synchronous, resourceType.ImageAddress);
        }
      }

      if (synchronous)
      {
        onComplete(new AssetService(result.ToImmutable()));
      }
      else
      {
        yield return new WaitUntil(() => requests.Values.All(r => r.isDone));
        Debug.Log("Got Asset Responses...");

        foreach (var request in requests)
        {
          if (request.Value.asset)
          {
            result[request.Key] = request.Value.asset;
          }
          else
          {
            Debug.LogError($"Null asset for {request.Key}");
          }
        }

        onComplete(new AssetService(result.ToImmutable()));
      }
    }

    static void Load<T>(
      IDictionary<string, ResourceRequest> requests,
      ImmutableDictionary<string, Object>.Builder result,
      bool synchronous,
      string key) where T : Object
    {
      if (!string.IsNullOrWhiteSpace(key))
      {
        if (synchronous)
        {
          result[key] = Resources.Load<T>(key);
        }
        else
        {
          requests[key] = Resources.LoadAsync<T>(key);
        }
      }
    }
  }
}