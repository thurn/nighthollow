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
  public enum School
  {
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
      foreach (School school in Enum.GetValues(typeof(School)))
      {
        if (Get(school).Value > other.Get(school).Value)
        {
          return false;
        }
      }

      return true;
    }
  }
}