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
using System.Collections.Immutable;
using MessagePack;
using Nighthollow.Rules;
using Nighthollow.Rules.Effects;
using Nighthollow.Services;
using UnityEngine;

#nullable enable

namespace Nighthollow.Editing.Scenarios
{
  /// <summary>
  /// Represents a test scenario. Loads a specific scene and then applies the effects in <see cref="Effects"/>
  /// sequentially.
  /// </summary>
  [MessagePackObject]
  public sealed partial class ScenarioData : IHasEffects
  {
    const string LoadScenarioKey = "Scenarios/LoadScenario";

    public ScenarioData(
      string? name = null,
      string? description = null,
      SceneName scene = SceneName.Unknown,
      ImmutableList<RuleEffect>? effects = null,
      bool repeating = false)
    {
      Name = name ?? "";
      Description = description ?? "";
      Scene = scene;
      Effects = effects ?? ImmutableList<RuleEffect>.Empty;
      Repeating = repeating;
    }

    [Key(0)] public string Name { get; }

    [Key(1)] public string Description { get; }

    [Key(2)] public SceneName Scene { get; }

    [Key(3)] public ImmutableList<RuleEffect> Effects { get; }

    /// <summary>
    /// If true, this scenario will not be removed from the "on scene load" list, causing it to run every time
    /// its associated scene is loaded.
    /// </summary>
    [Key(4)] public bool Repeating { get; }

    /// <summary>
    /// Initiates a scenario. This causes the associated scene to be loaded and the effects to be applied. Normal
    /// "on scene loaded" events will *not* fire.
    /// </summary>
    public static void Start(ServiceRegistry registry, int entityId)
    {
      PlayerPrefs.SetInt(LoadScenarioKey, entityId);
      LoadSceneEffect.LoadAsync(registry.Database.Snapshot().Scenarios[entityId].Scene);
    }

    /// <summary>
    /// Check if a scenario load has been requested and invoke it if available. Returns true if a scenario was found.
    /// </summary>
    public static bool Invoke(ServiceRegistry registry)
    {
      var scenarioId = PlayerPrefs.GetInt(LoadScenarioKey, 0);
      if (scenarioId != 0)
      {
        var scenario = registry.Database.Snapshot().Scenarios[scenarioId];
        registry.CoroutineRunner.StartCoroutine(scenario.InvokeAsync(registry));
        return true;
      }

      return false;
    }

    public static void ClearPreference()
    {
      PlayerPrefs.SetInt(LoadScenarioKey, 0);
    }

    IEnumerator<YieldInstruction> InvokeAsync(ServiceRegistry registry)
    {
      Debug.Log($"Invoking Scenario '{Name}'");

      if (!Repeating)
      {
        PlayerPrefs.SetInt(LoadScenarioKey, 0);
      }

      foreach (var effect in Effects)
      {
        yield return new WaitForSeconds(1f);
        Debug.Log($"ScenarioData::InvokeAsync {effect}");
        effect.Execute(registry.Scope.WithDependencies(effect.GetDependencies()), null);
      }
    }

    public IHasEffects WithNewEffects(ImmutableList<RuleEffect> effects) => WithEffects(effects);
  }
}