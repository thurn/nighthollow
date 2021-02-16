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

using Nighthollow.Data;

#nullable enable

namespace Nighthollow.Stats
{
  public interface IStatModifier
  {
    public StatId StatId { get; }

    ModifierType Type { get; }

    IStatModifier Unwrap();

    /// <summary>
    /// A modifier may optionally be constructed with a lifetime, which dictates when this modifier should be removed
    /// from its containing StatTable.
    /// </summary>
    ILifetime? Lifetime { get; }

    IStatModifier WithLifetime(ILifetime lifetime);

    /// <summary>
    /// A modifier may be associated with a given status effect.
    /// </summary>
    int? StatusEffectTypeId { get; }

    IStatModifier WithStatusEffectTypeId(int statusEffectTypeId);

    /// <summary>
    /// Produce a description of this modifier based on the provided template. Templates for numeric stats contain the
    /// character '#' which should be replaced with the modifier description. Templates for boolean stats contain two
    /// descriptions separated by the '/' character, which correspond to true / false respectively.
    ///
    /// If <paramref name="highValue"/> is provided, this description is for a range of numeric modifier values, and
    /// the # character should be replaced with a hyphen-separated range of currentValue - highValue.
    /// </summary>
    public string Describe(string template, IValueData? highValue);
  }

  public abstract class AbstractStatModifier : IStatModifier
  {
    protected AbstractStatModifier(
      StatId statId,
      ModifierType type,
      ILifetime? lifetime = null,
      int? statusEffectTypeId = null)
    {
      StatId = statId;
      Type = type;
      Lifetime = lifetime;
      StatusEffectTypeId = statusEffectTypeId;
    }

    public StatId StatId { get; }

    public ModifierType Type { get; }

    public virtual IStatModifier Unwrap() => this;

    public ILifetime? Lifetime { get; }

    public int? StatusEffectTypeId { get; }

    public IStatModifier WithLifetime(ILifetime lifetime) =>
      new StatModifierWrapper(this, StatId, Type, lifetime, StatusEffectTypeId);

    public IStatModifier WithStatusEffectTypeId(int statusEffectTypeId) =>
      new StatModifierWrapper(this, StatId, Type, Lifetime, statusEffectTypeId);

    public abstract string Describe(string template, IValueData? highValue);
  }

  public sealed class StatModifierWrapper : AbstractStatModifier
  {
    readonly IStatModifier _child;

    public StatModifierWrapper(
      IStatModifier child,
      StatId statId,
      ModifierType type,
      ILifetime? lifetime = null,
      int? statusEffectTypeId = null) : base(statId, type, lifetime, statusEffectTypeId)
    {
      _child = child;
    }

    public override IStatModifier Unwrap() => _child;

    public override string Describe(string template, IValueData? highValue) => _child.Describe(template, highValue);
  }
}