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
using Nighthollow.Stats;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Data
{
  [MessagePackObject]
  public sealed partial class StatusEffectTypeData
  {
    public StatusEffectTypeData(
      string name,
      bool isNamed = false,
      int maxStacks = 1,
      ImmutableList<ModifierData>? implicitModifiers = null,
      DurationValue? duration = null,
      DurationValue? durationHigh = null,
      string? imageAddress = null,
      string? effectAddress = null)
    {
      Name = name;
      IsNamed = isNamed;
      MaxStacks = maxStacks;
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
    [Key(1)] public bool IsNamed { get; }

    /// <summary>How many times this effect can be applied to the same target.</summary>
    [Key(2)] public int MaxStacks { get; }

    /// <summary>Modifiers applied to the effect target.</summary>
    [Key(3)] public ImmutableList<ModifierData> ImplicitModifiers { get; }

    /// <summary>How long the effect persists for -- if this is null the effect persists indefinitely</summary>
    [Key(4)] public DurationValue? Duration { get; }

    /// <summary>Optionally an upper bound for the duration, if this effect has a range of possible durations.</summary>
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

    public static StatusEffectItemData DefaultItem(int statusEffectTypeId, GameData gameData)
    {
      var value = gameData.StatusEffectTypes[statusEffectTypeId];
      return new StatusEffectItemData(
        statusEffectTypeId,
        value.ImplicitModifiers.Select(m => m.Value != null ? m : m.WithValue(m.ValueLow)).ToImmutableList(),
        value.Duration);
    }
  }

  [MessagePackObject]
  public sealed partial class StatusEffectItemData
  {
    public StatusEffectItemData(
      int statusEffectTypeId,
      ImmutableList<ModifierData>? implicitModifiers = null,
      DurationValue? duration = null)
    {
      StatusEffectTypeId = statusEffectTypeId;
      ImplicitModifiers = implicitModifiers ?? ImmutableList<ModifierData>.Empty;
      Duration = duration;
    }

    [ForeignKey(typeof(StatusEffectTypeData))]
    [Key(0)] public int StatusEffectTypeId { get; }

    [Key(1)] public ImmutableList<ModifierData> ImplicitModifiers { get; }
    [Key(2)] public DurationValue? Duration { get; }

    public StatusEffectData BuildStatusEffect(GameData gameData) =>
      new StatusEffectData(StatusEffectTypeId, gameData.StatusEffectTypes[StatusEffectTypeId], this);
  }

  public sealed class StatusEffectData
  {
    public StatusEffectData(int typeId, StatusEffectTypeData baseType, StatusEffectItemData itemData)
    {
      StatusEffectTypeId = typeId;
      BaseType = baseType;
      Lifetime = itemData.Duration.HasValue ? new TimedLifetime(itemData.Duration.Value) : null;
      Modifiers = itemData.ImplicitModifiers
        .Select(m => m.BuildStatModifier())
        .WhereNotNull()
        .Select(m => Lifetime == null ? m : m.WithLifetime(Lifetime).WithStatusEffectTypeId(StatusEffectTypeId))
        .ToImmutableList();
    }

    public int StatusEffectTypeId { get; }
    public StatusEffectTypeData BaseType { get; }
    public ILifetime? Lifetime { get; }
    public ImmutableList<IStatModifier> Modifiers { get; }
  }
}