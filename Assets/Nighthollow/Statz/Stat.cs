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

#nullable enable

namespace Nighthollow.Statz
{
  public static class Stat
  {
    public static readonly IntStat Health = new IntStat(1);
    public static readonly IntStat Speed = new IntStat(3);
    public static readonly PercentageStat CritChance = new PercentageStat(4);
    public static readonly PercentageStat CritMultiplier = new PercentageStat(5);

    public static IStat GetStat(int id)
    {
      return id switch
      {
        1 => Health,
        3 => Speed,
        4 => CritChance,
        5 => CritMultiplier,
        _ => throw new IndexOutOfRangeException()
      };
    }
  }
}