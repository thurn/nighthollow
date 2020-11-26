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

#nullable enable

using System.Collections.Generic;
using System.Linq;
using Nighthollow.Data;
using Nighthollow.Generated;
using Nighthollow.Interface;
using Nighthollow.Services;
using Nighthollow.Utils;
using UnityEngine;

// ReSharper disable IteratorNeverReturns

namespace Nighthollow.Components
{
  public sealed class User : MonoBehaviour
  {
    [SerializeField] Hand _hand = null!;
    public Hand Hand => _hand;

    [SerializeField] Deck _deck = null!;
    public Deck Deck => _deck;

    [SerializeField] int _life;
    public int Life
    {
      get => _life;
      private set => _life = NumericUtils.Clamp(value, 0, 99);
    }

    [SerializeField] int _mana;

    public int Mana
    {
      get => _mana;
      private set => _mana = NumericUtils.Clamp(value, 0, 999);
    }

    UserStatus? _statusDisplay;

    public UserDataService Data => Database.Instance.UserData;

    public void DrawOpeningHand()
    {
      var builtDeck = Data.Deck.Select(c => CreatureUtil.Build(Data.Stats, c)).ToList();
      _deck.OnStartGame(builtDeck, orderedDraws: Data.TutorialState != UserDataService.Tutorial.Completed);

      var openingHand = new List<CreatureData>();
      for (var i = 0; i < Errors.CheckPositive(Data.GetInt(Stat.StartingHandSize)); ++i)
      {
        openingHand.Add(_deck.Draw());
      }

      _hand.SetPreviewMode(true);
      _hand.DrawCards(openingHand, OnDrewHand);
    }

    void OnDrewHand()
    {
      ButtonUtil.DisplayChoiceButtons(Root.Instance.ScreenController,
        new List<ButtonUtil.Button> {new ButtonUtil.Button("Start Game!", () =>
        {
          _hand.SetPreviewMode(false, OnStartGame);
        }, large: true)});
      Root.Instance.HelperTextService.OnDrewOpeningHand();
    }

    public void OnStartGame()
    {
      gameObject.SetActive(true);
      Root.Instance.HelperTextService.OnGameStarted();
      StartCoroutine(GainMana());
      StartCoroutine(DrawCards());

      _statusDisplay = Root.Instance.ScreenController.Get(ScreenController.UserStatus);
      _statusDisplay.Show(animate: true);

      Life = Errors.CheckPositive(Data.GetInt(Stat.StartingLife));
      Mana = Data.GetInt(Stat.StartingMana);
    }

    public void SpendMana(int amount)
    {
      Mana -= amount;
    }

    public void LoseLife(int amount)
    {
      Errors.CheckArgument(amount >= 0, "Cannot lose a negative amount of life");
      Life -= amount;
      if (Life == 0)
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
        Mana += Data.GetInt(Stat.ManaGain);
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
      if (_statusDisplay != null)
      {
        _statusDisplay.Life = Life;
        _statusDisplay.Mana = Mana;
        _statusDisplay.Influence = Data.Stats.Get(Stat.Influence).Values;
      }
    }
  }
}
