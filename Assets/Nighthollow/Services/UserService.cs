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
using System.Collections.Immutable;
using System.Linq;
using DataStructures.RandomSelector;
using JetBrains.Annotations;
using Nighthollow.Components;
using Nighthollow.Data;
using Nighthollow.Delegates.Handlers;
using Nighthollow.Interface;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Services
{
  public sealed class UserService : StatEntity
  {
    readonly UserStatus? _statusDisplay;
    readonly Hand _handComponent;
    readonly int _nextCardId;

    public UserService(
      Hand handComponent,
      GameData gameData)
    {
      _statusDisplay = null;
      _handComponent = handComponent;
      UserState = UserState.BuildUserData(gameData);
      Deck = BuildStartingDeck(gameData, UserState);
      Hand = ImmutableDictionary<int, CreatureData>.Empty;
      _nextCardId = 1;
    }

    UserService(
      UserStatus? statusDisplay,
      Hand handComponent,
      UserState userState,
      DeckData deck,
      ImmutableDictionary<int, CreatureData> hand,
      int nextCardId)
    {
      _statusDisplay = statusDisplay;
      _handComponent = handComponent;
      UserState = userState;
      Deck = deck;
      Hand = hand;
      _nextCardId = nextCardId;
    }

    public UserState UserState { get; }
    public override StatTable Stats => UserState.Stats;

    public DeckData Deck { get; }

    public ImmutableDictionary<int, CreatureData> Hand { get; }

    static DeckData BuildStartingDeck(GameData gameData, UserState state)
    {
      var cards =
        gameData.BattleData.UserDeckOverride.HasValue
          ? gameData.ItemLists[gameData.BattleData.UserDeckOverride.Value].Creatures
          : gameData.Deck.Values.ToImmutableList();
      return new DeckData(
        cards.Select(card => card.BuildCreature(gameData, state)).ToImmutableList(),
        orderedDraws: gameData.BattleData.UserDeckOverride.HasValue);
    }

    public sealed class Controller : ICardCallbacks
    {
      readonly GameServiceRegistry _registry;
      readonly GameServiceRegistry.IUserServiceMutator _mutator;

      public Controller(GameServiceRegistry registry, GameServiceRegistry.IUserServiceMutator mutator)
      {
        _registry = registry;
        _mutator = mutator;
      }

      public void OnUpdate()
      {
        var self = _registry.UserService;
        if (self._statusDisplay != null)
        {
          self._statusDisplay.Mana = self.UserState.Mana;
          self._statusDisplay.Influence = self.Get(Stat.Influence);
        }

        var userState = self.UserState.OnTick(_registry);
        var hand = self.Hand.ToImmutableDictionary(
          p => p.Key,
          p => p.Value.OnTick(_registry));

        _mutator.SetUserService(new UserService(
          self._statusDisplay,
          self._handComponent,
          userState,
          self.Deck,
          hand,
          self._nextCardId));

        foreach (var (cardId, card) in self._handComponent.Cards)
        {
          var manaCost = hand[cardId].GetInt(Stat.ManaCost);
          var influenceCost = hand[cardId].Get(Stat.InfluenceCost);
          var canPlay = manaCost <= userState.Mana &&
                        InfluenceUtil.LessThanOrEqualTo(influenceCost, userState.Get(Stat.Influence));
          card.OnUpdate(manaCost, influenceCost, canPlay);
        }
      }

      public void DrawOpeningHand()
      {
        Debug.Log($"Controller::DrawOpeningHand ");
        _registry.UserService._handComponent.OverrideHandPosition(true);
        DrawCards(_registry.UserService.GetInt(Stat.StartingHandSize), OnDrewHand);
      }

      void OnDrewHand()
      {
        Debug.Log($"Controller::OnDrewHand ");
        var hand = _registry.UserService._handComponent;
        hand.SetCardsToPreviewMode(value: true);
        ButtonUtil.DisplayMainButtons(Root.Instance.ScreenController,
          new List<ButtonUtil.Button>
          {
            new ButtonUtil.Button("Start Game!", () =>
            {
              hand.SetCardsToPreviewMode(value: false);
              hand.OverrideHandPosition(value: false, () => _registry.Invoke(new IOnStartBattle.Data()));
            }, large: true)
          });
        _registry.HelperTextService.OnDrewOpeningHand();
      }

      public void StartBattle()
      {
        Debug.Log($"Controller::StartBattle");
        var statusDisplay = _registry.ScreenController.Get(ScreenController.UserStatus);
        statusDisplay.Show(animate: true);
        _registry.HelperTextService.OnGameStarted();
        _registry.CoroutineRunner.StartCoroutine(GainManaCoroutine());
        _registry.CoroutineRunner.StartCoroutine(DrawCardCoroutine());

        var self = _registry.UserService;
        _mutator.SetUserService(new UserService(
          statusDisplay,
          self._handComponent,
          self.UserState,
          self.Deck,
          self.Hand,
          self._nextCardId));
      }

      public void GainMana(int amount)
      {
        Errors.CheckArgument(amount >= 0, "Mana gain must be non-negative");
        var self = _registry.UserService;
        _mutator.SetUserService(new UserService(
          self._statusDisplay,
          self._handComponent,
          self.UserState.WithMana(self.UserState.Mana + amount),
          self.Deck,
          self.Hand,
          self._nextCardId));
      }

      public void SpendMana(int amount)
      {
        Errors.CheckArgument(amount >= 0, "Mana spend must be non-negative");
        var self = _registry.UserService;
        Errors.CheckArgument(self.UserState.Mana - amount >= 0, "Mana spend must be >= total mana");
        _mutator.SetUserService(new UserService(
          self._statusDisplay,
          self._handComponent,
          self.UserState.WithMana(self.UserState.Mana - amount),
          self.Deck,
          self.Hand,
          self._nextCardId));
      }

      public void DrawCards(int count, Action? onComplete = null)
      {
        var userService = _registry.UserService;
        var deck = userService.Deck;
        var cardId = userService._nextCardId;
        var hand = userService.Hand.ToBuilder();
        for (var i = 0; i < Errors.CheckPositive(count); ++i)
        {
          deck = deck.DrawCard(out var card);
          hand[cardId] = card;
          cardId += 1;
        }

        _mutator.SetUserService(new UserService(
          userService._statusDisplay,
          userService._handComponent,
          userService.UserState,
          deck,
          hand.ToImmutable(),
          nextCardId: cardId + 1));

        userService._handComponent.SynchronizeHand(_registry, onComplete);
      }

      IEnumerator<YieldInstruction> GainManaCoroutine()
      {
        while (true)
        {
          yield return new WaitForSeconds(_registry.UserService.UserState.GetDurationSeconds(Stat.ManaGainInterval));
          GainMana(_registry.UserService.UserState.GetInt(Stat.ManaGain));
        }
        // ReSharper disable once IteratorNeverReturns
      }

      IEnumerator<YieldInstruction> DrawCardCoroutine()
      {
        while (true)
        {
          Errors.CheckArgument(
            _registry.UserService.UserState.GetDurationSeconds(Stat.CardDrawInterval) > 0.1f,
            "Card draw interval cannot be 0");
          yield return new WaitForSeconds(_registry.UserService.UserState.GetDurationSeconds(Stat.CardDrawInterval));
          DrawCards(1);
        }
        // ReSharper disable once IteratorNeverReturns
      }

      public void OnCardOverBoard(int cardId, Card card)
      {
        throw new NotImplementedException();
      }

      public void OnReturnToHand(int cardId)
      {
        throw new NotImplementedException();
      }

      public void OnCardPlayed(int cardId)
      {
        throw new NotImplementedException();
      }
    }

    public sealed class DeckData
    {
      readonly bool _orderedDraws;
      readonly int _lastDraw;
      readonly ImmutableList<CreatureData> _cards;
      readonly ImmutableList<int> _weights;

      public DeckData(ImmutableList<CreatureData> cards, bool orderedDraws) :
        this(orderedDraws, 0, cards, StartingWeights(cards))
      {
      }

      DeckData(bool orderedDraws, int lastDraw, ImmutableList<CreatureData> cards, ImmutableList<int> cardWeights)
      {
        Errors.CheckState(cards.Count > 0, "No cards in deck");
        _orderedDraws = orderedDraws;
        _lastDraw = lastDraw;
        _cards = cards;
        _weights = cardWeights;
      }

      [MustUseReturnValue] public DeckData DrawCard(out CreatureData card)
      {
        if (_orderedDraws)
        {
          card = _cards[_lastDraw % _cards.Count];
          return new DeckData(_orderedDraws, _lastDraw + 1, _cards, _weights);
        }

        var selector = new DynamicRandomSelector<int>(seed: -1, _cards.Count);
        for (var i = 0; i < _cards.Count; ++i)
        {
          selector.Add(i, _weights[i]);
        }

        selector.Build();

        var index = selector.SelectRandomItem();
        card = _cards[index];
        return DecrementWeight(index);
      }

      DeckData DecrementWeight(int index) => new DeckData(
        _orderedDraws,
        _lastDraw,
        _cards,
        _weights.SetItem(index,
          _weights[index] > 1000 ? _weights[index] - 1000 : _weights[index] / 2));

      static ImmutableList<int> StartingWeights(ImmutableList<CreatureData> cards)
      {
        var manaCreatureCount = cards.Count(c => c.GetBool(Stat.IsManaCreature));
        var manaCreatureWeight = 4000 * ((2.0 * cards.Count - manaCreatureCount) / 3.0);
        return cards
          .Select(card => card.GetBool(Stat.IsManaCreature) ? (int) Math.Round(manaCreatureWeight) : 4000)
          .ToImmutableList();
      }
    }
  }
}