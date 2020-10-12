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
using Nighthollow.Data;
using Nighthollow.Generated;
using Nighthollow.Services;
using Nighthollow.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ReSharper disable IteratorNeverReturns

namespace Nighthollow.Components
{
  public sealed class User : MonoBehaviour
  {
    [Header("Config")]
    [SerializeField] Hand _hand;
    public Hand Hand => _hand;

    [SerializeField] Deck _deck;
    public Deck Deck => _deck;

    [SerializeField] TextMeshProUGUI _lifeText;
    [SerializeField] TextMeshProUGUI _manaText;
    [SerializeField] RectTransform _influenceRow;

    [Header("State")]
    UserData _data;
    public UserData Data => _data;

    [SerializeField] List<Image> _influenceImages;
    [SerializeField] int _life;
    [SerializeField] int _mana;

    public int Mana => _mana;
    public int Life => _life;

    public void OnStartGame(UserData data)
    {
      _data = data;
      gameObject.SetActive(true);
      StartCoroutine(GainMana());
      StartCoroutine(DrawCards());

      _life = _data.GetInt(Stat.StartingLife);
      _mana = _data.GetInt(Stat.StartingMana);
      _deck.OnStartGame(data.Deck);

      var openingHand = new List<CreatureData>();
      for (var i = 0; i < _data.GetInt(Stat.StartingHandSize); ++i)
      {
        openingHand.Add(_deck.Draw());
      }

      _hand.DrawCards(openingHand);
    }

    public void SpendMana(int amount)
    {
      _mana -= amount;
    }

    public void LoseLife(int amount)
    {
      Errors.CheckArgument(amount >= 0, "Cannot lose a negative amount of life");
      _life -= amount;
      if (_life <= 0)
      {
        Root.Instance.Prefabs.ShowDialog("Game Over", () => { SceneManager.LoadScene("Main", LoadSceneMode.Single); });
      }
      else
      {
        Root.Instance.Enemy.OnEnemyCreatureRemoved();
      }
    }

    IEnumerator<YieldInstruction> GainMana()
    {
      while (true)
      {
        yield return new WaitForSeconds(_data.GetDurationSeconds(Stat.ManaGainInterval));
        _mana += Mathf.RoundToInt(_data.GetInt(Stat.ManaGain));
      }
    }

    IEnumerator<YieldInstruction> DrawCards()
    {
      while (true)
      {
        Errors.CheckArgument(_data.GetDurationSeconds(Stat.CardDrawInterval) > 0.1f, "Card draw interval cannot be 0");
        yield return new WaitForSeconds(_data.GetDurationSeconds(Stat.CardDrawInterval));
        _hand.DrawCards(new List<CreatureData> {_deck.Draw()});
      }
    }

    void Update()
    {
      _lifeText.text = _life.ToString();
      _manaText.text = _mana.ToString();

      var index = 0;
      foreach (var pair in _data.Stats.Get(Stat.Influence).AllEntries)
      {
        for (var i = 0; i < pair.Value.Value; ++i)
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
          image.sprite = Root.Instance.Prefabs.SpriteForInfluenceType(pair.Key);
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