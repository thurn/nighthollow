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
  public enum School
  {
    Unknown,
    Light,
    Sky,
    Flame,
    Ice,
    Earth,
    Shadow
  }

  [Serializable]
  public sealed class Influence
  {
    [SerializeField] Stat _light;
    public Stat Light => _light;

    [SerializeField] Stat _sky;
    public Stat Sky => _sky;

    [SerializeField] Stat _flame;
    public Stat Flame => _flame;

    [SerializeField] Stat _ice;
    public Stat Ice => _ice;

    [SerializeField] Stat _earth;
    public Stat Earth => _earth;

    [SerializeField] Stat _shadow;
    public Stat Shadow => _shadow;

    public static readonly IEnumerable<School> AllSchools = new List<School>
    {
      School.Light,
      School.Sky,
      School.Flame,
      School.Ice,
      School.Earth,
      School.Shadow
    };

    public Stat Get(School school)
    {
      switch (school)
      {
        case School.Light:
          return Light;
        case School.Sky:
          return Sky;
        case School.Flame:
          return Flame;
        case School.Ice:
          return Ice;
        case School.Earth:
          return Earth;
        case School.Shadow:
          return Shadow;
        default:
            throw new ArgumentOutOfRangeException(nameof(school), school.ToString());
      }
    }

    public bool LessThanOrEqual(Influence other)
    {
      return AllSchools.All(school => Get(school).Value <= other.Get(school).Value);
    }

    public void Merge(Influence other)
    {
      foreach (var type in AllSchools)
      {
        Get(type).Merge(other.Get(type));
      }
    }

    public override string ToString()
    {
      var builder = new StringBuilder("[ ");
      foreach (var school in AllSchools.Where(school => Get(school).Value > 0))
      {
        builder.Append($"{school}: {Get(school).Value} ");
      }

      builder.Append("]");
      return builder.ToString();
    }
  }
}