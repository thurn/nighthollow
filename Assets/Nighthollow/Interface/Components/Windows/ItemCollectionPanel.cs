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
using Nighthollow.Data;
using Nighthollow.Interface.Components.Core;
using Nighthollow.Interface.Components.Library;
using Nighthollow.Rules;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface.Components.Windows
{
  public sealed record ItemCollectionPanel : LayoutComponent
  {
    public ImmutableDictionary<int, CreatureItemData> Items { get; init; } =
      ImmutableDictionary<int, CreatureItemData>.Empty;

    protected override BaseComponent OnRender(Scope scope)
    {
      var children = ImmutableList.CreateBuilder<BaseComponent>();
      children.AddRange(Items.Select(pair => new ItemSlot
      {
        ItemLocation = ItemImage.Location.Collection,
        ItemId = pair.Key,
        Item = pair.Value,
        Draggable = true
      }));

      var i = children.Count;
      while (i < 20 || i % 5 != 0)
      {
        // Always add slots in multiples of 5 for visual symmetry
        children.Add(new ItemSlot
        {
          ItemLocation = ItemImage.Location.Collection
        });
        i++;
      }

      return new ScrollViewComponent
      {
        Width = Length.Percent(100),
        Height = Length.Percent(100),
        MarginLeftRight = 8,
        MarginTop = 8,
        MarginBottom = 32,
        VerticalScrollBar = new ScrollBar
        {
          Dragger = new ScrollElement
          {
            BackgroundColor = ColorPalette.PrimaryText
          },
          Tracker = new ScrollElement
          {
            BackgroundColor = ColorPalette.WindowBackground
          },
          HighButton = new ScrollElement
          {
            BackgroundColor = ColorPalette.WindowBackground
          },
          LowButton = new ScrollElement
          {
            BackgroundColor = ColorPalette.WindowBackground
          }
        },
        Children = List(new Row
        {
          AlignItems = Align.FlexStart,
          JustifyContent = Justify.Center,
          FlexWrap = Wrap.Wrap,
          Children = children
        })
      };
    }
  }
}