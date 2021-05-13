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

using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable

namespace Nighthollow.World
{
  public sealed class WorldStaticAssets : MonoBehaviour
  {
    [SerializeField] Tile _bottomLeftSelection = null!;
    public Tile CreateBottomLeftSelection() => Instantiate(_bottomLeftSelection);

    [SerializeField] Tile _bottomRightSelection = null!;
    public Tile CreateBottomRightSelection() => Instantiate(_bottomRightSelection);

    [SerializeField] Tile _rightSelection = null!;
    public Tile CreateRightSelection() => Instantiate(_rightSelection);

    [SerializeField] Tile _leftSelection = null!;
    public Tile CreateLeftSelection() => Instantiate(_leftSelection);

    [SerializeField] GameObject _worldTilemapContainer = null!;
    public GameObject WorldTilemapContainer => _worldTilemapContainer;

    [SerializeField] Tilemap _overlayTilemap = null!;
    public Tilemap OverlayTilemap => _overlayTilemap;

    [SerializeField] Grid _grid = null!;
    public Grid Grid => _grid;

    [SerializeField] Tile _selectedTileIcon = null!;
    public Tile CreateSelectedTileIcon() => Instantiate(_selectedTileIcon);
  }
}