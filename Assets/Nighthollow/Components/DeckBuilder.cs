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

using Nighthollow.Services;
using UnityEngine;

namespace Nighthollow.Components
{
  public sealed class DeckBuilder : MonoBehaviour
  {
    [SerializeField] RectTransform _inventory;
    [SerializeField] RectTransform _deckMana;
    [SerializeField] RectTransform _deck;

    void OnEnable()
    {
      Redraw();
    }

    public void Redraw()
    {
      foreach (Transform child in _inventory)
      {
        Destroy(child.gameObject);
      }

      foreach (Transform child in _deck)
      {
        Destroy(child.gameObject);
      }

      foreach (Transform child in _deckMana)
      {
        Destroy(child.gameObject);
      }

      foreach (var card in InventoryService.Instance.Inventory)
      {
        Root.Instance.Prefabs.CreateCardRow(_inventory).Initialize(card, CardRow.ButtonAction.MoveToDeck, this);
      }

      foreach (var card in InventoryService.Instance.Deck)
      {
        var manaCreature = card.BaseCard.Creature.IsManaCreature;
        var row = Root.Instance.Prefabs.CreateCardRow(manaCreature ? _deckMana : _deck);
        row.Initialize(card, CardRow.ButtonAction.MoveToInventory, this);
        if (manaCreature)
        {
          row.GetComponent<RectTransform>().anchoredPosition = new Vector2(315, 75);
        }
      }
    }

    public void OnClose()
    {
      Destroy(gameObject);
    }
  }
}