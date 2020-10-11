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

#nullable enable

namespace Nighthollow.Stats
{
  public readonly struct BoolStatId : IStatId<BoolStat>
  {
    readonly int _value;

    public BoolStatId(int value)
    {
      _value = value;
    }

    public int Value => _value;

    public IStat NotFoundValue() => new BoolStat();
  }

  public sealed class BoolStat : IStat<BoolStat>
  {
    readonly List<IModifier<NoValue>> _setTrueModifiers = new List<IModifier<NoValue>>();
    readonly List<IModifier<NoValue>> _setFalseModifiers = new List<IModifier<NoValue>>();
    bool _hasDynamicModifiers;
    bool _computedValue;

    public BoolStat()
    {
      _computedValue = false;
    }

    BoolStat(
      IEnumerable<IModifier<NoValue>> setTrueModifiers,
      IEnumerable<IModifier<NoValue>> setFalseModifiers,
      bool hasDynamicModifiers)
    {
      _setTrueModifiers = setTrueModifiers.Select(m => m.Clone<NoValue>()).ToList();
      _setFalseModifiers = setFalseModifiers.Select(m => m.Clone<NoValue>()).ToList();
      _hasDynamicModifiers = hasDynamicModifiers;
    }

    public BoolStat Clone() => new BoolStat(_setTrueModifiers, _setFalseModifiers, _hasDynamicModifiers);

    public bool Value
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

    public void AddSetTrueModifier(IModifier<NoValue> modifier)
    {
      _setTrueModifiers.Add(modifier);
      _hasDynamicModifiers |= modifier.IsDynamic();
      Recalculate();
    }

    public void AddSetFalseModifier(IModifier<NoValue> modifier)
    {
      _setFalseModifiers.Add(modifier);
      _hasDynamicModifiers |= modifier.IsDynamic();
      Recalculate();
    }

    void Recalculate()
    {
      _computedValue = _setTrueModifiers.Count > 0 && _setFalseModifiers.Count == 0;
    }
  }
}