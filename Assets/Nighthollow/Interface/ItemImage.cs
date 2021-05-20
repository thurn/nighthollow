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

using Nighthollow.Data;
using Nighthollow.Items;
using Nighthollow.Services;
using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public sealed class ItemImage : VisualElement, IDraggableElement<ItemImage, ItemSlot>
  {
    const int ContainerSize = 128;

    readonly IItemData _item;
    public IItemData Item => _item;
    readonly ServiceRegistry _registry;
    IDragManager<ItemImage, ItemSlot>? _dragManager;
    public bool HasTooltip { private get; set; }
    public bool EnableDragging { get; set; }
    public ItemSlot CurrentDragParent { get; set; }

    public ItemImage(ServiceRegistry registry, ItemSlot itemSlot, IItemData item, bool addTooltip = true)
    {
      CurrentDragParent = itemSlot;
      _registry = registry;
      _item = item;
      HasTooltip = addTooltip;

      RegisterCallback<MouseDownEvent>(OnMouseDown);
      RegisterCallback<MouseOverEvent>(OnMouseOver);
      RegisterCallback<MouseOutEvent>(OnMouseOut);

      AddToClassList("item-image");
      var imageAddress = _item.GetImageAddress(registry.Database.Snapshot());
      style.backgroundImage =
        new StyleBackground(registry.AssetService.GetImage(Errors.CheckNotNull(imageAddress, "Item has no image!")));
    }

    public void MakeDraggable(IDragManager<ItemImage, ItemSlot> dragManager)
    {
      _dragManager = dragManager;
      EnableDragging = true;
    }

    void OnMouseDown(MouseDownEvent e)
    {
      if (EnableDragging && _dragManager != null)
      {
        _registry.ScreenController.StartDrag(new DragInfo<ItemImage, ItemSlot>(e, this, _dragManager));
      }
    }

    void OnMouseOver(MouseOverEvent e)
    {
      if (HasTooltip && !_registry.ScreenController.IsCurrentlyDragging)
      {
        var tooltipBuilder = TooltipUtil.CreateTooltip(
          _registry.Database.Snapshot(),
          new Vector2(worldBound.x, worldBound.y),
          _item);
        tooltipBuilder.XOffset = ContainerSize;
        //_registry.ScreenController.ShowTooltip(tooltipBuilder, new Vector2(worldBound.x, worldBound.y));
        _registry.ScreenController.Get(ScreenController.Tooltip).Show(tooltipBuilder);
      }
    }

    void OnMouseOut(MouseOutEvent e)
    {
      if (HasTooltip)
      {
        _registry.ScreenController.Get(ScreenController.Tooltip).Hide();
      }
    }
  }
}