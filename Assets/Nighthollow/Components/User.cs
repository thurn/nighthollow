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
using System.Linq;
using Nighthollow.Data;
using Nighthollow.Generated;
using Nighthollow.Interface;
using Nighthollow.Services;
using Nighthollow.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable IteratorNeverReturns

namespace Nighthollow.Components
{
  public sealed class User : MonoBehaviour
  {
#pragma warning disable 0649
    [SerializeField] Hand _hand;
    public Hand Hand => _hand;

    [SerializeField] Deck _deck;
    public Deck Deck => _deck;

    [SerializeField] TextMeshProUGUI _lifeText;
    [SerializeField] TextMeshProUGUI _manaText;
    [SerializeField] RectTransform _influenceRow;

    [SerializeField] List<Image> _influenceImages;
    [SerializeField] int _life;
    [SerializeField] int _mana;
#pragma warning restore 0649

    public UserDataService Data => Database.Instance.UserData;
    public int Mana => _mana;
    public int Life => _life;

    public void DrawOpeningHand()
    {
      var builtDeck = Data.Deck.Select(c => CreatureUtil.Build(Data.Stats, c)).ToList();
      _deck.OnStartGame(builtDeck, orderedDraws: Data.TutorialState != UserDataService.Tutorial.Completed);

      var openingHand = new List<CreatureData>();
      for (var i = 0; i < Errors.CheckPositive(Data.GetInt(Stat.StartingHandSize)); ++i)
      {
        openingHand.Add(_deck.Draw());
      }

      _hand.PreviewMode = true;
      _hand.DrawCards(openingHand, OnDrewHand);
    }

    void OnDrewHand()
    {
      ButtonUtil.DisplayChoiceButtons(Root.Instance.ScreenController,
        new List<ButtonUtil.Button> {new ButtonUtil.Button("Start Game!", OnStartGame, large: true)});
      Root.Instance.HelperTextService.OnDrewOpeningHand();
    }

    public void OnStartGame()
    {
      gameObject.SetActive(true);
      StartCoroutine(GainMana());
      StartCoroutine(DrawCards());

      _hand.PreviewMode = false;
      _life = Errors.CheckPositive(Data.GetInt(Stat.StartingLife));
      _mana = Data.GetInt(Stat.StartingMana);
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
        Debug.Log("Game Over");
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
        yield return new WaitForSeconds(Data.GetDurationSeconds(Stat.ManaGainInterval));
        _mana += Mathf.RoundToInt(Data.GetInt(Stat.ManaGain));
      }
    }

    IEnumerator<YieldInstruction> DrawCards()
    {
      while (true)
      {
        Errors.CheckArgument(Data.GetDurationSeconds(Stat.CardDrawInterval) > 0.1f, "Card draw interval cannot be 0");
        yield return new WaitForSeconds(Data.GetDurationSeconds(Stat.CardDrawInterval));
        _hand.DrawCards(new List<CreatureData> {_deck.Draw()});
      }
    }

    void Update()
    {
      _lifeText.text = _life.ToString();
      _manaText.text = _mana.ToString();

      var index = 0;
      foreach (var pair in Data.Stats.Get(Stat.Influence).Values)
      {
        for (var i = 0; i < pair.Value; ++i)
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
