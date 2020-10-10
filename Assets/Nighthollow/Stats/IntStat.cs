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

#nullable enable

using System.Collections.Generic;
using System.Linq;
using Nighthollow.Utils;

namespace Nighthollow.Stats
{
  public readonly struct IntStatId : IStatId<IntStat>
  {
    readonly int _value;

    public IntStatId(int value)
    {
      _value = value;
    }

    public int Value => _value;

    public IntStat NotFoundValue() => new IntStat(0);

    public IntStat Deserialize(string value) => new IntStat(int.Parse(value));
  }

  public sealed class IntStat : IStat<IntStat>
  {
    readonly int _initialValue;
    readonly List<IModifier<IntValue>> _addedModifiers = new List<IModifier<IntValue>>();
    readonly List<IModifier<PercentageValue>> _increaseModifiers = new List<IModifier<PercentageValue>>();
    bool _hasDynamicModifiers;
    int _computedValue;

    public IntStat() : this(0)
    {
    }

    public IntStat(int initialValue)
    {
      _initialValue = initialValue;
      _computedValue = initialValue;
    }


    IntStat(int initialValue,
      IEnumerable<IModifier<IntValue>> addedModifiers,
      IEnumerable<IModifier<PercentageValue>> increaseModifiers,
      bool hasDynamicModifiers)
    {
      _initialValue = initialValue;
      _addedModifiers = addedModifiers.Select(m => m.Clone<IntValue>()).ToList();
      _increaseModifiers = increaseModifiers.Select(m => m.Clone<PercentageValue>()).ToList();
      _hasDynamicModifiers = hasDynamicModifiers;
      Recalculate();
    }

    public int Value
    {
      get
      {
        if (_hasDynamicModifiers)
        {
          Recalculate();
        }

        return _computedValue;
      }
    }

    public void AddAddedModifier(IModifier<IntValue> modifier)
    {
      _addedModifiers.Add(modifier);
      _hasDynamicModifiers |= modifier.IsDynamic();
      Recalculate();
    }

    public void AddIncreaseModifier(IModifier<PercentageValue> modifier)
    {
      _increaseModifiers.Add(modifier);
      _hasDynamicModifiers |= modifier.IsDynamic();
      Recalculate();
    }

    public IntStat Clone() => new IntStat(_initialValue, _addedModifiers, _increaseModifiers, _hasDynamicModifiers);

    void Recalculate()
    {
      var result = _initialValue;

      _addedModifiers.RemoveAll(m => !m.IsValid());
      _increaseModifiers.RemoveAll(m => !m.IsValid());

      result += _addedModifiers.Sum(m => m.Modifier.Value.Value);

      var increaseBy = 10000 + _increaseModifiers.Sum(m => m.Modifier.Value.Value);
      result = Constants.FractionBasisPoints(result, increaseBy);

      _computedValue = result;
    }
  }
}