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
using Magewatch.Data;
using Magewatch.Services;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Magewatch.Utils
{
  public static class ComponentUtils
  {
    public static T Instantiate<T>(Asset prefab, Transform parent = null) where T : Component
    {
      Errors.CheckNotNull(prefab);
      var prefabObject = Object.Instantiate(Root.Instance.AssetService.Get<GameObject>(prefab), parent);
      var result = prefabObject.GetComponent<T>();
      if (!result)
      {
        throw new NullReferenceException($"Expected a component of type {typeof(T).FullName}");
      }

      if (!parent)
      {
        // Instantiate things safely out of view if there's no parent specified :)
        prefabObject.transform.position = 1000f * Vector3.one;
      }

      return result;
    }

    public static T Instantiate<T>(T prefabComponent, Transform parent = null) where T : Component
    {
      Errors.CheckNotNull(prefabComponent);
      var prefabObject = Object.Instantiate(prefabComponent.gameObject, parent);
      var result = prefabObject.GetComponent<T>();
      if (!result)
      {
        throw new NullReferenceException($"Expected a component of type {typeof(T).FullName}");
      }

      if (!parent)
      {
        // Instantiate things safely out of view if there's no parent specified :)
        prefabObject.transform.position = 1000f * Vector3.one;
      }

      return result;
    }

    public static T GetComponent<T>(Component component) where T : Component
    {
      Errors.CheckNotNull(component);
      var result = component.GetComponent<T>();
      if (result == null)
      {
        throw new NullReferenceException($"Expected a component of type {typeof(T).FullName}");
      }

      return result;
    }

    public static T GetComponent<T>(GameObject gameObject) where T : Component
    {
      Errors.CheckNotNull(gameObject);
      var result = gameObject.GetComponent<T>();
      if (!result)
      {
        throw new NullReferenceException($"Expected a component of type {typeof(T).FullName}");
      }

      return result;
    }
  }
}