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
using Nighthollow.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nighthollow.Components
{
  public sealed class CardRow : MonoBehaviour
  {
    static CardTooltip _tooltip;

    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] Text _buttonText;
    [SerializeField] Image _image;
    [SerializeField] CardItemData _card;
    [SerializeField] ButtonAction _action;
    [SerializeField] DeckBuilder _deckBuilder;

    public enum ButtonAction
    {
      MoveToDeck,
      MoveToInventory
    }

    public void Initialize(CardItemData card, ButtonAction action, DeckBuilder deckBuilder)
    {
      _text.text = card.BaseCard.Creature.Name;
      _image.sprite = card.BaseCard.Image;
      _card = card;
      _action = action;
      _deckBuilder = deckBuilder;

      switch (action)
      {
        case ButtonAction.MoveToDeck:
          _buttonText.text = "▶";
          break;
        case ButtonAction.MoveToInventory:
          _buttonText.text = "◀";
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(action), action, null);
      }
    }

    public void OnButtonClick()
    {
      var deck = Root.Instance.User.Deck;
      var inventory = Root.Instance.User.Inventory;
      switch (_action)
      {
        case ButtonAction.MoveToDeck:
          inventory.RemoveCard(_card);
          deck.AddCard(_card);
          break;
        case ButtonAction.MoveToInventory:
          deck.RemoveCard(_card);
          inventory.AddCard(_card);
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(_action), _action, null);
      }

      _deckBuilder.Redraw();
      Destroy(_tooltip.gameObject);
    }

    public void OnHover()
    {
      if (_tooltip)
      {
        Destroy(_tooltip.gameObject);
      }

      _tooltip = Root.Instance.Prefabs.CreateTooltip();
      _tooltip.GetComponent<RectTransform>().anchoredPosition =
        new Vector2(_action == ButtonAction.MoveToDeck ? 400 : -400, 0);
      _tooltip.Initialize(_card);
    }

    public void OnStopHovering()
    {
      if (_tooltip)
      {
        Destroy(_tooltip.gameObject);
      }
    }
  }
}