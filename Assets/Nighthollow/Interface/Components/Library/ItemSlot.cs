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
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface.Components.Library
{
  public sealed record ItemSlot : LayoutComponent
  {
    public enum SlotSize
    {
      Large,
      Small
    }

    public SlotSize Size { get; init; } = SlotSize.Large;
    public IItemData? Item { get; init; }
    public bool HasTooltip { get; init; } = true;

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
        Children = List(Item is null
          ? null
          : new ItemImage(Item)
          {
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
  }
}