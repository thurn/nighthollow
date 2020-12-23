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
using UnityEngine;

#nullable enable

namespace Nighthollow.Interface
{
  public interface IDragManager<in T>
  {
    /// <summary>
    /// Should return the target element for drag operations -- this element's world bounds will be used to determine
    /// the enter/exit state of the drag as well as whether or not a draggable element is 'received' when it is dropped.
    /// </summary>
    VisualElement GetDragTarget(T element);

    /// <summary>Called when a draggable element is dragged into the drag target.</summary>
    void OnDragEnter(T element);

    /// <summary>Called when a draggable element is dragged out of the drag target.</summary>
    void OnDragExit(T element);

    /// <summary>
    /// Called when the draggable element is released over the drag target. Should return the new parent
    /// element for the draggable element -- the element will be animated to be a child of this parent. The original
    /// USS 'position', 'left', and 'top' attribute values will be restored. If an element is released while *not* over
    /// the drag target, it is instead returned to its original position.
    /// </summary>
    VisualElement OnDraggableReleased(T element);

    /// <summary>
    /// Called when the animation re-parenting a draggable element is completed.
    /// </summary>
    void OnDragReceived(T element);
  }

  public interface IDraggableElement<out T>
  {
    void MakeDraggable(IDragManager<T> dragManager);

    void DisableDragging();
  }

  public abstract class DragInfo
  {
    public abstract VisualElement Element { get; }
    public abstract VisualElement DragTarget { get; }
    public VisualElement OriginalParent { get; }
    public Vector2 DragStartMousePosition { get; }
    public Vector2 DragStartElementPosition { get; }
    public StyleLength OriginalLeft { get; }
    public StyleLength OriginalTop { get; }
    public StyleEnum<Position> OriginalPosition { get; }

    protected DragInfo(MouseDownEvent e, VisualElement element)
    {
      OriginalParent = element.parent;
      DragStartMousePosition = e.mousePosition;
      DragStartElementPosition = element.worldBound.position;
      OriginalLeft = element.style.left;
      OriginalTop = element.style.top;
      OriginalPosition = element.style.position;
    }

    public abstract void OnDragEnter();

    public abstract void OnDragExit();

    public abstract VisualElement OnDraggableReleased();

    public abstract void OnDragReceived();

    public abstract void MakeElementDraggable();

    public abstract void DisableElementDragging();
  }

  public sealed class DragInfo<T> : DragInfo where T : VisualElement, IDraggableElement<T>
  {
    readonly T _element;
    readonly IDragManager<T> _dragManager;
    public override VisualElement DragTarget { get; }

    public DragInfo(MouseDownEvent e, T element, IDragManager<T> dragManager) : base(e, element)
    {
      _element = element;
      _dragManager = dragManager;
      DragTarget = _dragManager.GetDragTarget(_element);
    }

    public override VisualElement Element => _element;

    public override void OnDragEnter() => _dragManager.OnDragEnter(_element);

    public override void OnDragExit() => _dragManager.OnDragExit(_element);

    public override VisualElement OnDraggableReleased() => _dragManager.OnDraggableReleased(_element);

    public override void OnDragReceived() => _dragManager.OnDragReceived(_element);

    public override void MakeElementDraggable() => _element.MakeDraggable(_dragManager);

    public override void DisableElementDragging() => _element.DisableDragging();
  }
}
