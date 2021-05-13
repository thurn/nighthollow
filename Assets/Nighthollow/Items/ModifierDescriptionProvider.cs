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
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Items
{
  public sealed class ModifierDescriptionProvider : IStatDescriptionProvider
  {
    readonly GameData _gameData;
    readonly StatTable? _values;
    readonly StatTable? _low;
    readonly StatTable? _high;

    public ModifierDescriptionProvider(
      GameData gameData, StatTable? values = null, StatTable? low = null, StatTable? high = null)
    {
      _gameData = gameData;
      _values = values;
      _low = low;
      _high = high;
    }

    public ModifierDescriptionProvider Insert(ModifierData modifier)
    {
      var defaults = StatData.BuildDefaultStatTable(_gameData);
      var valueModifier = modifier.BuildStatModifier();
      if (valueModifier != null)
      {
        return new ModifierDescriptionProvider(
          _gameData,
          (_values ?? new StatTable(defaults)).InsertModifier(valueModifier),
          _low,
          _high
        );
      }
      else
      {
        return new ModifierDescriptionProvider(
          _gameData,
          _values,
          (_low ?? new StatTable(defaults)).InsertNullableModifier(modifier.ModifierForValue(modifier.ValueLow)),
          (_high ?? new StatTable(defaults)).InsertNullableModifier(modifier.ModifierForValue(modifier.ValueHigh))
        );
      }
    }

    public string Get<TModifier, TValue>(AbstractStat<TModifier, TValue> stat)
      where TModifier : IStatModifier where TValue : notnull
    {
      if (_values != null)
      {
        return stat.Describe(_values.Get(stat));
      }
      else
      {
        var low = _low!.Get(stat);
        var high = _high!.Get(stat);
        return Equals(low, high) ? stat.Describe(low) : $"({stat.Describe(low)} to {stat.Describe(high)})";
      }
    }
  }
}