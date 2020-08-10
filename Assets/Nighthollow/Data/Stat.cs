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

using Nighthollow.Components;
using Nighthollow.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nighthollow.Data
{
  public enum Operator
  {
    Set,
    Add,
    Increase,
    Multiply
  }

  [Serializable]
  public class Modifier
  {
    [SerializeField] Operator _operator;
    public Operator Operator => _operator;

    [SerializeField] int _value;
    public int Value => _value;

    [SerializeField] float _endTimeSeconds;
    public float EndTimeSeconds => _endTimeSeconds;

    [SerializeField] WeakReference<Creature> _scopeCreature;
    public WeakReference<Creature> ScopeCreature => _scopeCreature;

    public static Modifier Create(Operator op, int value)
    {
      return new Modifier
      {
        _operator = op,
        _value = value
      };
    }

    public static Modifier WithDurationMs(Operator op, int value, int durationMs)
    {
      return new Modifier
      {
        _operator = op,
        _value = value,
        _endTimeSeconds = Time.time + (durationMs / 1000.0f)
      };
    }

    public static Modifier WhileAlive(Operator op, int value, Creature creature)
    {
      return new Modifier
      {
        _operator = op,
        _value = value,
        _scopeCreature = new WeakReference<Creature>(Errors.CheckNotNull(creature))
      };
    }
  }

  [Serializable]
  public class Stat
  {
    [SerializeField] int _value;
    bool _hasDynamicModifiers;
    int _cachedValue;
    Dictionary<Operator, List<Modifier>> _modifiers;
    int _lastCalculatedFrame;

    public Stat(int value)
    {
      _value = value;
      _cachedValue = value;
    }

    public int Value
    {
      get
      {
        if (_modifiers == null)
        {
          // Unity deserializer does not run constructors, need to lazily populate
          _modifiers = new Dictionary<Operator, List<Modifier>>();
          _cachedValue = _value;
        }

        if (_hasDynamicModifiers && _lastCalculatedFrame < Time.frameCount)
        {
          Recalculate();
        }

        return _cachedValue;
      }
    }

    public void AddModifier(Modifier modifier)
    {
      Errors.CheckNotNull(modifier);
      if (_modifiers == null)
      {
        _modifiers = new Dictionary<Operator, List<Modifier>>();
      }

      if (!_modifiers.ContainsKey(modifier.Operator))
      {
        _modifiers[modifier.Operator] = new List<Modifier>();
      }

      _modifiers[modifier.Operator].Add(modifier);

      if (Math.Abs(modifier.EndTimeSeconds) > 0.0001f || modifier.ScopeCreature != null)
      {
        _hasDynamicModifiers = true;
      }

      Recalculate();
    }

    void Recalculate()
    {
      var result = _value;

      if (_modifiers.ContainsKey(Operator.Set))
      {
        var setValues = _modifiers[Operator.Set];
        var foundSetter = false;
        for (var i = setValues.Count - 1; i >= 0; --i)
        {
          if (!ActiveModifier(setValues[i]))
          {
            setValues.RemoveAt(i);
          }
          else if (!foundSetter)
          {
            result = setValues[i].Value;
            foundSetter = true;
          }
        }
      }

      if (_modifiers.ContainsKey(Operator.Add))
      {
        var addValues = _modifiers[Operator.Add];
        for (var i = addValues.Count - 1; i >= 0; --i)
        {
          if (!ActiveModifier(addValues[i]))
          {
            addValues.RemoveAt(i);
          }
          else
          {
            result += addValues[i].Value;
          }
        }
      }

      if (_modifiers.ContainsKey(Operator.Increase))
      {
        var increaseValues = _modifiers[Operator.Increase];
        var increaseBy = 100;
        for (var i = increaseValues.Count - 1; i >= 0; --i)
        {
          if (!ActiveModifier(increaseValues[i]))
          {
            increaseValues.RemoveAt(i);
          }
          else
          {
            increaseBy += increaseValues[i].Value;
          }
        }

        result = Mathf.RoundToInt((result * increaseBy) / 100f);
      }

      if (_modifiers.ContainsKey(Operator.Multiply))
      {
        var multiplyValues = _modifiers[Operator.Multiply];
        for (var i = multiplyValues.Count - 1; i >= 0; --i)
        {
          if (!ActiveModifier(multiplyValues[i]))
          {
            multiplyValues.RemoveAt(i);
          }
          else
          {
            result = Mathf.RoundToInt((result * (100 + multiplyValues[i].Value)) / 100f);
          }
        }
      }

      _lastCalculatedFrame = Time.frameCount;
      _cachedValue = result;
    }

    bool ActiveModifier(Modifier modifier)
    {
      if (Math.Abs(modifier.EndTimeSeconds) > 0.0001f)
      {
        return modifier.EndTimeSeconds > Time.time;
      }

      if (modifier.ScopeCreature != null)
      {
        modifier.ScopeCreature.TryGetTarget(out var creature);
        return creature && creature.IsAlive();
      }

      return true;
    }
  }
}