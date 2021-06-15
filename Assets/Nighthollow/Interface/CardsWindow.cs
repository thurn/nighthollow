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
  public sealed class CardsWindow : DefaultHideableElement, IDragManager<InterfaceItemImage, InterfaceItemSlot>
  {
    VisualElement _collection = null!;
    VisualElement _mainDeck = null!;
    VisualElement _manaDeck = null!;

    ImmutableList<InterfaceItemSlot> _collectionSlots = ImmutableList<InterfaceItemSlot>.Empty;
    ImmutableList<InterfaceItemSlot> _mainDeckSlots = ImmutableList<InterfaceItemSlot>.Empty;
    ImmutableList<InterfaceItemSlot> _manaSlots = ImmutableList<InterfaceItemSlot>.Empty;

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
      _collectionSlots = ItemRenderer2.AddItems(
        Registry,
        _collection,
        gameData.Collection.Values,
        new ItemRenderer2.Config(count: 20, dragManager: this));
      _mainDeckSlots = ItemRenderer2.AddItems(
        Registry,
        _mainDeck,
        gameData.Deck.Values
          .Where(creature => !gameData.CreatureTypes[creature.CreatureTypeId].IsManaCreature)
          .ToList(),
        new ItemRenderer2.Config(count: 9, dragManager: this));
      _manaSlots = ItemRenderer2.AddItems(
        Registry,
        _manaDeck,
        gameData.Deck.Values
          .Where(creature => gameData.CreatureTypes[creature.CreatureTypeId].IsManaCreature)
          .ToList(),
        new ItemRenderer2.Config(count: 6, InterfaceItemSlot.Size.Small));
    }

    public IEnumerable<InterfaceItemSlot> GetDragTargets(InterfaceItemImage element)
    {
      return _collectionSlots.Concat(_mainDeckSlots);
    }

    public void OnDragReceived(InterfaceItemSlot target, InterfaceItemImage element)
    {
    }
  }
}