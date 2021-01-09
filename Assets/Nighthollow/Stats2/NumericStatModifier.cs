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

using System;
using MessagePack;
using Nighthollow.Data;

#nullable enable

namespace Nighthollow.Stats2
{
  public static class NumericStatModifier
  {
    public static string NumericModifierString(
      string? addTo,
      PercentageValue? increaseBy,
      string? setTo,
      string? highValue)
    {
      var high = highValue == null ? "" : $"-{highValue}";
      if (addTo != null)
      {
        return $"+{addTo}{high}";
      }
      else if (increaseBy.HasValue)
      {
        return increaseBy.Value.IsReduction()
          ? $"{increaseBy}{high} Reduced"
          : $"{increaseBy}{high} Increased";
      }

      return $"{setTo}{high}";
    }
  }

  [MessagePackObject]
  public sealed class NumericStatModifier<T> : IStatModifier where T : struct
  {
#pragma warning disable 618 // Using Obsolete
    public static NumericStatModifier<T> Add(AbstractStat<NumericStatModifier<T>, T> stat, T value) =>
      new NumericStatModifier<T>(stat.StatId, ModifierType.Add, value, null, null);

    public static NumericStatModifier<T>
      Increase(AbstractStat<NumericStatModifier<T>, T> stat, PercentageValue value) =>
      new NumericStatModifier<T>(stat.StatId, ModifierType.Increase, null, value, null);

    public static NumericStatModifier<T> Set(AbstractStat<NumericStatModifier<T>, T> stat, T value) =>
      new NumericStatModifier<T>(stat.StatId, ModifierType.Set, null, null, value);
#pragma warning restore 618

    [Obsolete("This constructor is visible only for use by the serialization system.")]
    public NumericStatModifier(
      StatId statId,
      ModifierType type,
      T? add,
      PercentageValue? increase,
      T? ovewrite)
    {
      StatId = statId;
      Type = type;
      AddTo = add;
      IncreaseBy = increase;
      SetTo = ovewrite;
    }

    [Key(0)] public StatId StatId { get; }
    [Key(1)] public ModifierType Type { get; }
    [Key(2)] public T? AddTo { get; }
    [Key(3)] public PercentageValue? IncreaseBy { get; }
    [Key(4)] public T? SetTo { get; }

    public string Describe(string template, IValueData? highValue) =>
      template.Replace("#",
        NumericStatModifier.NumericModifierString(
          AddTo?.ToString(),
          IncreaseBy,
          SetTo?.ToString(),
          highValue?.ToString()));
  }
}