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

using System.Collections.Immutable;
using System.Linq;
using Nighthollow.Rules;
using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface.Components.Core
{
  public interface IDragReceiver
  {
  }

  public interface IDragReceiver<in T> : IDragReceiver
  {
    bool CanReceiveDrag(T value);

    void OnDragReceived(Scope scope, T value);
  }

  public interface IDraggable
  {
    string TargetClassName { get; }

    bool CanReceiveDrag(IDragReceiver? receiver);

    void OnDragReceived(Scope scope, IDragReceiver receiver);
  }

  public sealed class Draggable<T> : IDraggable
  {
    readonly T _value;

    public Draggable(T value, string targetClassName)
    {
      _value = value;
      TargetClassName = targetClassName;
    }

    public string TargetClassName { get; }

    public bool CanReceiveDrag(IDragReceiver? receiver) => receiver is IDragReceiver<T> r && r.CanReceiveDrag(_value);

    public void OnDragReceived(Scope scope, IDragReceiver receiver)
    {
      ((IDragReceiver<T>) receiver).OnDragReceived(scope, _value);
    }
  }

  public interface IMakeDraggable
  {
    /// <summary>
    /// Starts a drag of a visual element targeting parent elements with the provided class names.
    /// </summary>
    void OnMouseDown(VisualElement element, MouseDownEvent e, IDraggable draggable);
  }

  /// <summary>
  /// Manages the drag and drop operations of a <see cref="ComponentController"/>. Currently only supports dragging one
  /// element at a time.
  /// </summary>
  public sealed class DragController : IMakeDraggable
  {
    readonly VisualElement _rootElement;
    readonly ComponentController _componentController;
    DragElement? _currentlyDragging;
    int? _currentlyWithinDragTarget;

    public DragController(VisualElement rootElement, ComponentController c)
    {
      _rootElement = rootElement;
      _componentController = c;
    }

    public bool IsDragging => _currentlyDragging != null;

    public void OnMouseDown(VisualElement element, MouseDownEvent e, IDraggable draggable)
    {
      _componentController.OnDragBegin();

      Errors.CheckState(_currentlyDragging == null,
        "Cannot drag multiple elements at once");
      var targets = _rootElement.Query(className: draggable.TargetClassName)
        .ToList()
        .Where(target => draggable.CanReceiveDrag(((ComponentVisualElement) target).DragReceiver))
        .ToImmutableList();

      _currentlyDragging = new DragElement(element, targets, draggable, e);
      element.RemoveFromHierarchy();
      _rootElement.Add(element);
      element.style.position = new StyleEnum<Position>(Position.Absolute);
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
        }
        else if (_currentlyWithinDragTarget != null && withinDragTarget == null)
        {
          _currentlyWithinDragTarget = null;
        }
      }
    }

    public void OnMouseUp(Scope scope, MouseUpEvent e)
    {
      if (_currentlyDragging != null)
      {
        var currentlyDragging = _currentlyDragging;
        _currentlyDragging = null;
        _currentlyWithinDragTarget = null;

        var element = currentlyDragging.Element;
        var droppedOverTargetIndex = FindWithinDragTarget(currentlyDragging, e);

        var targetParent = currentlyDragging.OriginalParent;
        if (droppedOverTargetIndex != null)
        {
          targetParent = currentlyDragging.Targets[droppedOverTargetIndex.Value];
          var receiver = Errors.CheckNotNull(((ComponentVisualElement) targetParent).DragReceiver,
            "Expected a DragReceiver to be attached to drag target.");
          currentlyDragging.Draggable.OnDragReceived(scope, receiver);
        }

        element.RemoveFromHierarchy();
        targetParent.Add(element);
        element.style.position = currentlyDragging.OriginalPosition;
        element.style.left = currentlyDragging.OriginalLeft;
        element.style.top = currentlyDragging.OriginalTop;
        element.style.width = currentlyDragging.OriginalWidth;
        element.style.height = currentlyDragging.OriginalHeight;

        _componentController.OnDragCompleted();
      }
    }

    static int? FindWithinDragTarget(DragElement currentlyDragging, IMouseEvent e)
    {
      for (var i = 0; i < currentlyDragging.Targets.Count; ++i)
      {
        var target = currentlyDragging.Targets[i];
        if (target.worldBound.Contains(e.mousePosition))
        {
          return i;
        }
      }

      return null;
    }

    sealed class DragElement
    {
      public VisualElement Element { get; }
      public ImmutableList<VisualElement> Targets { get; }
      public IDraggable Draggable { get; }
      public VisualElement OriginalParent { get; }
      public Vector2 DragStartMousePosition { get; }
      public Vector2 DragStartElementPosition { get; }
      public StyleLength OriginalLeft { get; }
      public StyleLength OriginalTop { get; }
      public StyleLength OriginalWidth { get; }
      public StyleLength OriginalHeight { get; }
      public StyleEnum<Position> OriginalPosition { get; }

      public DragElement(
        VisualElement element,
        ImmutableList<VisualElement> targets,
        IDraggable draggable,
        IMouseEvent e)
      {
        Element = element;
        Targets = targets;
        Draggable = draggable;
        OriginalParent = element.parent;
        DragStartMousePosition = e.mousePosition;
        DragStartElementPosition = element.worldBound.position;
        OriginalLeft = element.style.left;
        OriginalTop = element.style.top;
        OriginalWidth = element.style.width;
        OriginalHeight = element.style.height;
        OriginalPosition = element.style.position;
      }
    }
  }
}