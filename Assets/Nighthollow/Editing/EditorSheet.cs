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
    readonly Action? _onEscape;
    readonly ScrollView _scrollView;
    readonly VisualElement _content;
    readonly Vector2Int? _initiallySelected;

    Dictionary<Vector2Int, EditorCell> _cells;
    Label? _leftPositionHelper;
    Label? _topPositionHelper;

    public Vector2Int? CurrentlySelected;
    Vector2Int? CurrentlyActive { get; set; }

    public EditorSheet(
      ScreenController controller,
      EditorSheetDelegate sheetDelegate,
      Vector2Int? selected,
      Action? onEscape = null)
    {
      _screenController = controller;
      _sheetDelegate = sheetDelegate;
      _onEscape = onEscape;
      _initiallySelected = selected;

      _content = new VisualElement();
      _content.AddToClassList("editor-content");

      _scrollView = new ScrollView();
      _scrollView.AddToClassList("editor-scroll-view");
      _scrollView.Add(_content);

      Add(_scrollView);

      _cells = RenderCells();
      _sheetDelegate.Initialize(OnDataChanged);

      focusable = true;
      RegisterCallback<KeyDownEvent>(OnKeyDown);
      RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    void OnGeometryChanged(GeometryChangedEvent evt)
    {
      Focus();
      SelectPosition(_initiallySelected ?? Vector2Int.zero);
      UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    public int Width { get; private set; }

    Dictionary<Vector2Int, EditorCell> RenderCells()
    {
      _content.Clear();

      var content = _sheetDelegate.GetCells();
      var cells = content.Cells;
      var result = new Dictionary<Vector2Int, EditorCell>();
      var columnCount = cells.Max(row => row.Count);
      var columnWidths = content.ColumnWidths;
      Width = ContentPadding + columnWidths.Sum();

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
            if (position == CurrentlySelected)
            {
              ActivateCell(position);
            }
            else
            {
              SelectPosition(position);
            }
          });

          cell.style.width = columnWidths[columnIndex];

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
      var footer = new VisualElement();
      footer.AddToClassList("editor-footer");
      _content.Add(footer);

      return result;
    }

    void OnDataChanged()
    {
      var previouslySelected = CurrentlySelected;
      var key = CurrentlySelected.HasValue ? _cells[CurrentlySelected.Value].Key : null;
      CurrentlySelected = null;
      CurrentlyActive = null;
      _cells = RenderCells();

      SelectPosition(key != null ? _cells.FirstOrDefault(p => p.Value.Key == key).Key : previouslySelected);
    }

    public void OnKeyDown(KeyDownEvent evt)
    {
      if (!CurrentlySelected.HasValue)
      {
        return;
      }

      if (CurrentlyActive.HasValue)
      {
        _cells[CurrentlyActive.Value].OnParentKeyDown(evt);
        return;
      }

      switch (evt.keyCode)
      {
        case KeyCode.Tab when !evt.shiftKey:
        case KeyCode.RightArrow:
          SelectPosition(CurrentlySelected.Value + new Vector2Int(1, 0));
          break;
        case KeyCode.Tab when evt.shiftKey:
        case KeyCode.LeftArrow:
          SelectPosition(CurrentlySelected.Value + new Vector2Int(-1, 0));
          break;
        case KeyCode.UpArrow:
          SelectPosition(CurrentlySelected.Value + new Vector2Int(0, -1));
          break;
        case KeyCode.DownArrow:
          SelectPosition(CurrentlySelected.Value + new Vector2Int(0, 1));
          break;
        case KeyCode.KeypadEnter:
        case KeyCode.Return:
          if (CurrentlyActive == null)
          {
            ActivateCell(CurrentlySelected.Value);
          }

          break;
        case KeyCode.Escape:
        case KeyCode.Backspace:
          _onEscape?.Invoke();
          break;
      }
    }

    void ActivateCell(Vector2Int position)
    {
      if (position != CurrentlyActive)
      {
        if (CurrentlyActive.HasValue)
        {
          _cells[CurrentlyActive.Value].Deactivate();
        }

        CurrentlyActive = position;
        _cells[position].Activate();
      }
    }

    void SelectPosition(Vector2Int? position)
    {
      if (position.HasValue && _cells.ContainsKey(position.Value) && position != CurrentlySelected)
      {
        if (CurrentlyActive.HasValue)
        {
          _cells[CurrentlyActive.Value].Deactivate();
        }

        CurrentlyActive = null;
        Focus();

        if (CurrentlySelected.HasValue)
        {
          _cells[CurrentlySelected.Value].Unselect();
        }

        _cells[position.Value].Select();
        CurrentlySelected = position;
        Scroll(position.Value);
        RenderPositionHelpers(position.Value);
      }
    }

    public void FocusRoot()
    {
      Focus();
    }

    public void OnChildEditingComplete()
    {
      CurrentlyActive = null;
      FocusRoot();
    }

    public void DeactivateAllCells()
    {
      foreach (var cell in _cells.Values)
      {
        cell.Deactivate();
      }
    }

    bool IsFullScreen(Vector2Int position) => _cells[position].GetClasses().Contains("full-width-cell");

    void Scroll(Vector2Int position)
    {
      _scrollView.ScrollTo(InterfaceUtils.FirstLeaf(_cells[position]));

      // Fix unity refusing to scroll all the way to the edge, triggering me endlessly.
      if (position.y == 0)
      {
        _scrollView.scrollOffset =
          IsFullScreen(position) ? new Vector2(0, 0) : new Vector2(_scrollView.scrollOffset.x, 0);
      }

      if (position.x == 0)
      {
        _scrollView.scrollOffset = new Vector2(0, _scrollView.scrollOffset.y);
      }
    }

    void RenderPositionHelpers(Vector2Int position)
    {
      var leftIndex = new Vector2Int(0, position.y);
      string? left = null;
      while (_cells.ContainsKey(leftIndex))
      {
        if (_cells[leftIndex].Preview() is { } preview)
        {
          left = preview;
          break;
        }

        leftIndex = new Vector2Int(leftIndex.x + 1, leftIndex.y);
      }

      var topIndex = new Vector2Int(position.x, 0);
      string? top = null;
      while (_cells.ContainsKey(topIndex))
      {
        if (!IsFullScreen(topIndex) && _cells[topIndex].Preview() is { } preview)
        {
          top = preview;
          break;
        }

        topIndex = new Vector2Int(topIndex.x, topIndex.y + 1);
      }

      var currentPosition = _cells[position].worldBound;
      _leftPositionHelper?.RemoveFromHierarchy();
      if (!IsFullScreen(position) && _scrollView.scrollOffset.x > 0 && left != null)
      {
        _leftPositionHelper = new Label(left);
        _leftPositionHelper.style.left = 32;
        _leftPositionHelper.style.top = currentPosition.y - 16;
        _leftPositionHelper.AddToClassList("position-helper");
        Add(_leftPositionHelper);
      }

      _topPositionHelper?.RemoveFromHierarchy();
      if (!IsFullScreen(position) && _scrollView.scrollOffset.y > 0 && top != null)
      {
        _topPositionHelper = new Label(top);
        _topPositionHelper.style.left = currentPosition.x - 16;
        _topPositionHelper.style.top = 0;
        _topPositionHelper.style.width = currentPosition.width;
        _topPositionHelper.AddToClassList("position-helper");
        Add(_topPositionHelper);
      }
    }
  }
}