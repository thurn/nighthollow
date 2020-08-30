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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Nighthollow.Data
{
  public enum DamageType
  {
    Unknown,
    Radiant,
    Lightning,
    Fire,
    Cold,
    Physical,
    Necrotic
  }

  [Serializable]
  public sealed class Damage
  {
    [SerializeField] Stat _radiant;
    public Stat Radiant => _radiant;

    [SerializeField] Stat _lightning;
    public Stat Lightning => _lightning;

    [SerializeField] Stat _fire;
    public Stat Fire => _fire;

    [SerializeField] Stat _cold;
    public Stat Cold => _cold;

    [SerializeField] Stat _physical;
    public Stat Physical => _physical;

    [SerializeField] Stat _necrotic;
    public Stat Necrotic => _necrotic;

    public static readonly IEnumerable<DamageType> AllTypes = new List<DamageType>
    {
      DamageType.Radiant,
      DamageType.Lightning,
      DamageType.Fire,
      DamageType.Cold,
      DamageType.Physical,
      DamageType.Necrotic
    };

    public int Total(Damage resistances)
    {
      return Mathf.RoundToInt(AllTypes.Sum(damageType =>
      {
        var damage = Get(damageType).Value;
        if (damage == 0)
        {
          return 0;
        }

        var resistance = (float) resistances.Get(damageType).Value;
        var reduction = resistance / (resistance + (2.0f * damage));
        return Mathf.Clamp(1f - reduction, 0.25f, 1.0f) * damage;
      }));
    }

    public Stat Get(DamageType type)
    {
      switch (type)
      {
        case DamageType.Radiant:
          return Radiant;
        case DamageType.Lightning:
          return Lightning;
        case DamageType.Fire:
          return Fire;
        case DamageType.Cold:
          return Cold;
        case DamageType.Physical:
          return Physical;
        case DamageType.Necrotic:
          return Necrotic;
        default:
          throw new ArgumentOutOfRangeException(nameof(type), type.ToString());
      }
    }

    public override string ToString()
    {
      var builder = new StringBuilder("[ ");
      foreach (var type in AllTypes.Where(type => Get(type).Value > 0))
      {
        builder.Append($"{type}: {Get(type)} ");
      }

      builder.Append("]");
      return builder.ToString();
    }
  }
}