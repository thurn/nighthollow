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
using Nighthollow.Generated;
using Nighthollow.Interface;
using Nighthollow.Model;
using Nighthollow.Services;
using Nighthollow.Utils;
using UnityEngine;

// ReSharper disable IteratorNeverReturns

#nullable enable

namespace Nighthollow.Components
{
  public sealed class User : MonoBehaviour
  {
    [SerializeField] Hand _hand = null!;

    [SerializeField] Deck _deck = null!;

    [SerializeField] int _mana;

    UserDataService _data = null!;

    UserStatus? _statusDisplay;
    public Hand Hand => _hand;
    public Deck Deck => _deck;
    public UserDataService Data => _data;

    public bool GameOver { get; private set; }

    public int Mana
    {
      get => _mana;
      private set => _mana = NumericUtils.Clamp(value, low: 0, high: 999);
    }

    void Update()
    {
      if (_statusDisplay != null)
      {
        _statusDisplay.Mana = Mana;
        _statusDisplay.Influence = _data.Stats.Get(Stat.Influence).Values;
      }
    }

    public void DrawOpeningHand(DataService data)
    {
      _data = data.UserData;
      var builtDeck = _data.Deck.Select(c => CreatureUtil.Build(_data.Stats, c)).ToList();
      _deck.OnStartGame(builtDeck, _data.TutorialState != UserDataService.Tutorial.Completed);

      var openingHand = new List<CreatureData>();
      for (var i = 0; i < Errors.CheckPositive(data.UserData.GetInt(Stat.StartingHandSize)); ++i)
      {
        openingHand.Add(_deck.Draw());
      }

      _hand.OverrideHandPosition(value: true);
      _hand.DrawCards(openingHand, OnDrewHand);
    }

    void OnDrewHand()
    {
      _hand.SetCardsToPreviewMode(value: true);
      ButtonUtil.DisplayMainButtons(Root.Instance.ScreenController,
        new List<ButtonUtil.Button>
        {
          new ButtonUtil.Button("Start Game!", () =>
          {
            _hand.SetCardsToPreviewMode(value: false);
            _hand.OverrideHandPosition(value: false, OnStartGame);
          }, large: true)
        });
      Root.Instance.HelperTextService.OnDrewOpeningHand();
    }

    public void OnStartGame()
    {
      gameObject.SetActive(value: true);
      Root.Instance.Enemy.OnGameStarted();
      Root.Instance.HelperTextService.OnGameStarted();
      StartCoroutine(GainMana());
      StartCoroutine(DrawCards());

      _statusDisplay = Root.Instance.ScreenController.Get(ScreenController.UserStatus);
      _statusDisplay.Show(animate: true);

      Mana = _data.GetInt(Stat.StartingMana);
    }

    public void SpendMana(int amount)
    {
      Mana -= amount;
    }

    public void OnGameOver()
    {
      GameOver = true;
    }

    IEnumerator<YieldInstruction> GainMana()
    {
      while (!GameOver)
      {
        yield return new WaitForSeconds(_data.GetDurationSeconds(Stat.ManaGainInterval));
        Mana += _data.GetInt(Stat.ManaGain);
      }
    }

    IEnumerator<YieldInstruction> DrawCards()
    {
      while (!GameOver)
      {
        Errors.CheckArgument(_data.GetDurationSeconds(Stat.CardDrawInterval) > 0.1f, "Card draw interval cannot be 0");
        yield return new WaitForSeconds(_data.GetDurationSeconds(Stat.CardDrawInterval));
        _hand.DrawCards(new List<CreatureData> {_deck.Draw()});
      }
    }
  }
}
