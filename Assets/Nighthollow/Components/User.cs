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
using Nighthollow.Interface;
using Nighthollow.Data;
using Nighthollow.Services;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;

// ReSharper disable IteratorNeverReturns

#nullable enable

namespace Nighthollow.Components
{
  public sealed class User : MonoBehaviour, IStatOwner
  {
    [SerializeField] Hand _hand = null!;

    [SerializeField] Deck _deck = null!;

    [SerializeField] int _mana;

    UserStatus? _statusDisplay;
    public Hand Hand => _hand;
    public Deck Deck => _deck;

    public UserData Data { get; private set; } = null!;

    public bool GameOver { get; private set; }

    public int Mana
    {
      get => _mana;
      private set => _mana = NumericUtils.Clamp(value, low: 0, high: 999);
    }

    public void DrawOpeningHand(Database database)
    {
      var gameData = database.Snapshot();
      Data = gameData.GameState.BuildUserData(gameData);

      ImmutableList<CreatureItemData> cards;
      if (gameData.Deck.IsEmpty)
      {
        cards = gameData.ItemLists.Values.First(list => list.Name == StaticItemListName.StartingDeck).Creatures;
        database.InsertRange(TableId.Deck, cards);
      }
      else
      {
        cards = gameData.Deck.Values.ToImmutableList();
      }

      var builtDeck = cards.Select(item => item.BuildCreature(gameData, Data.Stats));
      _deck.OnStartGame(builtDeck, gameData.GameState.TutorialState != TutorialState.Completed);

      var openingHand = new List<CreatureData>();
      for (var i = 0; i < Errors.CheckPositive(Data.GetInt(Stat.StartingHandSize)); ++i)
      {
        openingHand.Add(_deck.Draw());
      }

      _hand.OverrideHandPosition(value: true);
      _hand.DrawCards(openingHand, () => OnDrewHand(gameData));
    }

    public void OnStartGame(GameData gameData)
    {
      gameObject.SetActive(value: true);
      Root.Instance.Enemy.OnGameStarted(gameData);
      Root.Instance.HelperTextService.OnGameStarted();
      StartCoroutine(GainMana());
      StartCoroutine(DrawCards());

      _statusDisplay = Root.Instance.ScreenController.Get(ScreenController.UserStatus);
      _statusDisplay.Show(animate: true);

      Mana = Data.GetInt(Stat.StartingMana);
    }

    void OnDrewHand(GameData gameData)
    {
      _hand.SetCardsToPreviewMode(value: true);
      ButtonUtil.DisplayMainButtons(Root.Instance.ScreenController,
        new List<ButtonUtil.Button>
        {
          new ButtonUtil.Button("Start Game!", () =>
          {
            _hand.SetCardsToPreviewMode(value: false);
            _hand.OverrideHandPosition(value: false, () => OnStartGame(gameData));
          }, large: true)
        });
      Root.Instance.HelperTextService.OnDrewOpeningHand();
    }

    public void InsertModifier(IStatModifier modifier)
    {
      Data = Data.WithStats(Data.Stats.InsertModifier(modifier));
    }

    void Update()
    {
      if (_statusDisplay != null)
      {
        _statusDisplay.Mana = Mana;
        _statusDisplay.Influence = Data.Stats.Get(Stat.Influence);
      }

      Data = Data?.OnTick();
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
        yield return new WaitForSeconds(Data.GetDurationSeconds(Stat.ManaGainInterval));
        Mana += Data.GetInt(Stat.ManaGain);
      }
    }

    IEnumerator<YieldInstruction> DrawCards()
    {
      while (!GameOver)
      {
        Errors.CheckArgument(Data.GetDurationSeconds(Stat.CardDrawInterval) > 0.1f, "Card draw interval cannot be 0");
        yield return new WaitForSeconds(Data.GetDurationSeconds(Stat.CardDrawInterval));
        _hand.DrawCards(new List<CreatureData> {_deck.Draw()});
      }
    }
  }
}
