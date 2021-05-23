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
using System.Collections.Immutable;
using System.Linq;
using Nighthollow.Data;
using Nighthollow.Interface;
using Nighthollow.Items;
using Nighthollow.Services;
using Nighthollow.Triggers;
using Nighthollow.Triggers.Events;
using Nighthollow.World.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable

namespace Nighthollow.World
{
  public sealed class WorldMapController
  {
    readonly WorldServiceRegistry _registry;
    readonly ImmutableDictionary<HexPosition, int> _hexIndex;
    readonly int _nighthollowKingdomId;
    ImmutableDictionary<int, KingdomData>? _lastRendered;

    public WorldMapController(WorldServiceRegistry registry)
    {
      _registry = registry;

      var gameData = _registry.Database.Snapshot();
      var hexIndex = ImmutableDictionary.CreateBuilder<HexPosition, int>();
      foreach (var pair in gameData.Hexes)
      {
        hexIndex[pair.Value.Position] = pair.Key;
      }

      _nighthollowKingdomId = gameData.Kingdoms.First(pair => pair.Value.Name == KingdomName.Nighthollow).Key;
      _hexIndex = hexIndex.ToImmutable();
    }

    public void OnUpdate()
    {
      var data = _registry.Database.Snapshot();
      if (!ReferenceEquals(data.Kingdoms, _lastRendered))
      {
        // TODO: Clear previous outline/icons

        foreach (var kingdom in data.Kingdoms.Values)
        {
          _registry.WorldMapRenderer.OutlineHexes(
            kingdom.Color.AsUnityColor(),
            new HashSet<HexPosition> {kingdom.StartingPosition});
          _registry.WorldMapRenderer.ShowIcon(kingdom.StartingPosition,
            _registry.AssetService.GetTile(kingdom.TileImageAddress));
        }

        _lastRendered = data.Kingdoms;
      }
    }

    public void OnHexSelected(HexPosition hexPosition, Vector3 screenPosition, Action onComplete)
    {
      var gameData = _registry.Database.Snapshot();
      var hexData = gameData.Hexes[_hexIndex[hexPosition]];
      var builder = new TooltipBuilder(
        hexData.HexType.GetName(),
        InterfaceUtils.ScreenPointToInterfacePoint(screenPosition))
      {
        XOffset = 128,
        CloseButton = true,
        OnHide = onComplete
      };

      if (hexData.HexType == HexType.Ocean)
      {
        builder
          .AppendText($"Impassable Terrain");
      }
      else
      {
        var owner = hexData.OwningKingdom.HasValue
          ? gameData.Kingdoms[hexData.OwningKingdom.Value].Name.GetName()
          : "None";

        builder
          .AppendText($"Area Level: 1")
          .AppendText($"Owner: {owner}");
      }

      if (CanAttack(hexPosition, hexData, gameData))
      {
        builder.AppendButton("Attack!", AttackHex);
      }

      _registry.ScreenController.Get(ScreenController.Tooltip).Show(builder);
    }

    bool CanAttack(HexPosition hexPosition, HexData hexData, GameData gameData)
    {
      return hexData.OwningKingdom != _nighthollowKingdomId &&
             hexData.HexType != HexType.Ocean &&
             gameData.Hexes.Values.Any(hex =>
               hex.OwningKingdom == _nighthollowKingdomId && HexUtils.AreAdjacent(hex.Position, hexPosition));
    }

    public void AttackHex()
    {
      _registry.ScreenController.Get(ScreenController.Tooltip).Hide();
      var triggerOutput = new TriggerOutput();
      _registry.TriggerService.Invoke<HexAttackedEvent>(_registry.Scope, triggerOutput);
      if (!triggerOutput.PreventDefault)
      {
        SceneManager.LoadScene("Battle");
      }
    }
  }
}