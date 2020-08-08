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
  public class Influence
  {
    public Stat Light;
    public Stat Sky;
    public Stat Flame;
    public Stat Ice;
    public Stat Earth;
    public Stat Shadow;

    public Stat Get(School school) => GetRef(school);

    ref Stat GetRef(School school)
    {
      switch (school)
      {
        case School.Light:
          return ref Light;
        case School.Sky:
          return ref Sky;
        case School.Flame:
          return ref Flame;
        case School.Ice:
          return ref Ice;
        case School.Earth:
          return ref Earth;
        case School.Shadow:
          return ref Shadow;
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