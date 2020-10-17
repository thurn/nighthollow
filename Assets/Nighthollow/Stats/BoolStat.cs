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
    readonly List<IModifier> _setTrueModifiers = new List<IModifier>();
    readonly List<IModifier> _setFalseModifiers = new List<IModifier>();
    bool _hasDynamicModifiers;
    bool _computedValue;

    public BoolStat()
    {
      _computedValue = false;
    }

    BoolStat(
      IEnumerable<IModifier> setTrueModifiers,
      IEnumerable<IModifier> setFalseModifiers,
      bool hasDynamicModifiers)
    {
      _setTrueModifiers = setTrueModifiers.Select(m => m.Clone()).ToList();
      _setFalseModifiers = setFalseModifiers.Select(m => m.Clone()).ToList();
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

    public void AddSetTrueModifier(IModifier modifier)
    {
      _setTrueModifiers.Add(modifier);
      _hasDynamicModifiers |= modifier.IsDynamic();
      Recalculate();
    }

    public void AddSetFalseModifier(IModifier modifier)
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