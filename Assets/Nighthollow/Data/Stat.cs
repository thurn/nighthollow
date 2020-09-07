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
using System.Linq;
using UnityEngine;

namespace Nighthollow.Data
{
  public enum Operator
  {
    Unknown,
    Set,
    Add,
    Increase,
    Multiply
  }

  [Serializable]
  public sealed class Modifier
  {
    [SerializeField] Operator _operator;
    public Operator Operator => _operator;

    [SerializeField] int _value;
    public int Value => _value;

    float _endTimeSeconds;
    public float EndTimeSeconds => _endTimeSeconds;

    WeakReference<Creature> _scopeCreature;
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

  /// <summary>
  /// Represents an integer value which can have its value changed by operations called Modifiers.
  /// </summary>
  [Serializable]
  public sealed class Stat
  {
    [SerializeField] int _value;
    [SerializeField] List<Modifier> _modifiers;

    /// <summary>
    /// The maximum value randomly added to this stat when "Roll" is called. The Roll() method returns the value of a
    /// Stat modified by adding an integer between 0 and _range (inclusive), selected uniformly at random.
    /// </summary>
    [SerializeField] int _range;

    bool _hasBeenRead;
    bool _hasDynamicModifiers;
    int _cachedValue;
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
        if (!_hasBeenRead || (_hasDynamicModifiers && _lastCalculatedFrame < Time.frameCount))
        {
          Recalculate();
          _hasBeenRead = true;
        }

        return _cachedValue;
      }
    }

    public void AddModifier(Modifier modifier)
    {
      Errors.CheckNotNull(modifier);

      _modifiers.Add(modifier);

      if (Math.Abs(modifier.EndTimeSeconds) > 0.0001f || modifier.ScopeCreature != null)
      {
        _hasDynamicModifiers = true;
      }

      Recalculate();
    }

    public override string ToString()
    {
      return _cachedValue != _value ? $"{_cachedValue} ({_value})" : _value.ToString();
    }

    void Recalculate()
    {
      if (_modifiers == null)
      {
        return;
      }

      var result = _value;

      _modifiers.RemoveAll(InactiveModifier);

      result = _modifiers.FirstOrDefault(m => m.Operator == Operator.Set)?.Value ?? result;

      result += _modifiers.Where(m => m.Operator == Operator.Add).Sum(modifier => modifier.Value);

      var increaseBy = 100 + _modifiers.Where(m => m.Operator == Operator.Increase).Sum(modifier => modifier.Value);
      result = Mathf.RoundToInt((result * increaseBy) / 100f);

      result = _modifiers
        .Where(m => m.Operator == Operator.Multiply)
        .Aggregate(result, (current, modifier) => Mathf.RoundToInt((current * (100 + modifier.Value)) / 100f));

      _lastCalculatedFrame = Time.frameCount;
      _cachedValue = result;
    }

    bool InactiveModifier(Modifier modifier)
    {
      if (Math.Abs(modifier.EndTimeSeconds) > 0.0001f)
      {
        return Time.time > modifier.EndTimeSeconds;
      }

      if (modifier.ScopeCreature != null)
      {
        modifier.ScopeCreature.TryGetTarget(out var creature);
        return !(creature && creature.IsAlive());
      }

      return false;
    }
  }
}