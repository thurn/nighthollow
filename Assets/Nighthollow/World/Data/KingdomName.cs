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

namespace Nighthollow.World.Data
{
  public enum KingdomName
  {
    Unknown = 0,
    Nighthollow = 1,
    Ivywood = 2,
    Frostford = 3,
    Uldreyin = 4,
    Dawnhaven = 5,
    Istu = 6
  }

  public static class KingdomNameUtil
  {
    public static string GetName(this KingdomName name) => name switch
    {
      KingdomName.Nighthollow => "Cursed Kingdom of Nighthollow",
      KingdomName.Ivywood => "Ivywood",
      KingdomName.Frostford => "Frostford",
      KingdomName.Uldreyin => "Uldreyin",
      KingdomName.Dawnhaven => "Dawnhaven",
      KingdomName.Istu => "Istu",
      _ => throw new ArgumentOutOfRangeException(nameof(name), name, null)
    };
  }
}