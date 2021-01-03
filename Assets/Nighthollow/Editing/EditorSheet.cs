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
using Nighthollow.Interface;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Editing
{
  public interface IEditor
  {
    /// <summary>
    /// When you click on things, Unity takes focus away and drops your next KeyDown event, even if what you clicked
    /// on has `focusable` set to false. So all child click and key handlers need to call this method to re-focus the
    /// sheet properly to get click events.
    /// </summary>
    void FocusRoot();

    void OnChildEditingComplete();
  }

  public sealed class EditorSheet : VisualElement, IEditor
  {
    public const int DefaultCellWidth = 250;
    const int ContentPadding = 32;

    readonly ScreenController _screenController;
    readonly EditorSheetDelegate _sheetDelegate;
    readonly IEditor? _parent;
    readonly ScrollView _scrollView;
    readonly VisualElement _content;

    Dictionary<Vector2Int, EditorCell> _cells;
    Vector2Int? _currentlySelected;
    EditorCell? _currentlyActive;

    public EditorSheet(ScreenController controller, EditorSheetDelegate sheetDelegate, IEditor? parent = null)
    {
      _screenController = controller;
      _sheetDelegate = sheetDelegate;
      _parent = parent;

      _content = new VisualElement();
      _content.AddToClassList("editor-content");

      if (_sheetDelegate.ContentHeightOverride != null)
      {
        _content.style.height = _sheetDelegate.ContentHeightOverride.Value;
      }

      _scrollView = new ScrollView();
      _scrollView.AddToClassList("editor-scroll-view");
      _scrollView.Add(_content);

      Add(_scrollView);

      _cells = RenderCells();
      _sheetDelegate.Initialize(OnDataChanged);

      focusable = _parent == null;
      if (_parent == null)
      {
        RegisterCallback<KeyDownEvent>(OnKeyDown);
      }

      SelectPosition(Vector2Int.zero);
      InterfaceUtils.After(0.1f, FocusRoot);
    }

    public int Width { get; private set; }

    Dictionary<Vector2Int, EditorCell> RenderCells()
    {
      _content.Clear();
      var cells = _sheetDelegate.GetCells();
      var result = new Dictionary<Vector2Int, EditorCell>();
      var columnCount = cells.Max(row => row.Count);
      var columnWidths = _sheetDelegate.GetColumnWidths();
      Width = ContentPadding + (columnWidths?.Sum() ?? (DefaultCellWidth * columnCount));

      for (var rowIndex = 0; rowIndex < cells.Count; rowIndex++)
      {
        var list = cells[rowIndex];
        for (var columnIndex = 0; columnIndex < columnCount; columnIndex++)
        {
          var position = new Vector2Int(columnIndex, rowIndex);

          if (columnIndex != 0 && list.Count == 1)
          {
            // Full width column support
            result[position] = result[new Vector2Int(0, rowIndex)];
            continue;
          }

          var cell = columnIndex >= list.Count
            ? EditorCellFactory.CreateBlank()
            : EditorCellFactory.Create(_screenController, this, list[columnIndex]);
          cell.RegisterCallback<ClickEvent>(e =>
          {
            if (position == _currentlySelected)
            {
              ActivateCell(position);
            }
            else
            {
              SelectPosition(position);
            }

            _parent?.FocusRoot();
          });

          cell.style.width = columnWidths?[columnIndex] ?? DefaultCellWidth;

          if (list.Count == 1)
          {
            cell.style.width = Width - ContentPadding;
            cell.AddToClassList("full-width-cell");
          }

          result[position] = cell;
          _content.Add(cell);
        }
      }

      _content.style.width = Width;

      return result;
    }

    void OnDataChanged()
    {
      var previouslySelected = _currentlySelected;
      var key = _currentlySelected.HasValue ? _cells[_currentlySelected.Value].Key : null;
      _currentlySelected = null;
      _currentlyActive = null;
      _cells = RenderCells();

      SelectPosition(key != null ? _cells.FirstOrDefault(p => p.Value.Key == key).Key : previouslySelected);
    }

    public void OnKeyDown(KeyDownEvent evt)
    {
      if (!_currentlySelected.HasValue)
      {
        return;
      }

      if (_currentlyActive != null)
      {
        _currentlyActive.OnParentKeyDown(evt);
        return;
      }

      switch (evt.keyCode)
      {
        case KeyCode.Tab when !evt.shiftKey:
        case KeyCode.RightArrow:
          SelectPosition(_currentlySelected.Value + new Vector2Int(1, 0));
          break;
        case KeyCode.Tab when evt.shiftKey:
        case KeyCode.LeftArrow:
          SelectPosition(_currentlySelected.Value + new Vector2Int(-1, 0));
          break;
        case KeyCode.UpArrow:
          SelectPosition(_currentlySelected.Value + new Vector2Int(0, -1));
          break;
        case KeyCode.DownArrow:
          SelectPosition(_currentlySelected.Value + new Vector2Int(0, 1));
          break;
        case KeyCode.KeypadEnter:
        case KeyCode.Return:
          ActivateCell(_currentlySelected.Value);
          break;
        case KeyCode.Escape:
        case KeyCode.Backspace:
          _parent?.OnChildEditingComplete();
          break;
      }
    }

    void ActivateCell(Vector2Int position)
    {
      _currentlyActive?.Deactivate();
      _currentlyActive = _cells[position];
      _currentlyActive.Activate();
    }

    void SelectPosition(Vector2Int? position)
    {
      if (position.HasValue && _cells.ContainsKey(position.Value) && position != _currentlySelected)
      {
        _currentlyActive?.Deactivate();
        _currentlyActive = null;

        if (_parent == null)
        {
          Focus();
        }

        if (_currentlySelected.HasValue)
        {
          _cells[_currentlySelected.Value].Unselect();
        }

        _cells[position.Value].Select();
        _currentlySelected = position;

        if (!_cells[position.Value].ClassListContains("full-width-cell"))
        {
          _scrollView.ScrollTo(_cells[position.Value]);
        }
      }
    }

    public void FocusRoot()
    {
      if (_parent == null)
      {
        Focus();
      }
      else
      {
        _parent.FocusRoot();
      }
    }

    public void OnChildEditingComplete()
    {
      _currentlyActive = null;
      FocusRoot();
    }

    public void DeactivateAllCells()
    {
      foreach (var cell in _cells.Values)
      {
        cell.Deactivate();
      }
    }
  }
}