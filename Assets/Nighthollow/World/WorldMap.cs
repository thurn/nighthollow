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

#nullable enable

using System.Collections.Generic;
using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Nighthollow.World
{
  public sealed class WorldMap : MonoBehaviour
  {
    [SerializeField] Tile _bottomLeftSelection = null!;
    [SerializeField] Tile _bottomRightSelection = null!;
    [SerializeField] Tile _rightSelection = null!;
    [SerializeField] Tile _leftSelection = null!;
    [SerializeField] Tilemap _overlayTilemap = null!;

    const int BottomLeftZ = 1;
    const int BottomRightZ = 2;
    const int RightZ = 3;
    const int LeftZ = 4;
    readonly Dictionary<int, Tilemap> _children = new Dictionary<int, Tilemap>();

    void Start()
    {
      var map = ComponentUtils.GetComponent<Tilemap>(gameObject);

      // Unity is supposed to be able to handle overlapping by y coordinate correctly in a single chunked tilemap, but
      // it's currently very buggy. So I just make each row in to a separate tilemap.
      for (var yCoordinate = -30; yCoordinate <= 30; ++yCoordinate)
      {
        var childMap = Instantiate(this, transform.parent);
        Destroy(childMap.GetComponent<WorldMap>());
        childMap.name = $"Row {yCoordinate}";
        var tilemap = ComponentUtils.GetComponent<Tilemap>(childMap);
        tilemap.ClearAllTiles();
        ComponentUtils.GetComponent<TilemapRenderer>(childMap).sortingOrder = -yCoordinate;

        for (var xCoordinate = -30; xCoordinate <= 30; ++xCoordinate)
        {
          tilemap.SetTile(new Vector3Int(xCoordinate, yCoordinate, 0),
            map.GetTile(new Vector3Int(xCoordinate, yCoordinate, 0)));
        }

        _children[yCoordinate] = tilemap;
      }

      map.ClearAllTiles();
      Destroy(GetComponent<TilemapRenderer>());
      Destroy(GetComponent<Tilemap>());

      OutlineHexes(new Color(0.776f, 0.157f, 0.157f), new HashSet<Vector2Int>
      {
        new Vector2Int(-12, 7),
        new Vector2Int(-11, 7),
        new Vector2Int(-10, 7),
        new Vector2Int(-12, 6),
        new Vector2Int(-11, 6),
      });
    }

    public void OutlineHexes(Color color, ISet<Vector2Int> hexes)
    {
      var bottomLeftSelection = Instantiate(_bottomLeftSelection);
      var bottomRightSelection = Instantiate(_bottomRightSelection);
      var rightSelection = Instantiate(_rightSelection);
      var leftSelection = Instantiate(_leftSelection);
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

    void ApplySelection(Vector2Int hex, Tile tile, int zPosition, bool toOverlay)
    {
      if (toOverlay)
      {
        _overlayTilemap.SetTile(new Vector3Int(hex.x, hex.y, zPosition), tile);
      }
      else
      {
        _children[hex.y].SetTile(new Vector3Int(hex.x, hex.y, zPosition), tile);
      }
    }

    // void Start()
    // {
    //   _tilemap = GetComponent<Tilemap>();
    //   _tilemapRenderer = GetComponent<TilemapRenderer>();
    //
    //   var color = new Color(0.776f, 0.157f, 0.157f);
    //   _bottomLeftSelection.color = color;
    //   _bottomRightSelection.color = color;
    //   _rightSelection.color = color;
    //   _leftSelection.color = color;
    //
    //   if (!IsChild)
    //   {
    //     // Unity is supposed to be able to handle overlapping by y coordinate correctly in a single tilemap, but it's
    //     // currently very buggy. So I just make each row in to a separate tilemap.
    //     for (var yCoordinate = -30; yCoordinate <= 30; ++yCoordinate)
    //     {
    //       var childMap = ComponentUtils.GetComponent<WorldMap>(Instantiate(gameObject, transform.parent));
    //       childMap.name = $"Row {yCoordinate}";
    //       childMap.IsChild = true;
    //       childMap.TilemapRenderer.sortingOrder = -yCoordinate;
    //       childMap.Tilemap.ClearAllTiles();
    //
    //       for (var xCoordinate = -30; xCoordinate <= 30; ++xCoordinate)
    //       {
    //         childMap.Tilemap.SetTile(new Vector3Int(xCoordinate, yCoordinate, 0),
    //           _tilemap.GetTile(new Vector3Int(xCoordinate, yCoordinate, 0)));
    //       }
    //
    //       if (yCoordinate == 8)
    //       {
    //         childMap.Tilemap.SetTile(new Vector3Int(-12, 8, 1), Instantiate(_bottomRightSelection));
    //         childMap.Tilemap.SetTile(new Vector3Int(-11, 8, 2), Instantiate(_bottomLeftSelection));
    //         childMap.Tilemap.SetTile(new Vector3Int(-11, 8, 3), Instantiate(_bottomRightSelection));
    //         childMap.Tilemap.SetTile(new Vector3Int(-10, 8, 3), Instantiate(_bottomLeftSelection));
    //       }
    //
    //       if (yCoordinate == 7)
    //       {
    //         _overlayTilemap.SetTile(new Vector3Int(-12, 7, 2), _bottomRightSelection);
    //         _overlayTilemap.SetTile(new Vector3Int(-12, 7, 3), _bottomLeftSelection);
    //         _overlayTilemap.SetTile(new Vector3Int(-12, 7, 4), _leftSelection);
    //
    //         _overlayTilemap.SetTile(new Vector3Int(-11, 7, 2), _bottomRightSelection);
    //         _overlayTilemap.SetTile(new Vector3Int(-11, 7, 3), _bottomLeftSelection);
    //         _overlayTilemap.SetTile(new Vector3Int(-11, 7, 4), _rightSelection);
    //
    //       }
    //     }
    //
    //     Destroy(gameObject);
    //   }
    // }
  }
}