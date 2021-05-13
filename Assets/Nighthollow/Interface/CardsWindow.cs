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
using Nighthollow.Items;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public sealed class CardsWindow : DefaultHideableElement, IDragManager<ItemImage, ItemSlot>
  {
    VisualElement _collection = null!;
    VisualElement _mainDeck = null!;
    VisualElement _manaDeck = null!;

    ImmutableList<ItemSlot> _collectionSlots = ImmutableList<ItemSlot>.Empty;
    ImmutableList<ItemSlot> _mainDeckSlots = ImmutableList<ItemSlot>.Empty;
    ImmutableList<ItemSlot> _manaSlots = ImmutableList<ItemSlot>.Empty;

    public override bool ExclusiveFocus => true;

    public new sealed class UxmlFactory : UxmlFactory<CardsWindow, UxmlTraits>
    {
    }

    protected override void Initialize()
    {
      this.Q("CloseButton").RegisterCallback<ClickEvent>(e => { Hide(); });
      _collection = this.Q("Collection");
      _mainDeck = this.Q("MainDeck");
      _manaDeck = this.Q("ManaDeck");
    }

    protected override void OnShow()
    {
      var gameData = Registry.Database.Snapshot();
      _collectionSlots = ItemRenderer.AddItems(
        Registry,
        _collection,
        gameData.Collection.Values,
        new ItemRenderer.Config(count: 20, dragManager: this));
      _mainDeckSlots = ItemRenderer.AddItems(
        Registry,
        _mainDeck,
        gameData.Deck.Values
          .Where(creature => !gameData.CreatureTypes[creature.CreatureTypeId].IsManaCreature)
          .ToList(),
        new ItemRenderer.Config(count: 9, dragManager: this));
      _manaSlots = ItemRenderer.AddItems(
        Registry,
        _manaDeck,
        gameData.Deck.Values
          .Where(creature => gameData.CreatureTypes[creature.CreatureTypeId].IsManaCreature)
          .ToList(),
        new ItemRenderer.Config(count: 6, ItemSlot.Size.Small));
    }

    public IEnumerable<ItemSlot> GetDragTargets(ItemImage element)
    {
      return _collectionSlots.Concat(_mainDeckSlots);
    }

    public void OnDragReceived(ItemSlot target, ItemImage element)
    {
    }
  }
}