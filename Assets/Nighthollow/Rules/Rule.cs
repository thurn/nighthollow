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
using Nighthollow.Services;

#nullable enable

namespace Nighthollow.Rules
{
  /// <summary>
  /// A Rule contains logic to execute when a specific game event occurs. When the associated event occurs and
  /// all of the rule's conditions are satisfied, its effects are applied to change the state of the game.
  /// </summary>
  [MessagePackObject]
  public sealed partial class Rule : IHasEffects
  {
    public Rule(
      EventName eventName = EventName.Unknown,
      string? name = null,
      RuleCategory category = RuleCategory.NoCategory,
      ImmutableList<RuleCondition>? conditions = null,
      ImmutableList<RuleEffect>? effects = null,
      bool oneTime = false,
      bool disabled = false)
    {
      EventName = eventName;
      Name = name;
      Category = category;
      Conditions = conditions ?? ImmutableList<RuleCondition>.Empty;
      Effects = effects ?? ImmutableList<RuleEffect>.Empty;
      OneTime = oneTime;
      Disabled = disabled;
    }

    /// <summary>Human-readable name for this rule.</summary>
    [Key(1)] public string? Name { get; }

    /// <summary>Identifies the event which causes this rule to run.</summary>
    [Key(0)] public EventName EventName { get; }

    /// <summary>Organizational category for this rule.</summary>
    [Key(2)] public RuleCategory Category { get; }

    /// <summary>Predicates to determine whether to run this rule when its event occurs.</summary>
    [Key(3)] public ImmutableList<RuleCondition> Conditions { get; }

    /// <summary>Mutations to apply to the world when the event occurs & the rule runs.</summary>
    [Key(4)] public ImmutableList<RuleEffect> Effects { get; }

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
    public bool Invoke(Scope scope, RuleOutput? output)
    {
      if (Conditions.Any(condition => !condition.Satisfied(scope.WithDependencies(condition.GetDependencies()))))
      {
        return false;
      }

      foreach (var effect in Effects)
      {
        effect.Execute(scope.WithDependencies(effect.GetDependencies()), output);
      }

      return true;
    }

    public ImmutableHashSet<IKey> MissingBindings()
    {
      var spec = EventName.GetSpec();
      if (spec.ParentRegistry is { } parentRegistry)
      {
        var bindings = spec.Bindings()
          .Concat(ServiceRegistryBindings.Get(parentRegistry))
          .Concat(ServiceRegistry.Keys)
          .ToImmutableHashSet();
        var required = Conditions.SelectMany(c => c.GetDependencies())
          .Concat(Effects.SelectMany(e => e.GetDependencies()))
          .ToImmutableHashSet();

        return required.Except(bindings);
      }
      else
      {
        return ImmutableHashSet<IKey>.Empty;
      }
    }

    public IHasEffects WithNewEffects(ImmutableList<RuleEffect> effects) => WithEffects(effects);
  }
}