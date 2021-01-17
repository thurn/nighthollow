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
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Data
{
  [MessagePackObject]
  public sealed partial class StatusEffectTypeData
  {
    public StatusEffectTypeData(
      string name,
      bool isFirstClass = false,
      int? maxStacks = null,
      ImmutableList<ModifierData>? implicitModifiers = null,
      DurationValue? duration = null,
      DurationValue? durationHigh = null,
      string? imageAddress = null,
      string? effectAddress = null)
    {
      Name = name;
      IsFirstClass = isFirstClass;
      MaxStacks = maxStacks ?? 1;
      ImplicitModifiers = implicitModifiers ?? ImmutableList<ModifierData>.Empty;
      Duration = duration;
      DurationHigh = durationHigh;
      ImageAddress = imageAddress;
      EffectAddress = effectAddress;
    }

    /// <summary>A name to identify this effect.</summary>
    [Key(0)] public string Name { get; }

    /// <summary>
    /// True if this effect should be communicated in the UI using its name. Typically this is false for status effects
    /// which are directly tied to a single skill, where the skill name itself is what's shown in the UI.
    /// </summary>
    [Key(1)] public bool IsFirstClass { get; }

    /// <summary>How many times this effect can be applied to the same target.</summary>
    [Key(2)] public int MaxStacks { get; }

    /// <summary>Modifiers applied to the effect target.</summary>
    [Key(3)] public ImmutableList<ModifierData> ImplicitModifiers { get; }

    /// <summary>How long the effect persists for -- if this is null the effect persists indefinitely</summary>
    [Key(4)] public DurationValue? Duration { get; }

    /// <summary>Optionally an upper bound for the duration, if this effect has a range of possible duration.</summary>
    [Key(5)] public DurationValue? DurationHigh { get; }

    /// <summary>
    /// Address for an image representing this status effect
    /// </summary>
    [Key(6)] public string? ImageAddress { get; }

    /// <summary>
    /// Address for a particle effect to play when this status effect is applied.
    /// </summary>
    [Key(7)] public string? EffectAddress { get; }

    public override string ToString() => Name;
  }

  [MessagePackObject]
  public sealed partial class StatusEffectItemData
  {
    public StatusEffectItemData(
      int statusEffectTypeId,
      ImmutableList<ModifierData> implicitModifiers,
      DurationValue? duration)
    {
      StatusEffectTypeId = statusEffectTypeId;
      ImplicitModifiers = implicitModifiers;
      Duration = duration;
    }

    [Key(0)] public int StatusEffectTypeId { get; }
    [Key(1)] public ImmutableList<ModifierData> ImplicitModifiers { get; }
    [Key(2)] public DurationValue? Duration { get; }
  }

  public sealed partial class StatusEffectData : StatEntity
  {
    public StatusEffectData(StatTable stats, StatusEffectTypeData baseType, StatusEffectItemData itemData)
    {
      Stats = stats;
      BaseType = baseType;
      ItemData = itemData;
    }

    [Field] public override StatTable Stats { get; }
    [Field] public StatusEffectTypeData BaseType { get; }
    [Field] public StatusEffectItemData ItemData { get; }
  }
}
