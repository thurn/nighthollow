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
using Nighthollow.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

#nullable enable

namespace Nighthollow.Services
{
  public sealed class AssetService
  {
    readonly Dictionary<string, Object> _assets = new Dictionary<string, Object>();

    AssetService()
    {
    }

    public static void Initialize(MonoBehaviour runner, GameDataService gameDataService, Action<AssetService> action)
    {
      var service = new AssetService();
      runner.StartCoroutine(service.FetchAssetsAsync(gameDataService, () => action(service)));
    }

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

    IEnumerator<WaitUntil> FetchAssetsAsync(GameDataService gameDataService, Action onComplete)
    {
      Debug.Log("Fetching Assets...");
      var requests = new Dictionary<string, ResourceRequest>();
      foreach (var creature in gameDataService.AllCreatureTypes)
      {
        requests[creature.PrefabAddress] = Resources.LoadAsync<GameObject>(creature.PrefabAddress);
        if (creature.ImageAddress != null)
        {
          requests[creature.ImageAddress] = Resources.LoadAsync<Sprite>(creature.ImageAddress);
        }
      }

      foreach (var skill in gameDataService.AllSkillTypes)
      {
        if (skill.Address != null)
        {
          requests[skill.Address] = Resources.LoadAsync<GameObject>(skill.Address);
        }
      }

      foreach (var resource in gameDataService.AllResources)
      {
        requests[resource.ImageAddress] = Resources.LoadAsync<Sprite>(resource.ImageAddress);
      }

      yield return new WaitUntil(() => requests.Values.All(r => r.isDone));
      Debug.Log("Got Asset Responses...");

      foreach (var request in requests)
      {
        Errors.CheckNotNull(request.Value.asset, $"Null asset for {request.Key}");
        _assets[request.Key] = request.Value.asset;
      }

      onComplete();
    }
  }
}
