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
using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

#nullable enable

namespace Nighthollow.World
{
  public sealed class WorldMap : MonoBehaviour
  {
    const int BottomLeftZ = 1;
    const int BottomRightZ = 2;
    const int RightZ = 3;
    const int LeftZ = 4;
    const int IconZ = 5;

    readonly Dictionary<int, Tilemap> _children = new Dictionary<int, Tilemap>();
    Vector2Int? _currentlySelected;
    TileBase _previousIconOnSelectedTile = null!;

    void Awake()
    {
      Errors.CheckNotNull(_overlayTilemap);
      Errors.CheckNotNull(_mainCamera);
      Errors.CheckNotNull(_grid);
    }

    void Update()
    {
      if (Input.GetMouseButtonDown(button: 0) && !_screen.ConsumesMousePosition(Input.mousePosition))
      {
        var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        var worldPoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
        var position = _grid.WorldToCell(worldPoint);
        var hex = new Vector2Int(position.x, position.y);

        ClearPreviousSelection();

        _previousIconOnSelectedTile = GetIcon(hex);
        ShowIcon(hex, _selectedTileIcon);
        _currentlySelected = hex;

        var tile = _children[hex.y].GetTile(new Vector3Int(hex.x, hex.y, z: 0));
        var screenPoint = _mainCamera.WorldToScreenPoint(_grid.CellToWorld(position));
        _screen.ShowTooltip(WorldHexTooltip.Create(
            this,
            tile.name,
            hex == WorldTutorial.StartingHex ? "Kingdom of Nighthollow" : "None",
            hex == WorldTutorial.TutorialAttackHex),
          InterfaceUtils.ScreenPointToInterfacePoint(screenPoint));
      }
    }

    public void Initialize()
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
          tilemap.SetTile(new Vector3Int(xCoordinate, yCoordinate, z: 0),
            map.GetTile(new Vector3Int(xCoordinate, yCoordinate, z: 0)));

        _children[yCoordinate] = tilemap;
      }

      map.ClearAllTiles();
      Destroy(GetComponent<TilemapRenderer>());
      Destroy(GetComponent<Tilemap>());

      foreach (var kingdom in _kingdoms)
      {
        OutlineHexes(kingdom.BorderColor, new HashSet<Vector2Int> {kingdom.StartingLocation});
        ShowIcon(kingdom.StartingLocation, kingdom.Icon);
      }
    }

    public void ShowIcon(Vector2Int hex, TileBase tile)
    {
      _overlayTilemap.SetTile(new Vector3Int(hex.x, hex.y, IconZ), tile);
    }

    public void RemoveIcon(Vector2Int hex)
    {
      _overlayTilemap.SetTile(new Vector3Int(hex.x, hex.y, IconZ), tile: null);
    }

    public TileBase GetIcon(Vector2Int hex)
    {
      return _overlayTilemap.GetTile(new Vector3Int(hex.x, hex.y, IconZ));
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
        if (!hexes.Contains(upLeft)) ApplySelection(upLeft, bottomRightSelection, BottomRightZ, toOverlay: false);

        var upRight = HexUtils.GetInDirection(hex, HexUtils.Direction.UpRight);
        if (!hexes.Contains(upRight)) ApplySelection(upRight, bottomLeftSelection, BottomLeftZ, toOverlay: false);

        if (!hexes.Contains(HexUtils.GetInDirection(hex, HexUtils.Direction.Left)))
          ApplySelection(hex, leftSelection, LeftZ, toOverlay: true);

        if (!hexes.Contains(HexUtils.GetInDirection(hex, HexUtils.Direction.Right)))
          ApplySelection(hex, rightSelection, RightZ, toOverlay: true);

        if (!hexes.Contains(HexUtils.GetInDirection(hex, HexUtils.Direction.DownLeft)))
          ApplySelection(hex, bottomLeftSelection, BottomLeftZ, toOverlay: true);

        if (!hexes.Contains(HexUtils.GetInDirection(hex, HexUtils.Direction.DownRight)))
          ApplySelection(hex, bottomRightSelection, BottomRightZ, toOverlay: true);
      }
    }

    void ApplySelection(Vector2Int hex, Tile tile, int zPosition, bool toOverlay)
    {
      if (toOverlay)
        _overlayTilemap.SetTile(new Vector3Int(hex.x, hex.y, zPosition), tile);
      else
        _children[hex.y].SetTile(new Vector3Int(hex.x, hex.y, zPosition), tile);
    }

    public void ClearSelection()
    {
      ClearPreviousSelection();
      _screen.HideTooltip();
      _currentlySelected = null;
    }

    void ClearPreviousSelection()
    {
      if (_currentlySelected.HasValue)
      {
        RemoveIcon(_currentlySelected.Value);
        if (_previousIconOnSelectedTile) ShowIcon(_currentlySelected.Value, _previousIconOnSelectedTile);
      }
    }

    public void AttackHex()
    {
      StartCoroutine(LoadGame());
    }

    IEnumerator<YieldInstruction> LoadGame()
    {
      _screen.HideTooltip();
      _screen.Get(ScreenController.BlackoutWindow).Show(argument: 1.0f);
      _screen.ShowDialog("you", "Surrender to the night!", hideCloseButton: true);
      yield return new WaitForSeconds(seconds: 3.5f);
      _screen.HideDialog();
      yield return new WaitForSeconds(seconds: 0.5f);

      Database.Instance.UserData.TutorialState = UserDataService.Tutorial.GameOne;
      SceneManager.LoadScene("Game");
    }
#pragma warning disable 0649
    [SerializeField] ScreenController _screen = null!;
    [SerializeField] Tile _bottomLeftSelection = null!;
    [SerializeField] Tile _bottomRightSelection = null!;
    [SerializeField] Tile _rightSelection = null!;
    [SerializeField] Tile _leftSelection = null!;
    [SerializeField] Tilemap _overlayTilemap = null!;
    [SerializeField] Camera _mainCamera = null!;
    [SerializeField] Grid _grid = null!;
    [SerializeField] List<KingdomData> _kingdoms = null!;
    [SerializeField] Tile _selectedTileIcon = null!;
#pragma warning restore 0649
  }
}
