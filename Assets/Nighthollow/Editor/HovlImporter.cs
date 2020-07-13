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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Nighthollow.Components;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Nighthollow.Editor
{
  public sealed class HovlImporter
  {
    [MenuItem("Tools/Import/Hovl Projectile %#h")]
    public static void Import()
    {
      if (Selection.assetGUIDs.Length != 1)
      {
        throw new InvalidOperationException("Please select the projectile prefab");
      }

      var projectilePath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs.First());
      var match = Regex.Match(projectilePath, @"Projectile (\d+)");
      if (!match.Success)
      {
        throw new InvalidOperationException("Match failed");
      }

      var projectilePrefab =
        Object.Instantiate(
          AssetDatabase.LoadAssetAtPath<GameObject>(projectilePath));

      var projectileNumber = int.Parse(match.Groups[1].ToString());
      var flashPrefab =
        Object.Instantiate(
          AssetDatabase.LoadAssetAtPath<GameObject>(
            $"{Directory.GetParent(projectilePath)}/Flash {projectileNumber}.prefab"));
      flashPrefab.AddComponent<TimedEffect>();

      var hitPrefab =
        Object.Instantiate(
          AssetDatabase.LoadAssetAtPath<GameObject>(
            $"{Directory.GetParent(projectilePath)}/Hit {projectileNumber}.prefab"));
      hitPrefab.AddComponent<TimedEffect>();

      var flash = Save(flashPrefab, $"Assets/Resources/Projectiles/Flashes/Flash {projectileNumber}.prefab");
      var hit = Save(hitPrefab, $"Assets/Resources/Projectiles/Hits/Hit {projectileNumber}.prefab");

      Object.DestroyImmediate(projectilePrefab.GetComponent<ProjectileMover>());
      Object.DestroyImmediate(projectilePrefab.GetComponent<Rigidbody>());
      Object.DestroyImmediate(projectilePrefab.GetComponent<SphereCollider>());
      projectilePrefab.AddComponent<Projectile>().SetReferences(
        flash.GetComponent<TimedEffect>(), hit.GetComponent<TimedEffect>());
      var rigidbody = projectilePrefab.AddComponent<Rigidbody2D>();
      rigidbody.gravityScale = 0f;

      var collider = projectilePrefab.AddComponent<CircleCollider2D>();
      collider.isTrigger = true;
      collider.radius = 0.25f;

      Save(projectilePrefab, $"Assets/Resources/Projectiles/Projectile {projectileNumber}.prefab");

      Debug.Log($"Projectile {projectileNumber} imported");
    }

    static GameObject Save(GameObject prefab, string path)
    {
      AssetDatabase.DeleteAsset(path);
      var result = PrefabUtility.SaveAsPrefabAsset(prefab, path);
      Object.DestroyImmediate(prefab);
      return result;
    }
  }
}