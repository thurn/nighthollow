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
using Nighthollow.Interface;
using Nighthollow.Services;
using Nighthollow.Triggers;
using Nighthollow.Triggers.Events;
using Nighthollow.Utils;
using Nighthollow.World.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

#nullable enable

namespace Nighthollow.World
{
  public sealed class WorldMap
  {
    const int BottomLeftZ = 1;
    const int BottomRightZ = 2;
    const int RightZ = 3;
    const int LeftZ = 4;
    const int IconZ = 5;
    static readonly HexPosition StartingHex = new HexPosition(x: -12, y: 7);
    static readonly HexPosition TutorialAttackHex = new HexPosition(x: -11, y: 7);

    readonly WorldServiceRegistry _registry;
    readonly Dictionary<int, Tilemap> _children = new Dictionary<int, Tilemap>();
    HexPosition? _currentlySelected;
    TileBase _previousIconOnSelectedTile = null!;

    public WorldMap(WorldServiceRegistry registry)
    {
      _registry = registry;
      var worldMapRoot = registry.StaticAssets.WorldTilemapContainer;
      var map = ComponentUtils.GetComponent<Tilemap>(worldMapRoot);

      // Unity is supposed to be able to handle overlapping by y coordinate correctly in a single chunked tilemap, but
      // it's currently very buggy. So I just make each row into a separate tilemap.
      for (var yCoordinate = -30; yCoordinate <= 30; ++yCoordinate)
      {
        var childMap = Object.Instantiate(worldMapRoot, registry.StaticAssets.Grid.transform);
        childMap.name = $"Row {yCoordinate}";
        var tilemap = ComponentUtils.GetComponent<Tilemap>(childMap);
        tilemap.ClearAllTiles();
        ComponentUtils.GetComponent<TilemapRenderer>(childMap).sortingOrder = -yCoordinate;

        for (var xCoordinate = -30; xCoordinate <= 30; ++xCoordinate)
        {
          tilemap.SetTile(new Vector3Int(xCoordinate, yCoordinate, z: 0),
            map.GetTile(new Vector3Int(xCoordinate, yCoordinate, z: 0)));
        }

        _children[yCoordinate] = tilemap;
      }

      map.ClearAllTiles();
      Object.Destroy(worldMapRoot.GetComponent<TilemapRenderer>());
      Object.Destroy(worldMapRoot.GetComponent<Tilemap>());

      foreach (var kingdom in registry.Database.Snapshot().Kingdoms.Values)
      {
        OutlineHexes(kingdom.Color.AsUnityColor(), new HashSet<HexPosition> {kingdom.StartingPosition});
        ShowIcon(kingdom.StartingPosition, registry.AssetService.GetTile(kingdom.TileImageAddress));
      }
    }

    public void OnUpdate()
    {
      if (Input.GetMouseButtonDown(button: 0) &&
          !_registry.ScreenController.ConsumesMousePosition(Input.mousePosition))
      {
        var ray = _registry.MainCamera.ScreenPointToRay(Input.mousePosition);
        var worldPoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
        var position = _registry.StaticAssets.Grid.WorldToCell(worldPoint);
        var hex = new HexPosition(position.x, position.y);

        ClearPreviousSelection();

        _previousIconOnSelectedTile = GetIcon(hex);
        ShowIcon(hex, _registry.StaticAssets.CreateSelectedTileIcon());
        _currentlySelected = hex;

        var tile = _children[hex.Y].GetTile(new Vector3Int(hex.X, hex.Y, z: 0));
        var screenPoint = _registry.MainCamera.WorldToScreenPoint(_registry.StaticAssets.Grid.CellToWorld(position));
        _registry.ScreenController.ShowTooltip(WorldHexTooltip.Create(
            this,
            tile.name,
            hex == StartingHex ? "Kingdom of Nighthollow" : "None",
            hex == TutorialAttackHex),
          InterfaceUtils.ScreenPointToInterfacePoint(screenPoint));
      }
    }

    public void ShowIcon(HexPosition hex, TileBase tile)
    {
      _registry.StaticAssets.OverlayTilemap.SetTile(new Vector3Int(hex.X, hex.Y, IconZ), tile);
    }

    public void RemoveIcon(HexPosition hex)
    {
      _registry.StaticAssets.OverlayTilemap.SetTile(new Vector3Int(hex.X, hex.Y, IconZ), tile: null);
    }

    public TileBase GetIcon(HexPosition hex) =>
      _registry.StaticAssets.OverlayTilemap.GetTile(new Vector3Int(hex.X, hex.Y, IconZ));

    void OutlineHexes(Color color, ISet<HexPosition> hexes)
    {
      var bottomLeftSelection = _registry.StaticAssets.CreateBottomLeftSelection();
      var bottomRightSelection = _registry.StaticAssets.CreateBottomRightSelection();
      var rightSelection = _registry.StaticAssets.CreateRightSelection();
      var leftSelection = _registry.StaticAssets.CreateLeftSelection();
      bottomLeftSelection.color = color;
      bottomRightSelection.color = color;
      rightSelection.color = color;
      leftSelection.color = color;

      foreach (var hex in hexes)
      {
        var upLeft = HexUtils.GetInDirection(hex, HexUtils.Direction.UpLeft);
        if (!hexes.Contains(upLeft))
        {
          ApplySelection(upLeft, bottomRightSelection, BottomRightZ, toOverlay: false);
        }

        var upRight = HexUtils.GetInDirection(hex, HexUtils.Direction.UpRight);
        if (!hexes.Contains(upRight))
        {
          ApplySelection(upRight, bottomLeftSelection, BottomLeftZ, toOverlay: false);
        }

        if (!hexes.Contains(HexUtils.GetInDirection(hex, HexUtils.Direction.Left)))
        {
          ApplySelection(hex, leftSelection, LeftZ, toOverlay: true);
        }

        if (!hexes.Contains(HexUtils.GetInDirection(hex, HexUtils.Direction.Right)))
        {
          ApplySelection(hex, rightSelection, RightZ, toOverlay: true);
        }

        if (!hexes.Contains(HexUtils.GetInDirection(hex, HexUtils.Direction.DownLeft)))
        {
          ApplySelection(hex, bottomLeftSelection, BottomLeftZ, toOverlay: true);
        }

        if (!hexes.Contains(HexUtils.GetInDirection(hex, HexUtils.Direction.DownRight)))
        {
          ApplySelection(hex, bottomRightSelection, BottomRightZ, toOverlay: true);
        }
      }
    }

    void ApplySelection(HexPosition hex, Tile tile, int zPosition, bool toOverlay)
    {
      if (toOverlay)
      {
        _registry.StaticAssets.OverlayTilemap.SetTile(new Vector3Int(hex.X, hex.Y, zPosition), tile);
      }
      else
      {
        _children[hex.Y].SetTile(new Vector3Int(hex.X, hex.Y, zPosition), tile);
      }
    }

    public void ClearSelection()
    {
      ClearPreviousSelection();
      _registry.ScreenController.HideTooltip();
      _currentlySelected = null;
    }

    void ClearPreviousSelection()
    {
      if (_currentlySelected != null)
      {
        RemoveIcon(_currentlySelected);
        if (_previousIconOnSelectedTile)
        {
          ShowIcon(_currentlySelected, _previousIconOnSelectedTile);
        }
      }
    }

    public void AttackHex()
    {
      _registry.ScreenController.HideTooltip();
      var triggerOutput = new TriggerOutput();
      _registry.TriggerService.Invoke(new HexAttackedEvent(_registry, 12), triggerOutput);
      if (!triggerOutput.PreventDefault)
      {
        SceneManager.LoadScene("Battle");
      }
    }
  }
}