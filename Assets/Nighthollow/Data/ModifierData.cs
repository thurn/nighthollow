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
using MessagePack;
using Nighthollow.Delegates.Core;
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Data
{
  [MessagePackObject]
  public sealed partial class ModifierData
  {
    public ModifierData(
      StatId? statId = null,
      ModifierType? modifierType = null,
      DelegateId? delegateId = null,
      IValueData? value = null,
      IValueData? valueLow = null,
      IValueData? valueHigh = null,
      bool targeted = false)
    {
      StatId = statId;
      ModifierType = modifierType;
      DelegateId = delegateId;
      Value = value;
      ValueLow = valueLow;
      ValueHigh = valueHigh;
      Targeted = targeted;
    }

    /// <summary>The Stat which this modifier is operating on.</summary>
    [Key(0)] public StatId? StatId { get; }

    /// <summary>Operation to apply to the provided Stat.</summary>
    [Key(1)] public ModifierType? ModifierType { get; }

    /// <summary>Delegate which should be added to the modifier target.</summary>
    [Key(2)] public DelegateId? DelegateId { get; }

    /// <summary>
    /// Value for the modifier -- the stat value is set by applying <see cref="ModifierType"/> to this value. Will be
    /// null when we are representing a range of possible modifier values (e.g. in a base type) instead of just a single
    /// value.
    /// </summary>
    [Key(3)] public IValueData? Value { get; }

    /// <summary>
    /// Low value for the modifier. Setting this implies that we are representing a range of possible modifiers,
    /// not just a single instance of one.
    /// </summary>
    [Key(4)] public IValueData? ValueLow { get; }

    /// <summary>
    /// High value for the modifier. Setting this implies that we are representing a range of possible modifiers,
    /// not just a single instance of one.
    /// </summary>
    [Key(5)] public IValueData? ValueHigh { get; }

    /// <summary>
    /// Whether the modifier is manually applied to a target. If false, the modifier is automatically
    /// applied to the owner.
    /// </summary>
    [Key(6)] public bool Targeted { get; }

    /// <summary>Returns a <see cref="IStatModifier"/> for this modifier's <see cref="Value"/> if one is set.</summary>
    public IStatModifier? BuildStatModifier() => ModifierForValue(Value);

    public string? Describe(IStatDescriptionProvider descriptionProvider, ImmutableDictionary<int, StatData> stats)
    {
      if (DelegateId.HasValue)
      {
        var description = DelegateMap.Get(DelegateId.Value).Describe(descriptionProvider);
        if (description != null)
        {
          // Delegate descriptions take priority
          return description;
        }
      }

      if (StatId != null)
      {
        var template = stats[(int) StatId.Value].DescriptionTemplate;
        if (template != null)
        {
          if (!template.Contains("#"))
          {
            template = "# " + template; // Default position
          }

          return PopulateTemplate(template);
        }
      }

      return null;
    }

    public IValueData? Parse(string input)
    {
      if (StatId.HasValue &&
          ModifierType.HasValue &&
          Stat.GetStat(StatId.Value).TryParse(input, ModifierType.Value, out var result))
      {
        return result;
      }

      return null;
    }

    public IStatModifier? ModifierForValue(IValueData? value) =>
      StatId.HasValue && ModifierType.HasValue && value != null
        ? Stat.GetStat(StatId.Value).BuildModifier(ModifierType.Value, value)
        : null;

    string? PopulateTemplate(string template) =>
      Value != null
        ? BuildStatModifier()?.Describe(template, null)
        : ModifierForValue(ValueLow)?.Describe(template, Equals(ValueLow, ValueHigh) ? null : ValueHigh);
  }
}
