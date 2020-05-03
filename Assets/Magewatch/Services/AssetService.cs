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
using Magewatch.Data;
using Magewatch.Utils;
using UnityEngine;

namespace Magewatch.Services
{
  public sealed class AssetService : MonoBehaviour
  {
    public void InstantiateCard(CardData cardData, Action callback)
    {
      StartCoroutine(InstantiateCardAsync(cardData, callback));
    }

    IEnumerator InstantiateCardAsync(CardData cardData, Action callback)
    {
      var prefabRequest = Resources.LoadAsync<GameObject>(cardData.Prefab.Address);
      var imageRequest = Resources.LoadAsync<Sprite>(cardData.Image.Address);
      yield return prefabRequest;
      yield return imageRequest;
      Errors.CheckNotNull(prefabRequest.asset);
      cardData.Prefab.Value = Instantiate(prefabRequest.asset, Root.Instance.MainCanvas.transform) as GameObject;
      cardData.Image.Value = Errors.CheckNotNull(imageRequest.asset as Sprite);
      callback();
    }
  }
}