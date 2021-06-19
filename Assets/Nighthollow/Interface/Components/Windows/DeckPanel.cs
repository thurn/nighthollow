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
using System.Collections.Immutable;
using System.Linq;
using Nighthollow.Data;
using Nighthollow.Interface.Components.Core;
using Nighthollow.Interface.Components.Library;
using Nighthollow.Rules;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface.Components.Windows
{
  public sealed record DeckPanel : LayoutComponent
  {
    public ImmutableList<KeyValuePair<int, CreatureItemData>> MainDeck { get; init; } =
      ImmutableList<KeyValuePair<int, CreatureItemData>>.Empty;

    public ImmutableList<KeyValuePair<int, CreatureItemData>> ManaDeck { get; init; } =
      ImmutableList<KeyValuePair<int, CreatureItemData>>.Empty;

    protected override BaseComponent OnRender(Scope scope)
    {
      var mainDeck = WithMinimumItems(MainDeck, 9);
      var manaDeck = WithMinimumItems(ManaDeck, 6);

      return new Column
      {
        AlignItems = Align.Stretch,
        JustifyContent = Justify.Center,
        FlexGrow = 1,
        Children = List(
          new Column
          {
            AlignItems = Align.Stretch,
            JustifyContent = Justify.Center,
            FlexGrow = 1,
            MarginLeftRight = 64,
            Children = List(
              MainDeckRow(mainDeck.Take(3)),
              MainDeckRow(mainDeck.Skip(3).Take(3)),
              MainDeckRow(mainDeck.Skip(6).Take(3))
            )
          },
          new Text("Mana")
          {
            TextAlign = TextAnchor.MiddleCenter,
            Type = TextType.Headline,
            HierarchyLevel = HierarchyLevel.Three
          },
          new Row
          {
            JustifyContent = Justify.Center,
            Children = manaDeck.Select(m => new ItemSlot
            {
              ItemLocation = ItemImage.Location.Deck,
              ItemId = m?.Key,
              Item = m?.Value,
              Size = ItemSlot.SlotSize.Small,
              Draggable = true
            })
          }
        )
      };
    }

    static ImmutableList<KeyValuePair<int, CreatureItemData>?> WithMinimumItems(
      ImmutableList<KeyValuePair<int, CreatureItemData>> items, int count)
    {
      var result = ImmutableList.CreateBuilder<KeyValuePair<int, CreatureItemData>?>();
      foreach (var c in items)
      {
        result.Add(c);
      }

      for (var i = items.Count; i < count; ++i)
      {
        result.Add(null);
      }

      return result.ToImmutable();
    }

    static Row MainDeckRow(IEnumerable<KeyValuePair<int, CreatureItemData>?> items) =>
      new()
      {
        FlexGrow = 1,
        JustifyContent = Justify.SpaceAround,
        Children = items.Select(m => new ItemSlot
        {
          ItemLocation = ItemImage.Location.Deck,
          ItemId = m?.Key,
          Item = m?.Value,
          Size = ItemSlot.SlotSize.Large,
          Draggable = true
        })
      };
  }
}