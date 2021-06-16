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
using Nighthollow.Data;
using Nighthollow.Interface.Components.Core;
using Nighthollow.Rules;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface.Components.Library
{
  public sealed record ItemImage : LayoutComponent
  {
    public ItemImage(IItemData item)
    {
      Item = item;
    }

    public IItemData Item { get; }
    public ItemSlot.SlotSize Size { get; init; } = ItemSlot.SlotSize.Large;

    protected override BaseComponent OnRender(Scope scope)
    {
      var size = Size switch
      {
        ItemSlot.SlotSize.Large => 96,
        ItemSlot.SlotSize.Small => 80,
        _ => throw new ArgumentOutOfRangeException()
      };

      return new Column
      {
        BackgroundImage = Item.GetImageAddress(scope.Get(Key.GameData)),
        BackgroundScaleMode = ScaleMode.ScaleToFit,
        Width = size,
        Height = size,
        Children = List(
          Item.GetQuantity() is { } quantity and > 1
            ? new Text(quantity.ToString())
            {
              TextColor = ColorPalette.ItemQuantityNumber,
              Outline = true,
              FlexPosition = Position.Absolute,
              Right = 0,
              Bottom = 0
            }
            : null
        )
      };
    }
  }
}