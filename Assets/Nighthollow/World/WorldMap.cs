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

using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Nighthollow.World
{
  public sealed class WorldMap : MonoBehaviour
  {
    [SerializeField] Tilemap _overlayTilemap = null!;
    public Tilemap OverlayTilemap => _overlayTilemap;

    [SerializeField] Tilemap _tilemap = null!;
    public Tilemap Tilemap => _tilemap;

    [SerializeField] TilemapRenderer _tilemapRenderer = null!;
    public TilemapRenderer TilemapRenderer => _tilemapRenderer;

    [SerializeField] Tile _bottomLeftSelection = null!;
    [SerializeField] Tile _bottomRightSelection = null!;
    [SerializeField] Tile _rightSelection = null!;
    [SerializeField] Tile _leftSelection = null!;

    public bool IsChild { get; set; }

    void Start()
    {
      _tilemap = GetComponent<Tilemap>();
      _tilemapRenderer = GetComponent<TilemapRenderer>();

      var color = new Color(0.776f, 0.157f, 0.157f);
      _bottomLeftSelection.color = color;
      _bottomRightSelection.color = color;
      _rightSelection.color = color;
      _leftSelection.color = color;

      if (!IsChild)
      {
        // Unity is supposed to be able to handle overlapping by y coordinate correctly in a single tilemap, but it's
        // currently very buggy. So I just make each row in to a separate tilemap.
        for (var yCoordinate = -30; yCoordinate <= 30; ++yCoordinate)
        {
          var childMap = ComponentUtils.GetComponent<WorldMap>(Instantiate(gameObject, transform.parent));
          childMap.name = $"Row {yCoordinate}";
          childMap.IsChild = true;
          childMap.TilemapRenderer.sortingOrder = -yCoordinate;
          childMap.Tilemap.ClearAllTiles();

          for (var xCoordinate = -30; xCoordinate <= 30; ++xCoordinate)
          {
            childMap.Tilemap.SetTile(new Vector3Int(xCoordinate, yCoordinate, 0),
              _tilemap.GetTile(new Vector3Int(xCoordinate, yCoordinate, 0)));
          }

          if (yCoordinate == 8)
          {
            childMap.Tilemap.SetTile(new Vector3Int(-12, 8, 1), Instantiate(_bottomRightSelection));
            childMap.Tilemap.SetTile(new Vector3Int(-11, 8, 2), Instantiate(_bottomLeftSelection));
            childMap.Tilemap.SetTile(new Vector3Int(-11, 8, 3), Instantiate(_bottomRightSelection));
            childMap.Tilemap.SetTile(new Vector3Int(-10, 8, 3), Instantiate(_bottomLeftSelection));
          }

          if (yCoordinate == 7)
          {
            _overlayTilemap.SetTile(new Vector3Int(-12, 7, 2), _bottomRightSelection);
            _overlayTilemap.SetTile(new Vector3Int(-12, 7, 3), _bottomLeftSelection);
            _overlayTilemap.SetTile(new Vector3Int(-12, 7, 4), _leftSelection);

            _overlayTilemap.SetTile(new Vector3Int(-11, 7, 2), _bottomRightSelection);
            _overlayTilemap.SetTile(new Vector3Int(-11, 7, 3), _bottomLeftSelection);
            _overlayTilemap.SetTile(new Vector3Int(-11, 7, 4), _rightSelection);

          }
        }

        Destroy(gameObject);
      }
    }
  }
}