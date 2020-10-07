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

#nullable enable

using System.Collections.Generic;
using System.Linq;
using Nighthollow.Model;
using UnityEngine;

namespace Nighthollow.Services
{
  public sealed class InventoryService : MonoBehaviour
  {
    [SerializeField] List<CardItemData> _inventory;
    public IReadOnlyCollection<CardItemData> Inventory => _inventory;

    [SerializeField] List<CardItemData> _deck;
    public IReadOnlyCollection<CardItemData> Deck => _deck;

    public static InventoryService Instance { get; private set; }

    void Awake()
    {
//      if (Instance)
//      {
//        Destroy(gameObject);
//      }
//      else
//      {
//        _inventory = _inventory.Select(c => c.Clone()).ToList();
//        _deck = _deck.Select(c => c.Clone()).ToList();
//        DontDestroyOnLoad(this);
//        Instance = this;
//      }
    }

    public void MoveToDeck(CardItemData card)
    {
      var existing = _deck.FindIndex(c => c.Card.Creature.Name.Equals(card.Card.Creature.Name));
      if (existing != -1)
      {
        AddToInventory(_deck[existing]);
        _deck.RemoveAt(existing);
      }

      _deck.Add(card);
      _inventory.Remove(card);
    }

    public void MoveToInventory(CardItemData card)
    {
      _deck.Remove(card);
      _inventory.Add(card);
    }

    public void AddToInventory(CardItemData card)
    {
      _inventory.Add(card);
    }
  }
}