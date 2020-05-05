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
    public void FetchCardAssets(CardData cardData, Action onComplete)
    {
      StartCoroutine(PopulateAssets(new List<IAsset>
      {
        cardData.Prefab,
        cardData.Image,
        cardData.CreatureData?.Prefab
      }, onComplete));
    }

    public void FetchCreatureAssets(CreatureData creatureData, Action onComplete)
    {
      StartCoroutine(PopulateAssets(new List<IAsset>
      {
        creatureData.Prefab
      }, onComplete));
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