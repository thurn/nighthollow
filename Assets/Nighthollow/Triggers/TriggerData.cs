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

using System.Collections.Immutable;
using System.Linq;
using MessagePack;
using Nighthollow.Data;
using Nighthollow.Triggers.Events;

#nullable enable

namespace Nighthollow.Triggers
{
  /// <summary>
  /// Represents a Trigger. Triggers contain code to run in response to an event -- the conditions are evaluated and
  /// then, if they all pass, the effects are applied.
  ///
  /// NOTE: This type must contain Union tags for every possible type of Trigger, which must then be synced via the
  /// ./scripts/data.sh script. Failing to run the script after adding a tag here will cause triggers to silently
  /// serialize to null!
  /// </summary>
  [Union(0, typeof(TriggerData<WorldSceneReadyEvent>))]
  [Union(1, typeof(TriggerData<BattleSceneReadyEvent>))]
  [Union(2, typeof(TriggerData<BattleStartedEvent>))]
  [Union(3, typeof(TriggerData<DrewOpeningHandEvent>))]
  [Union(4, typeof(TriggerData<EnemyCreatureSpawnedEvent>))]
  [Union(5, typeof(TriggerData<UserCreaturePlayedEvent>))]
  [Union(6, typeof(TriggerData<TriggerInvokedEvent>))]
  [Union(7, typeof(TriggerData<HexAttackedEvent>))]
  public interface ITrigger
  {
    public string? Name { get; }
    TriggerCategory Category { get; }
    [NestedSheet] string EventDescription { get; }
    [NestedSheet] string ConditionsDescription { get; }
    [NestedSheet] string EffectsDescription { get; }
    bool Looping { get; }
    bool Disabled { get; }

    public ITrigger WithDisabled(bool disabled);
  }

  [MessagePackFormatter(typeof(TriggerDataFormatter<>))]
  public sealed class TriggerData<TEvent> : ITrigger where TEvent : TriggerEvent
  {
    public TriggerData(
      string? name = null,
      TriggerCategory category = TriggerCategory.Uncategorized,
      ImmutableList<ICondition<TEvent>>? conditions = null,
      ImmutableList<IEffect<TEvent>>? effects = null,
      bool looping = false,
      bool disabled = false)
    {
      Name = name;
      Category = category;
      Conditions = conditions ?? ImmutableList<ICondition<TEvent>>.Empty;
      Effects = effects ?? ImmutableList<IEffect<TEvent>>.Empty;
      Looping = looping;
      Disabled = disabled;
    }

    public string? Name { get; }
    public TriggerCategory Category { get; }
    public ImmutableList<ICondition<TEvent>> Conditions { get; }
    public ImmutableList<IEffect<TEvent>> Effects { get; }
    public bool Looping { get; }
    public bool Disabled { get; }

    public string EventDescription => Description.Snippet("When", typeof(TEvent));

    public string ConditionsDescription => Conditions.IsEmpty
      ? "No Conditions"
      : $"If {string.Join("\nAnd ", Conditions.Select(Description.Describe))}";

    public string EffectsDescription => Effects.IsEmpty
      ? "No Effects"
      : $"Then {string.Join("\nAnd ", Effects.Select(Description.Describe))}";

    /// <summary>
    /// Check trigger conditions and then fire effects if they all pass -- returns true if effects fired.
    /// </summary>
    public bool Invoke(TEvent triggerEvent, TriggerOutput? output)
    {
      if (Conditions.Any(condition => !condition.Satisfied(triggerEvent)))
      {
        return false;
      }

      foreach (var effect in Effects)
      {
        effect.Execute(triggerEvent, output);
      }

      return true;
    }

    public override string ToString() => Name ?? "<Unnamed>";

    public ITrigger WithName(string? name) => ReferenceEquals(name, Name)
      ? this
      : new TriggerData<TEvent>(name, Category, Conditions, Effects, Looping, Disabled);

    public ITrigger WithCategory(TriggerCategory category) => category == Category
      ? this
      : new TriggerData<TEvent>(Name, category, Conditions, Effects, Looping, Disabled);

    public ITrigger WithConditions(ImmutableList<ICondition<TEvent>> conditions) =>
      ReferenceEquals(conditions, Conditions)
        ? this
        : new TriggerData<TEvent>(Name, Category, conditions, Effects, Looping, Disabled);

    public ITrigger WithEffects(ImmutableList<IEffect<TEvent>> effects) =>
      ReferenceEquals(effects, Effects)
        ? this
        : new TriggerData<TEvent>(Name, Category, Conditions, effects, Looping, Disabled);

    public ITrigger WithLooping(bool looping) =>
      looping == Looping ? this : new TriggerData<TEvent>(Name, Category, Conditions, Effects, looping, Disabled);

    public ITrigger WithDisabled(bool disabled) =>
      disabled == Disabled ? this : new TriggerData<TEvent>(Name, Category, Conditions, Effects, Looping, disabled);
  }
}