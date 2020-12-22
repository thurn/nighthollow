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
using Nighthollow.Items;
using Nighthollow.World;

#nullable enable

namespace Nighthollow.Interface
{
  public static class WorldHexTooltip
  {
    static readonly List<(string, string)> HexTypes = new List<(string, string)>
    {
      ("necrocastle", "The Citadel"),
      ("ocean", "Ocean"),
      ("ruins", "Ruins"),

      ("village", "Village"),
      ("inn", "Inn"),
      ("smithy", "Smithy"),
      ("temple", "Temple"),
      ("city", "City"),
      ("henge", "Henge"),
      ("castle", "Castle"),
      ("farm", "Farmland"),

      ("snowfield", "Snowfield"),
      ("bog", "Swamp"),
      ("swamp", "Swamp"),

      ("desert", "Desert"),
      ("sand", "Desert"),
      ("dirt", "Plains"),
      ("scrubland", "Plains"),
      ("forest", "Forest"),
      ("ash", "Ash Plains"),
      ("lodge", "Lodge"),
      ("fumarole", "Volcanic Plains"),
      ("hills", "Hills"),
      ("lava", "Volcanic Plains"),
      ("marsh", "Swamp"),
      ("wetland", "Swamp"),
      ("woodland", "Forest"),
      ("mountain", "Mountain"),
      ("volcano", "Volcano"),
      ("plains", "Field")
    };

    public static TooltipBuilder Create(WorldMap worldMap, string hexName, string owner, bool canAttack)
    {
      var name = HexTypes
        .Where(pair => hexName.ToLowerInvariant().Contains(pair.Item1))
        .Select(pair => pair.Item2)
        .FirstOrDefault();

      var builder = new TooltipBuilder(name ?? hexName)
      {
        XOffset = 128,
        CloseButton = true,
        OnClose = worldMap.ClearSelection
      };

      builder
        .AppendText($"Area Level: 1")
        .AppendText($"Owner: {owner}");

      if (canAttack)
      {
        builder.AppendButton("Attack!", worldMap.AttackHex);
      }

      return builder;
    }
  }
}
