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
using System.Collections.Generic;
using Nighthollow.Data;
using Nighthollow.Services;
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

    public Hand Hand => _hand;

    public UserData Data => _data;

    void Awake()
    {
      _data = Instantiate(_data);
    }

    void Start()
    {
      StartCoroutine(GainMana());
    }

    IEnumerator<YieldInstruction> GainMana()
    {
      while (true)
      {
        yield return new WaitForSeconds(_data.ManaGainIntervalMs.Value / 1000f);
        _data.Mana += _data.ManaGain.Value;
      }
    }

    public void DrawOpeningHand()
    {
      gameObject.SetActive(true);

      var openingHand = new List<CardData>();
      for (var i = 0; i < _data.StartingHandSize; ++i)
      {
        openingHand.Add(_deck.Draw());
      }

      _hand.DrawCards(openingHand);
    }

    void Update()
    {
      _lifeText.text = _data.Life.ToString();
      _manaText.text = _data.Mana.ToString();

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

          image.sprite = Root.Instance.Prefabs.SpriteForInfluenceType(school);
          image.transform.SetParent(_influenceRow);
          image.transform.localScale = Vector3.one;
          image.transform.localPosition = new Vector3(i * 100, 0, 0);
          index++;
        }
      }
    }
  }
}