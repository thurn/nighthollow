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

using System.IO;
using System.Linq;
using Nighthollow.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

#nullable enable

namespace Nighthollow.Editor
{
  public sealed class PixelCattleImporter
  {
    const string PixelCattleDirectory = "Assets/ThirdParty/Pixel Cattle Games";
    string _creatureName = null!;
    string _internalCreatureName = null!;

    [MenuItem("Tools/Import/Pixel Cattle Creature %#p")]
    public static void Import()
    {
      if (Selection.assetGUIDs.Length == 1)
      {
        var path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs.First());
        if (!Directory.Exists(path) ||
            Directory.GetParent(Directory.GetParent(path).FullName).Name != "Pixel Cattle Games")
        {
          Debug.LogError($"Invalid directory: {path}");
          return;
        }

        var name = Directory.GetParent(path).Name;
        var internalName = Path.GetFileName(path);

        Debug.Log($"Importing {name} from {internalName}");

        new PixelCattleImporter
        {
          _creatureName = name,
          _internalCreatureName = internalName
        }.Run();
      }
      else
      {
        Debug.LogError("Please select the asset directory");
      }
    }

    void Run()
    {
      var template = AssetDatabase.LoadAssetAtPath<GameObject>(
        $"{PixelCattleDirectory}/{_creatureName}/{_internalCreatureName}/{_internalCreatureName}.prefab");
      var prefab = Object.Instantiate(template);
      UpdatePrefab(prefab);
      var newPath = $"Assets/Resources/Creatures/User/{_creatureName}.prefab";
      AssetDatabase.DeleteAsset(newPath);
      PrefabUtility.SaveAsPrefabAsset(prefab, newPath);
      Object.DestroyImmediate(prefab);

      Debug.Log($"{_creatureName} imported. Please manually configure: " +
                "1) box collider size & position " +
                "2) attachment/projectile/healthbar positions.");
    }

    void UpdatePrefab(GameObject prefab)
    {
      Object.DestroyImmediate(prefab.GetComponent<AnimationEvent>());
      Object.DestroyImmediate(prefab.GetComponent<Animator>());
      Object.DestroyImmediate(prefab.GetComponent<SortingGroup>());

      var projectileSource = new GameObject("ProjectileSource");
      projectileSource.transform.SetParent(prefab.transform);
      projectileSource.transform.localPosition = new Vector2(x: 100, y: 75);

      var healthbarPosition = new GameObject("HealthbarPosition");
      healthbarPosition.transform.SetParent(prefab.transform);
      healthbarPosition.transform.localPosition = new Vector2(x: 50, y: 200);

      AttachmentDisplay? attachmentDisplay = null;
      foreach (var t in prefab.GetComponentsInChildren<Transform>())
      {
        if (t.gameObject.name.Contains("buttock") && t.childCount == 0)
        {
          var child = new GameObject("AttachmentDisplay");
          child.transform.SetParent(t);
          child.transform.localPosition = Vector3.zero;
          child.transform.localScale = Vector3.one;
          attachmentDisplay = child.AddComponent<AttachmentDisplay>();
          break;
        }
      }

      var creature = prefab.AddComponent<Creature2>();
      if (attachmentDisplay)
      {
        creature.EditorSetReferences(projectileSource.transform, healthbarPosition.transform, attachmentDisplay!);
      }
      else
      {
        Debug.LogError("Buttock not found!");
      }

      var sortingGroup = prefab.AddComponent<SortingGroup>();
      sortingGroup.sortingOrder = 100;

      var animator = prefab.AddComponent<Animator>();
      animator.runtimeAnimatorController = CreateAnimatorOverrideController();

      var collider = prefab.AddComponent<BoxCollider2D>();
      collider.isTrigger = true;
      collider.size = new Vector2(x: 100, y: 200);
      collider.offset = new Vector2(x: 50, y: 100);

      var rigidbody = prefab.AddComponent<Rigidbody2D>();
      rigidbody.gravityScale = 0.0f;
    }

    AnimatorOverrideController CreateAnimatorOverrideController()
    {
      var template = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(
        "Assets/Animation/Berserker.overrideController");
      var controller = Object.Instantiate(template);
      LoadClip(controller, "death");
      LoadClip(controller, "idle_1");
      LoadClip(controller, "run");
      LoadClip(controller, "skill_1");
      LoadClip(controller, "skill_2");
      LoadClip(controller, "skill_3");
      LoadClip(controller, "skill_4");
      LoadClip(controller, "skill_5");
      LoadClip(controller, "hit_1");

      var newPath = $"Assets/Animation/{_creatureName}.overrideController";
      AssetDatabase.DeleteAsset(newPath);
      AssetDatabase.CreateAsset(controller, newPath);
      return AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(newPath);
    }

    void LoadClip(AnimatorOverrideController controller, string name)
    {
      var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(
        $"{PixelCattleDirectory}/{_creatureName}/{_internalCreatureName}/{name}.anim");
      controller[name] = clip;
    }
  }
}
