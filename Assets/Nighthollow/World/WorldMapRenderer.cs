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
using Nighthollow.Interface.Components.Windows;
using Nighthollow.Services;
using Nighthollow.Utils;
using Nighthollow.World.Data;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable

namespace Nighthollow.World
{
  public sealed class WorldMapRenderer
  {
    const int MinX = -30;
    const int MaxX = 30;
    const int MinY = -30;
    const int MaxY = 30;
    const int BottomLeftZ = 1;
    const int BottomRightZ = 2;
    const int RightZ = 3;
    const int LeftZ = 4;
    const int IconZ = 5;

    readonly Dictionary<int, Tilemap> _children = new Dictionary<int, Tilemap>();
    HexPosition? _currentlySelected;
    TileBase _previousIconOnSelectedTile = null!;
    readonly WorldServiceRegistry _registry;
    readonly ScreenController _screenController;
    readonly WorldStaticAssets _assets;

    public WorldMapRenderer(WorldServiceRegistry registry)
    {
      _registry = registry;
      _screenController = registry.ScreenController;
      _assets = registry.StaticAssets;

      var worldMapRoot = _assets.WorldTilemapContainer;
      var map = ComponentUtils.GetComponent<Tilemap>(worldMapRoot);

      // Unity is supposed to be able to handle overlapping by y coordinate correctly in a single chunked tilemap, but
      // it's currently very buggy. So I just make each row into a separate tilemap.
      for (var yCoordinate = MinY; yCoordinate <= MaxY; ++yCoordinate)
      {
        var childMap = Object.Instantiate(worldMapRoot, _assets.Grid.transform);
        childMap.name = $"Row {yCoordinate}";
        var tilemap = ComponentUtils.GetComponent<Tilemap>(childMap);
        tilemap.ClearAllTiles();
        ComponentUtils.GetComponent<TilemapRenderer>(childMap).sortingOrder = -yCoordinate;

        for (var xCoordinate = MinX; xCoordinate <= MaxX; ++xCoordinate)
        {
          tilemap.SetTile(new Vector3Int(xCoordinate, yCoordinate, z: 0),
            map.GetTile(new Vector3Int(xCoordinate, yCoordinate, z: 0)));
        }

        _children[yCoordinate] = tilemap;
      }

      map.ClearAllTiles();
      Object.Destroy(worldMapRoot.GetComponent<TilemapRenderer>());
      Object.Destroy(worldMapRoot.GetComponent<Tilemap>());
    }

    public void OnUpdate()
    {
      if (Input.GetMouseButtonDown(button: 0) &&
          !_screenController.ConsumesMousePosition(Input.mousePosition))
      {
        var ray = _registry.MainCamera.ScreenPointToRay(Input.mousePosition);
        var worldPoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
        var position = _assets.Grid.WorldToCell(worldPoint);
        var hex = new HexPosition(position.x, position.y);

        ClearPreviousSelection();

        _previousIconOnSelectedTile = GetIcon(hex);
        ShowIcon(hex, _assets.CreateSelectedTileIcon());
        _currentlySelected = hex;

        // var screenPosition = _registry.MainCamera.WorldToScreenPoint(_assets.Grid.CellToWorld(position));
        // _registry.WorldMapController.OnHexSelected(hex, screenPosition, ClearSelection);

        _registry.WorldMapController.OnHexSelected2(hex, GetTile(hex).sprite);
      }
    }

    public Vector3 GetWorldPosition(HexPosition hexPosition) =>
      _assets.Grid.CellToWorld(new Vector3Int(hexPosition.X, hexPosition.Y, 0));

    public void ShowIcon(HexPosition hex, TileBase tile)
    {
      _assets.OverlayTilemap.SetTile(new Vector3Int(hex.X, hex.Y, IconZ), tile);
    }

    public void RemoveIcon(HexPosition hex)
    {
      _assets.OverlayTilemap.SetTile(new Vector3Int(hex.X, hex.Y, IconZ), tile: null);
    }

    public IEnumerable<(TileBase, HexPosition)> AllTiles()
    {
      for (var yCoordinate = MinY; yCoordinate <= MaxY; ++yCoordinate)
      {
        for (var xCoordinate = MinX; xCoordinate <= MaxX; ++xCoordinate)
        {
          var position = new HexPosition(xCoordinate, yCoordinate);
          yield return (GetTile(position), position);
        }
      }
    }

    public TileBase GetIcon(HexPosition hex) =>
      _assets.OverlayTilemap.GetTile(new Vector3Int(hex.X, hex.Y, IconZ));

    public Tile GetTile(HexPosition hex) =>
      (Tile) _children[hex.Y].GetTile(new Vector3Int(hex.X, hex.Y, z: 0));

    public void OutlineHexes(Color color, ISet<HexPosition> hexes)
    {
      var bottomLeftSelection = _assets.CreateBottomLeftSelection();
      var bottomRightSelection = _assets.CreateBottomRightSelection();
      var rightSelection = _assets.CreateRightSelection();
      var leftSelection = _assets.CreateLeftSelection();
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
        _assets.OverlayTilemap.SetTile(new Vector3Int(hex.X, hex.Y, zPosition), tile);
      }
      else
      {
        _children[hex.Y].SetTile(new Vector3Int(hex.X, hex.Y, zPosition), tile);
      }
    }

    public void ClearSelection()
    {
      ClearPreviousSelection();
      _screenController.Get(ScreenController.Tooltip).Hide();
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
  }
}