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
using System.Collections.Immutable;
using Nighthollow.Data;
using Nighthollow.Interface.Components.Core;
using Nighthollow.Rules;
using Nighthollow.Utils;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface.Components.Library
{
  public sealed record ItemSlot : LayoutComponent, IDragReceiver<ItemImage>
  {
    public enum SlotSize
    {
      Large,
      Small
    }

    public SlotSize Size { get; init; } = SlotSize.Large;
    public int? ItemId { get; init; }
    public IItemData? Item { get; init; }
    public bool HasTooltip { get; init; } = true;
    public ItemImage.Location ItemLocation { get; init; }
    public bool Draggable { get; init; }

    protected override BaseComponent OnRender(Scope scope)
    {
      var size = Size switch
      {
        SlotSize.Large => 128,
        SlotSize.Small => 96,
        _ => throw new ArgumentOutOfRangeException()
      };

      return new Column
      {
        BackgroundImage = "GUI/ItemSlot",
        MarginAll = 8,
        AlignItems = Align.Center,
        JustifyContent = Justify.Center,
        Width = size,
        Height = size,
        FlexBasis = size,
        OnMouseOver = e => MouseOver(scope, e),
        OnMouseOut = e => MouseOut(scope, e),
        ClassNames = ImmutableHashSet.Create(ClassName.ItemSlot),
        DragReceiver = this,
        Children = List(Item is null
          ? null
          : new ItemImage(Item)
          {
            ItemId = ItemId,
            ItemLocation = ItemLocation,
            Draggable = Draggable,
            Size = Size
          })
      };
    }

    void MouseOver(Scope scope, MouseOverEvent evt)
    {
      if (HasTooltip && Item != null)
      {
        scope.Get(Key.ComponentController).UpdateRoot(root => root with
        {
          CurrentTooltip = new ItemTooltip(Item, ((VisualElement) evt.currentTarget).worldBound)
        });
      }
    }

    void MouseOut(Scope scope, MouseOutEvent evt)
    {
      if (HasTooltip && Item != null)
      {
        scope.Get(Key.ComponentController).UpdateRoot(root => root with
        {
          CurrentTooltip = null
        });
      }
    }

    public bool CanReceiveDrag(ItemImage value)
    {
      return Item is null && ItemLocation switch
      {
        ItemImage.Location.Collection => value.ItemLocation is ItemImage.Location.Deck,
        ItemImage.Location.Deck => value.ItemLocation is ItemImage.Location.Collection,
        _ => false
      };
    }

    public void OnDragReceived(Scope scope, ItemImage value)
    {
      var database = scope.Get(Key.Database);
      switch (value.ItemLocation)
      {
        case ItemImage.Location.Collection:
          database.Delete(TableId.Collection, Errors.CheckNotNull(value.ItemId));
          break;
        case ItemImage.Location.Deck:
          database.Delete(TableId.Deck, Errors.CheckNotNull(value.ItemId));
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      switch (ItemLocation)
      {
        case ItemImage.Location.Collection:
          database.Insert(TableId.Collection, (CreatureItemData) value.Item);
          break;
        case ItemImage.Location.Deck:
          database.Insert(TableId.Deck, (CreatureItemData) value.Item);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
}