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

    public IStat NotFoundValue() => new IntStat();
  }

  public sealed class IntStat : IStat<IntStat>, IAdditiveStat
  {
    readonly int _initialValue;
    readonly List<IModifier> _addedModifiers = new List<IModifier>();
    readonly List<IModifier> _increaseModifiers = new List<IModifier>();
    bool _hasDynamicModifiers;
    int _computedValue;

    public IntStat()
    {
    }

    IntStat(int initialValue,
      IEnumerable<IModifier> addedModifiers,
      IEnumerable<IModifier> increaseModifiers,
      bool hasDynamicModifiers)
    {
      _initialValue = initialValue;
      _addedModifiers = addedModifiers.Select(m => m.Clone()).ToList();
      _increaseModifiers = increaseModifiers.Select(m => m.Clone()).ToList();
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

    public void Add(int value) => AddAddedModifier(new StaticModifier(new IntValue(value)));

    public void AddAddedModifier(IModifier modifier)
    {
      _addedModifiers.Add(modifier);
      _hasDynamicModifiers |= modifier.IsDynamic();
      Recalculate();
    }

    public void AddIncreaseModifier(IModifier modifier)
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

      result += _addedModifiers.Sum(m => m.BaseModifier.Argument.AsIntValue().Value);

      var increaseBy = 10000 + _increaseModifiers.Sum(m => ((PercentageValue) m.BaseModifier.Argument).Value.Value);
      result = Constants.FractionBasisPoints(result, increaseBy);

      _computedValue = result;
    }

    public void AddValue(IStatValue value)
    {
      Add(((IntValue) value).Value);
    }
  }
}