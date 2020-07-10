using UnityEngine;
using UnityEditor;

namespace Nighthollow.Editor
{
  public sealed class CreatureImporter
  {
    const string PixelCattleDirectory = "Assets/ThirdParty/Pixel Cattle Games";
    string CreatureName;
    string InternalCreatureName;

    [MenuItem("Tools/Import/Pixel Cattle Creature")]
    public static void Import()
    {
      new CreatureImporter
      {
        CreatureName = "Wizard",
        InternalCreatureName = "divine_mage"
      }.Run();
    }

    void Run()
    {
      var template = AssetDatabase.LoadAssetAtPath<GameObject>(
        $"{PixelCattleDirectory}/{CreatureName}/{InternalCreatureName}/{InternalCreatureName}.prefab");
      var prefab = Object.Instantiate(template);
      UpdatePrefab(prefab);
      var newPath = $"Assets/Resources/Creatures/User/{CreatureName}.prefab";
      AssetDatabase.DeleteAsset(newPath);
      PrefabUtility.SaveAsPrefabAsset(prefab, newPath);
      Object.DestroyImmediate(prefab);
    }

    void UpdatePrefab(GameObject prefab)
    {
      Object.DestroyImmediate(prefab.GetComponent<AnimationEvent>());
      prefab.GetComponent<Animator>().runtimeAnimatorController = CreateAnimatorOverrideController();
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

      var newPath = $"Assets/Animation/{CreatureName}.overrideController";
      AssetDatabase.DeleteAsset(newPath);
      AssetDatabase.CreateAsset(controller, newPath);
      return AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(newPath);
    }

    void LoadClip(AnimatorOverrideController controller, string name)
    {
      var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(
        $"{PixelCattleDirectory}/{CreatureName}/{InternalCreatureName}/{name}.anim");
      controller[name] = clip;
    }
  }
}