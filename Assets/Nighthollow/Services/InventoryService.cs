using System.Collections.Generic;
using System.Linq;
using Nighthollow.Data;
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
      if (Instance)
      {
        Destroy(gameObject);
      }
      else
      {
        _inventory = _inventory.Select(c => c.Clone()).ToList();
        _deck = _deck.Select(c => c.Clone()).ToList();
        DontDestroyOnLoad(this);
        Instance = this;
      }
    }

    public void MoveToDeck(CardItemData card)
    {
      var existing = _deck.FindIndex(c => c.BaseCard.Creature.Name.Equals(card.BaseCard.Creature.Name));
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