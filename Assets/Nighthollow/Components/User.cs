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
    GameServiceRegistry? _registry;

    public Hand Hand => _hand;
    public Deck Deck => _deck;

    public UserState State { get; private set; } = null!;

    public bool GameOver { get; private set; }

    public int Mana
    {
      get => _mana;
      private set => _mana = NumericUtils.Clamp(value, low: 0, high: 999);
    }

    public void DrawOpeningHand(GameServiceRegistry registry)
    {
      Debug.Log($"User::DrawOpeningHand");
      _registry = registry;
      var gameData = registry.Database.Snapshot();
      State = UserState.BuildUserData(gameData);

      var cards =
        gameData.BattleData.UserDeckOverride.HasValue
          ? gameData.ItemLists[gameData.BattleData.UserDeckOverride.Value].Creatures
          : gameData.Deck.Values.ToImmutableList();
      Errors.CheckState(cards.Count > 0, "No cards in deck");

      var builtDeck = cards.Select(item => item.BuildCreatureTemp(registry));
      _deck.OnStartGame(builtDeck, orderedDraws: gameData.BattleData.UserDeckOverride.HasValue);

      var openingHand = new List<CreatureData>();
      for (var i = 0; i < Errors.CheckPositive(State.GetInt(Stat.StartingHandSize)); ++i)
      {
        openingHand.Add(_deck.Draw());
      }

      _hand.OverrideHandPosition(value: true);
      // _hand.DrawCards(_registry, openingHand, () => OnDrewHand(registry));
    }

    void OnDrewHand(GameServiceRegistry registry)
    {
      Debug.Log($"User::OnDrewHand ");
      _hand.SetCardsToPreviewMode(value: true);
      ButtonUtil.DisplayMainButtons(Root.Instance.ScreenController,
        new List<ButtonUtil.Button>
        {
          new ButtonUtil.Button("Start Game!", () =>
          {
            _hand.SetCardsToPreviewMode(value: false);
            _hand.OverrideHandPosition(value: false, () => OnStartGame(registry));
          }, large: true)
        });
      Root.Instance.HelperTextService.OnDrewOpeningHand();
    }

    public void OnStartGame(GameServiceRegistry registry)
    {
      Debug.Log($"User::OnStartGame");
      gameObject.SetActive(value: true);
      Root.Instance.Enemy.OnGameStarted(registry);
      Root.Instance.HelperTextService.OnGameStarted();
      StartCoroutine(GainMana());
      StartCoroutine(DrawCards());

      _statusDisplay = Root.Instance.ScreenController.Get(ScreenController.UserStatus);
      _statusDisplay.Show(animate: true);

      Mana = State.GetInt(Stat.StartingMana);
    }

    public void InsertModifier(IStatModifier modifier)
    {
      State = State.WithStats(State.Stats.InsertModifier(modifier));
    }

    public void InsertStatusEffect(StatusEffectData statusEffectData)
    {
      State = State.WithStats(State.Stats.InsertStatusEffect(statusEffectData));
    }

    void Update()
    {
      if (_statusDisplay != null)
      {
        _statusDisplay.Mana = Mana;
        _statusDisplay.Influence = State.Stats.Get(Stat.Influence);
      }

      if (_registry != null)
      {
        State = State.OnTick(_registry);
      }
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
        yield return new WaitForSeconds(State.GetDurationSeconds(Stat.ManaGainInterval));
        Mana += State.GetInt(Stat.ManaGain);
      }
    }

    IEnumerator<YieldInstruction> DrawCards()
    {
      while (!GameOver)
      {
        Errors.CheckArgument(State.GetDurationSeconds(Stat.CardDrawInterval) > 0.1f, "Card draw interval cannot be 0");
        yield return new WaitForSeconds(State.GetDurationSeconds(Stat.CardDrawInterval));
        // _hand.DrawCards(_registry!, new List<CreatureData> {_deck.Draw()});
      }
    }
  }
}