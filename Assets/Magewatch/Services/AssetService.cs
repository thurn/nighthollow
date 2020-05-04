// Copyright The Magewatch Project

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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Magewatch.Data;
using Magewatch.Utils;
using UnityEngine;

namespace Magewatch.Services
{
  public sealed class AssetService : MonoBehaviour
  {
    public void InstantiateCard(CardData cardData, Action onComplete)
    {
      StartCoroutine(PopulateAssets(new List<IAsset>
      {
        cardData.Prefab,
        cardData.Image,
        cardData.CreatureData?.Prefab
      }, onComplete));
    }

    IEnumerator InstantiateCardAsync2(CardData cardData, Action callback)
    {
      var prefabRequest = Resources.LoadAsync<GameObject>(cardData.Prefab.Address);
      var imageRequest = Resources.LoadAsync<Sprite>(cardData.Image.Address);

      var creatureRequest = cardData.CreatureData == null
        ? null
        : Resources.LoadAsync<GameObject>(cardData.CreatureData.Prefab.Address);

      yield return prefabRequest;
      yield return imageRequest;
      if (creatureRequest != null)
      {
        yield return creatureRequest;
      }

      Errors.CheckNotNull(prefabRequest.asset);
      cardData.Prefab.Value = Instantiate(prefabRequest.asset, Root.Instance.MainCanvas.transform) as GameObject;
      cardData.Image.Value = Errors.CheckNotNull(imageRequest.asset as Sprite);

      if (creatureRequest != null)
      {
        cardData.CreatureData.Prefab.Value = Instantiate(creatureRequest.asset as GameObject);
      }

      callback();
    }

    IEnumerator PopulateAssets(List<IAsset> assets, Action onComplete)
    {
      assets.RemoveAll(x => x == null);
      var requests = assets.Select(asset => Resources.LoadAsync(asset.GetAddress(), asset.GetAssetType())).ToList();

      foreach (var request in requests)
      {
        yield return request;
      }

      for (var i = 0; i < requests.Count; ++i)
      {
        Errors.CheckNotNull(requests[i].asset);
        assets[i].SetValueUnchecked(requests[i].asset);
      }

      onComplete();
    }
  }
}