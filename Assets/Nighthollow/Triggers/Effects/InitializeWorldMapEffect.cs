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
using MessagePack;
using Nighthollow.Data;
using Nighthollow.Utils;
using Nighthollow.World.Data;
using UnityEngine;

#nullable enable

namespace Nighthollow.Triggers.Effects
{
  /// <summary>
  /// Debug command -- imports tilemap data into the database, updating tiles to match their tilemap &
  /// initial game states.
  /// </summary>
  [MessagePackObject]
  public sealed class InitializeWorldMapEffect : IEffect<WorldEvent>
  {
    public static Description Describe => new Description("initialize the world map");

    public void Execute(WorldEvent trigger, TriggerOutput? output)
    {
      var database = trigger.Registry.Database;
      var hexLookup = new Dictionary<HexPosition, int>();
      foreach (var pair in database.Snapshot().Hexes)
      {
        hexLookup[pair.Value.Position] = pair.Key;
        Errors.CheckState(pair.Value.HexType != HexType.Unknown, "unknown hex type");
      }

      var tiles = trigger.Registry.WorldMapRenderer.AllTiles().ToList();
      Debug.Log($"Found {hexLookup.Count} hexes, updating from {tiles.Count} tiles.");

      foreach (var (tile, position) in tiles)
      {
        var hexType = HexTypeUtil.HexTypeForTileName(tile.name);
        if (hexLookup.ContainsKey(position))
        {
          database.Update(TableId.Hexes, hexLookup[position], hex => hex.WithHexType(hexType));
        }
        else
        {
          database.Insert(TableId.Hexes, new HexData(hexType, position));
        }
      }

      foreach (var pair in database.Snapshot().Kingdoms)
      {
        database.Update(TableId.Hexes, hexLookup[pair.Value.StartingPosition], hex => hex.WithOwningKingdom(pair.Key));
      }
    }
  }
}