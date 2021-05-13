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

using System.Collections.Generic;
using System.Linq;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.World.Data
{
  public enum HexType
  {
    Unknown = 0,
    TheCitadel = 1,
    Ocean = 2,
    Ruins = 3,
    Village = 4,
    Inn = 5,
    Smithy = 6,
    Temple = 7,
    City = 8,
    Henge = 9,
    Castle = 10,
    Farmland = 11,
    Snowfield = 12,
    Swamp = 13,
    Desert = 14,
    Plains = 15,
    Forest = 16,
    AshPlains = 17,
    Lodge = 18,
    VolcanicPlains = 19,
    Hills = 20,
    Mountain = 21,
    Volcano = 22,
    Field = 23
  }

  public static class HexTypeUtil
  {
    static readonly List<(string, HexType)> TypesMap = new List<(string, HexType)>
    {
      ("necrocastle", HexType.TheCitadel),
      ("ocean", HexType.Ocean),
      ("ruins", HexType.Ruins),
      ("village", HexType.Village),
      ("inn", HexType.Inn),
      ("smithy", HexType.Smithy),
      ("temple", HexType.Temple),
      ("city", HexType.City),
      ("henge", HexType.Henge),
      ("castle", HexType.Castle),
      ("farm", HexType.Farmland),
      ("snowfield", HexType.Snowfield),
      ("bog", HexType.Swamp),
      ("swamp", HexType.Swamp),
      ("desert", HexType.Desert),
      ("sand", HexType.Desert),
      ("dirt", HexType.Plains),
      ("scrubland", HexType.Plains),
      ("forest", HexType.Forest),
      ("ash", HexType.AshPlains),
      ("lodge", HexType.Lodge),
      ("fumarole", HexType.VolcanicPlains),
      ("hills", HexType.Hills),
      ("lava", HexType.VolcanicPlains),
      ("marsh", HexType.Swamp),
      ("wetland", HexType.Swamp),
      ("woodland", HexType.Forest),
      ("mountain", HexType.Mountain),
      ("volcano", HexType.Volcano),
      ("plains", HexType.Field)
    };

    public static HexType HexTypeForTileName(string tileName)
    {
      var result = TypesMap
        .Where(pair => tileName.ToLowerInvariant().Contains(pair.Item1))
        .Select(pair => pair.Item2)
        .FirstOrDefault();
      Errors.CheckState(result != HexType.Unknown, "Unrecognized tile name");
      return result;
    }
  }
}