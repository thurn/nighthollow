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

#nullable enable

namespace Nighthollow.Triggers
{
  /// <summary>
  /// A Rule contains logic to execute when a specific game event occurs. When the associated event occurs and
  /// all of the rule's conditions are satisfied, its effects are applied to change the state of the game.
  /// </summary>
  [MessagePackObject]
  public sealed partial class Rule
  {
    public Rule(
      EventType triggerEvent,
      string? name,
      TriggerCategory category,
      ImmutableList<ICondition> conditions,
      ImmutableList<IEffect> effects,
      bool oneTime,
      bool disabled)
    {
      TriggerEvent = triggerEvent;
      Name = name;
      Category = category;
      Conditions = conditions;
      Effects = effects;
      OneTime = oneTime;
      Disabled = disabled;
    }

    /// <summary>Identifies the event which causes this rule to run.</summary>
    [Key(0)] public EventType TriggerEvent { get; }

    /// <summary>Human-readable name for this rule.</summary>
    [Key(1)] public string? Name { get; }

    /// <summary>Organizational category for this rule.</summary>
    [Key(2)] public TriggerCategory Category { get; }

    /// <summary>Predicates to determine whether to run this rule when its event occurs.</summary>
    [Key(3)] public ImmutableList<ICondition> Conditions { get; }

    /// <summary>Mutations to apply to the world when the event occurs & the rule runs.</summary>
    [Key(4)] public ImmutableList<IEffect> Effects { get; }

    /// <summary>If true, the rule will disable itself after running.</summary>
    [Key(5)] public bool OneTime { get; }

    /// <summary>If true, the rule will not run unless directly invoked.</summary>
    [Key(6)] public bool Disabled { get; }

    string EventDescription() => Description.Snippet("When", typeof(int));

    string ConditionsDescription() => Conditions.IsEmpty
      ? ""
      : $"If {string.Join("\nAnd ", Conditions.Select(Description.Describe))}";

    string EffectsDescription() => Effects.IsEmpty
      ? ""
      : $"Then {string.Join("\nAnd ", Effects.Select(Description.Describe))}";

    string Describe() => string.Join(" ", EventDescription(), ConditionsDescription(), EffectsDescription());

    public override string ToString()
    {
      var description = Describe();
      return description.Length < 100 ? description : $"{description.Substring(0, 100)}...";
    }

    /// <summary>
    /// Check conditions and then fire effects if they all pass -- returns true if effects fired.
    /// </summary>
    public bool Invoke(Scope scope, TriggerOutput? output)
    {
      if (Conditions.Any(condition => !condition.Satisfied(scope.WithDependencies(condition.Dependencies))))
      {
        return false;
      }

      foreach (var effect in Effects)
      {
        effect.Execute(scope.WithDependencies(effect.Dependencies), output);
      }

      return true;
    }
  }
}