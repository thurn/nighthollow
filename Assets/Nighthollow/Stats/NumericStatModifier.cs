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
  public sealed class NumericStatModifier<T> : IStatModifier where T : struct
  {
    public static NumericStatModifier<T> Add(AbstractStat<NumericStatModifier<T>, T> stat, T value) =>
      new NumericStatModifier<T>(stat.StatId, ModifierType.Add, value, null, null);

    public static NumericStatModifier<T>
      Increase(AbstractStat<NumericStatModifier<T>, T> stat, PercentageValue value) =>
      new NumericStatModifier<T>(stat.StatId, ModifierType.Increase, null, value, null);

    public static NumericStatModifier<T> Set(AbstractStat<NumericStatModifier<T>, T> stat, T value) =>
      new NumericStatModifier<T>(stat.StatId, ModifierType.Set, null, null, value);

    NumericStatModifier(
      StatId statId,
      ModifierType type,
      T? add,
      PercentageValue? increase,
      T? ovewrite,
      ILifetime? lifetime = null)
    {
      StatId = statId;
      Type = type;
      AddTo = add;
      IncreaseBy = increase;
      SetTo = ovewrite;
      Lifetime = lifetime;
    }

    public StatId StatId { get; }
    public ModifierType Type { get; }
    public T? AddTo { get; }
    public PercentageValue? IncreaseBy { get; }
    public T? SetTo { get; }
    public ILifetime? Lifetime { get; }

    public IStatModifier WithLifetime(ILifetime lifetime) =>
      new NumericStatModifier<T>(StatId, Type, AddTo, IncreaseBy, SetTo, lifetime);

    public string Describe(string template, IValueData? highValue) =>
      template.Replace("#",
        ModifierDescriptions.NumericModifierString(
          AddTo,
          IncreaseBy,
          SetTo,
          highValue));
  }
}
