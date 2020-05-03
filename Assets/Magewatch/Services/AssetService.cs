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
using Magewatch.Utils;
using UnityEngine;

namespace Magewatch.Services
{
  public sealed class AssetService : MonoBehaviour
  {
    public void Instantiate<T>(string path, Transform parent, Action<T> callback) where T : Component
    {
      StartCoroutine(InstantiateAsync(path, parent, callback));
    }

    IEnumerator InstantiateAsync<T>(string path, Transform parent, Action<T> callback) where T : Component
    {
      Debug.Log($"AssetService::InstantiateAsync> Loading {path}");
      var request = Resources.LoadAsync<GameObject>(path);
      yield return request;
      Errors.CheckNotNull(request.asset);
      Debug.Log($"AssetService::InstantiateAsync> Loaded {request.asset}");
      var result = Instantiate(request.asset, parent) as GameObject;
      callback(ComponentUtils.GetComponent<T>(result));
    }
  }
}