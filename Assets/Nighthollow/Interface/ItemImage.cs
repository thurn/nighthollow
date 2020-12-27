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

using Nighthollow.Items;
using Nighthollow.Model;
using Nighthollow.Services;
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

    readonly ScreenController _screenController;

    IDragManager<ItemImage, ItemSlot>? _dragManager;

    public bool HasTooltip { private get; set; }

    public bool EnableDragging { get; set; }
    public ItemSlot CurrentDragParent { get; set; }

    public ItemImage(ScreenController screenController, ItemSlot itemSlot, IItemData item, bool addTooltip = true)
    {
      CurrentDragParent = itemSlot;
      _screenController = screenController;
      _item = item;
      HasTooltip = addTooltip;

      RegisterCallback<MouseDownEvent>(OnMouseDown);
      RegisterCallback<MouseOverEvent>(OnMouseOver);
      RegisterCallback<MouseOutEvent>(OnMouseOut);

      AddToClassList("item-image");
      style.backgroundImage = new StyleBackground(Database.Instance.Assets.GetImage(_item.ImageAddress));
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
        _screenController.StartDrag(new DragInfo<ItemImage, ItemSlot>(e, this, _dragManager));
      }
    }

    void OnMouseOver(MouseOverEvent e)
    {
      if (HasTooltip)
      {
        var tooltipBuilder = TooltipUtil.CreateTooltip(Database.Instance.UserData.Stats, _item);
        tooltipBuilder.XOffset = ContainerSize;
        _screenController.ShowTooltip(tooltipBuilder, new Vector2(worldBound.x, worldBound.y));
      }
    }

    void OnMouseOut(MouseOutEvent e)
    {
      if (HasTooltip)
      {
        _screenController.HideTooltip();
      }
    }
  }
}
