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
using UnityEngine.UIElements;
using UnityEngine;

#nullable enable

namespace Nighthollow.Interface
{
  public interface IDraggableElement<TElement, TTarget>
    where TElement : VisualElement, IDraggableElement<TElement, TTarget>
    where TTarget : class, IDragTarget<TElement, TTarget>
  {
    bool EnableDragging { set; }

    TTarget CurrentDragParent { get; set; }
  }

  public interface IDragTarget<in TElement, out TTarget>
    where TElement : VisualElement, IDraggableElement<TElement, TTarget>
    where TTarget : class, IDragTarget<TElement, TTarget>
  {
    TTarget This();

    VisualElement DragTargetElement { get; }

    void OnDraggableElementReceived(TElement element);

    void OnDraggableElementRemoved(TElement element);
  }

  public interface IDragManager<in TElement, TTarget>
    where TElement : VisualElement, IDraggableElement<TElement, TTarget>
    where TTarget : class, IDragTarget<TElement, TTarget>
  {
    /// <summary>
    /// Should return the possible target elements for drag operations -- these element's world bounds will be used to
    /// determine the enter/exit state of the drag as well as whether or not a draggable element is 'received' when it
    /// is dropped.
    ///
    /// Targets are currently assumed to be non-overlapping.
    /// </summary>
    IEnumerable<TTarget> GetDragTargets(TElement element);

    void OnDragReceived(TTarget target, TElement element);
  }

  public abstract class DragInfo
  {
    public abstract VisualElement Element { get; }
    public abstract IReadOnlyList<VisualElement> TargetElements { get; }
    public VisualElement OriginalParent { get; }
    public Vector2 DragStartMousePosition { get; }
    public Vector2 DragStartElementPosition { get; }
    public StyleLength OriginalLeft { get; }
    public StyleLength OriginalTop { get; }
    public StyleEnum<Position> OriginalPosition { get; }

    protected DragInfo(IMouseEvent e, VisualElement element)
    {
      OriginalParent = element.parent;
      DragStartMousePosition = e.mousePosition;
      DragStartElementPosition = element.worldBound.position;
      OriginalLeft = element.style.left;
      OriginalTop = element.style.top;
      OriginalPosition = element.style.position;
    }

    public abstract void OnDragEnter(int elementIndex);

    public abstract void OnDragExit(int elementIndex);

    public abstract void OnDropped(int elementIndex);

    public abstract VisualElement GetTarget(int elementIndex);

    public abstract bool EnableDragging { set; }
  }

  public sealed class DragInfo<TElement, TTarget> : DragInfo
    where TElement : VisualElement, IDraggableElement<TElement, TTarget>
    where TTarget : class, IDragTarget<TElement, TTarget>
  {
    readonly TElement _element;
    readonly IDragTarget<TElement, TTarget> _originalParentTarget;
    readonly IDragManager<TElement, TTarget> _dragManager;
    readonly IReadOnlyList<TTarget> _dragTargets;

    public DragInfo(IMouseEvent e, TElement element, IDragManager<TElement, TTarget> dragManager) : base(e, element)
    {
      _element = element;
      _originalParentTarget = element.CurrentDragParent;
      _dragManager = dragManager;
      _dragTargets = _dragManager.GetDragTargets(element).ToList();
      TargetElements = _dragTargets.Select(t => t.DragTargetElement).ToList();
    }

    public override VisualElement Element => _element;

    public override IReadOnlyList<VisualElement> TargetElements { get; }

    public override void OnDragEnter(int elementIndex)
    {
      var target = _dragTargets[elementIndex];
      target.DragTargetElement.AddToClassList("drag-highlight");
    }

    public override void OnDragExit(int elementIndex)
    {
      var target = _dragTargets[elementIndex];
      target.DragTargetElement.RemoveFromClassList("drag-highlight");
    }

    public override void OnDropped(int elementIndex)
    {
      var target = _dragTargets[elementIndex];
      target.DragTargetElement.RemoveFromClassList("drag-highlight");
      _element.CurrentDragParent = target.This();
      _originalParentTarget.OnDraggableElementRemoved(_element);
      target.OnDraggableElementReceived(_element);
      _dragManager.OnDragReceived(target, _element);
    }

    public override VisualElement GetTarget(int elementIndex) => _dragTargets[elementIndex].DragTargetElement;

    public override bool EnableDragging
    {
      set => _element.EnableDragging = value;
    }
  }
}
