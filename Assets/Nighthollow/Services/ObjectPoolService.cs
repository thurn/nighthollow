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


using System.Collections.Generic;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Services
{
  public sealed class ObjectPoolService : MonoBehaviour
  {
    readonly Dictionary<int, List<GameObject>> _pools = new Dictionary<int, List<GameObject>>();

    public T Create<T>(T prefab, Vector3 position, Transform? parent = null) where T : Component =>
      ComponentUtils.GetComponent<T>(Create(prefab.gameObject, position, parent));

    public GameObject Create(GameObject prefab, Vector3 position, Transform? parent = null)
    {
      var instanceId = prefab.GetInstanceID();
      if (_pools.ContainsKey(instanceId))
      {
        var list = _pools[instanceId];
        foreach (var pooledObject in list)
        {
          if (!pooledObject.activeSelf)
          {
            pooledObject.transform.SetParent(parent, worldPositionStays: false);
            pooledObject.transform.position = position;
            pooledObject.SetActive(value: true);
            return pooledObject;
          }
        }
      }
      else
      {
        _pools[instanceId] = new List<GameObject>();
      }

      var result = Instantiate(prefab, parent, worldPositionStays: false);
      result.transform.position = position;
      _pools[instanceId].Add(result.gameObject);
      return result;
    }
  }
}
