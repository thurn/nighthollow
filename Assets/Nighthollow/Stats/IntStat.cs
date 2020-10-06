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

using System.Collections.Generic;
using System.Linq;
using Nighthollow.Generated;
using Nighthollow.Utils;

namespace Nighthollow.Stats
{
  public readonly struct IntStatId : IStatId<IntStat>
  {
    readonly uint _value;

    public IntStatId(uint value)
    {
      _value = value;
    }

    public uint Value => _value;

    public IntStat NotFoundValue() => new IntStat(0);

    public IntStat Deserialize(string value) => new IntStat(int.Parse(value));
  }

  public sealed class IntStat : IStat<IntStat>
  {
    readonly int _initialValue;
    readonly Dictionary<Operator, List<IModifier>> _modifiers = new Dictionary<Operator, List<IModifier>>();
    bool _hasDynamicModifiers;
    int _computedValue;

    public IntStat()
    {
    }

    public IntStat(int initialValue)
    {
      _initialValue = initialValue;
      _computedValue = initialValue;
      _modifiers[Operator.Add] = new List<IModifier>();
      _modifiers[Operator.Increase] = new List<IModifier>();
    }

    IntStat(int initialValue, Dictionary<Operator, List<IModifier>> modifiers, bool hasDynamicModifiers)
    {
      _initialValue = initialValue;
      _modifiers = modifiers.ToDictionary(e => e.Key, e => e.Value.Select(m => m.Clone()).ToList());
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

    public void AddModifier(IModifier modifier)
    {
      _modifiers[modifier.Modifier.Operator].Add(modifier);
      _hasDynamicModifiers |= modifier.IsDynamic();
      Recalculate();
    }

    public IntStat Clone() => new IntStat(_initialValue, _modifiers, _hasDynamicModifiers);

    void Recalculate()
    {
      var result = _initialValue;

      foreach (var list in _modifiers.Values)
      {
        list.RemoveAll(m => !m.IsValid());
      }

      result += _modifiers[Operator.Add].Sum(m => m.Modifier.Value);

      var increaseBy = 10000 + _modifiers[Operator.Increase].Sum(m => m.Modifier.Value);
      result = Constants.FractionBasisPoints(result, increaseBy);

      _computedValue = result;
    }
  }
}