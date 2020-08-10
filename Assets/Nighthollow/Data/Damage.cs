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
  public class Damage
  {
    public Stat Radiant;
    public Stat Lightning;
    public Stat Fire;
    public Stat Cold;
    public Stat Physical;
    public Stat Necrotic;

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