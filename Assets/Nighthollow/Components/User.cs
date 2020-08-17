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

using Nighthollow.Data;
using Nighthollow.Services;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nighthollow.Components
{
  public sealed class User : MonoBehaviour
  {
    [Header("Config")]
    [SerializeField] Hand _hand;
    [SerializeField] Deck _deck;
    [SerializeField] TextMeshProUGUI _lifeText;
    [SerializeField] TextMeshProUGUI _manaText;
    [SerializeField] RectTransform _influenceRow;

    [Header("State")]
    [SerializeField] UserData _data;
    [SerializeField] List<Image> _influenceImages;
    [SerializeField] int _life;
    [SerializeField] int _mana;

    public Hand Hand => _hand;
    public int Mana => _mana;
    public int Life => _life;
    public Influence Influence => _data.Influence;
    public UserData Data => _data;

    void Awake()
    {
      _data = Instantiate(_data);
    }

    void Start()
    {
      _life = _data.StartingLife.Value;
      _mana = _data.StartingMana.Value;
    }

    IEnumerator<YieldInstruction> GainMana()
    {
      while (true)
      {
        yield return new WaitForSeconds(_data.ManaGainIntervalMs.Value / 1000f);
        _mana += _data.ManaGain.Value;
      }
    }

    IEnumerator<YieldInstruction> DrawCards()
    {
      while (true)
      {
        yield return new WaitForSeconds(_data.CardDrawIntervalMs.Value / 1000f);
        _hand.DrawCards(new List<CardData> { _deck.Draw() });
      }
    }

    public void OnStartGame()
    {
      gameObject.SetActive(true);
      StartCoroutine(GainMana());
      StartCoroutine(DrawCards());

      var openingHand = new List<CardData>();
      for (var i = 0; i < _data.StartingHandSize.Value; ++i)
      {
        openingHand.Add(_deck.Draw());
      }

      _hand.DrawCards(openingHand);
    }

    public void SpendMana(int amount)
    {
      _mana -= amount;
    }

    void Update()
    {
      _lifeText.text = _life.ToString();
      _manaText.text = _mana.ToString();

      var index = 0;
      foreach (School school in Enum.GetValues(typeof(School)))
      {
        for (var i = 0; i < _data.Influence.Get(school).Value; ++i)
        {
          Image image;
          if (index < _influenceImages.Count)
          {
            image = _influenceImages[index];
          }
          else
          {
            image = Root.Instance.Prefabs.CreateInfluence();
            _influenceImages.Add(image);
          }

          image.gameObject.SetActive(true);
          image.sprite = Root.Instance.Prefabs.SpriteForInfluenceType(school);
          image.transform.SetParent(_influenceRow);
          image.transform.localScale = Vector3.one;
          image.transform.localPosition = new Vector3(i * 100, 0, 0);
          index++;
        }
      }

      while (index < _influenceImages.Count)
      {
        _influenceImages[index++].gameObject.SetActive(false);
      }
    }
  }
}
