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

using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public sealed class DragController
  {
    DragInfo? _currentlyDragging;
    int? _currentlyWithinDragTarget;
    public bool IsCurrentlyDragging => _currentlyDragging != null;

    public void StartDrag(ScreenController screenController, DragInfo dragInfo)
    {
      _currentlyDragging = dragInfo;
      var element = dragInfo.Element;
      screenController.Get(ScreenController.Tooltip).Hide();
      element.RemoveFromHierarchy();

      element.style.position = new StyleEnum<Position>(Position.Absolute);
      screenController.Screen.Add(element);
      element.style.left = _currentlyDragging.DragStartElementPosition.x;
      element.style.top = _currentlyDragging.DragStartElementPosition.y;
      element.style.width = element.contentRect.width;
      element.style.height = element.contentRect.height;
    }

    public void OnMouseMove(MouseMoveEvent e)
    {
      if (_currentlyDragging != null)
      {
        var diff = e.mousePosition - _currentlyDragging.DragStartMousePosition;
        _currentlyDragging.Element.style.left = new StyleLength(_currentlyDragging.DragStartElementPosition.x + diff.x);
        _currentlyDragging.Element.style.top = new StyleLength(_currentlyDragging.DragStartElementPosition.y + diff.y);

        var withinDragTarget = FindWithinDragTarget(_currentlyDragging, e);
        if (_currentlyWithinDragTarget == null && withinDragTarget != null)
        {
          _currentlyWithinDragTarget = withinDragTarget;
          _currentlyDragging.OnDragEnter(withinDragTarget.Value);
        }
        else if (_currentlyWithinDragTarget != null && withinDragTarget == null)
        {
          _currentlyDragging.OnDragExit(_currentlyWithinDragTarget.Value);
          _currentlyWithinDragTarget = null;
        }
      }
    }

    static int? FindWithinDragTarget(DragInfo currentlyDragging, IMouseEvent e)
    {
      for (var i = 0; i < currentlyDragging.TargetElements.Count; ++i)
      {
        var target = currentlyDragging.TargetElements[i];
        if (target.worldBound.Contains(e.mousePosition))
        {
          return i;
        }
      }

      return null;
    }

    public void OnMouseUp(MouseUpEvent e)
    {
      if (_currentlyDragging != null)
      {
        var currentlyDragging = _currentlyDragging;
        _currentlyDragging = null;
        _currentlyWithinDragTarget = null;

        var element = currentlyDragging.Element;
        currentlyDragging.EnableDragging = false;

        var droppedOverTargetIndex = FindWithinDragTarget(currentlyDragging, e);

        var targetParent = currentlyDragging.OriginalParent;
        if (droppedOverTargetIndex != null)
        {
          targetParent = currentlyDragging.GetTarget(droppedOverTargetIndex.Value);
        }

        element.RemoveFromHierarchy();
        targetParent.Add(element);
        element.style.position = currentlyDragging.OriginalPosition;
        element.style.left = currentlyDragging.OriginalLeft;
        element.style.top = currentlyDragging.OriginalTop;
        element.style.width = currentlyDragging.OriginalWidth;
        element.style.height = currentlyDragging.OriginalHeight;

        if (droppedOverTargetIndex != null)
        {
          currentlyDragging.OnDropped(droppedOverTargetIndex.Value);
        }

        currentlyDragging.EnableDragging = true;
      }
    }
  }
}