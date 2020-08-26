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
using UnityEngine;

namespace Nighthollow.Data
{
  public enum DamageType
  {
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

    public int Total()
    {
      var result = 0;

      foreach (DamageType damageType in Enum.GetValues(typeof(DamageType)))
      {
        result += Get(damageType).Value;
      }

      return result;
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
  }
}