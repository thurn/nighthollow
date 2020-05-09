// Copyright The Magewatch Project

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//    https://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections;
using DG.Tweening;
using Magewatch.Data;
using Magewatch.Utils;
using UnityEngine;

namespace Magewatch.Services
{
  public interface IOnComplete
  {
    void OnComplete();
  }

  public sealed class CommandService : MonoBehaviour, IOnComplete
  {
    [SerializeField] CreatureService _creatureService;
    [SerializeField] AssetService _assetService;
    [SerializeField] CommandList _currentCommandList;
    [SerializeField] int _currentStep;
    [SerializeField] int _expectedCompletions;
    [SerializeField] int _completionCount;

    void Awake()
    {
      _creatureService = Root.Instance.CreatureService;
      _assetService = Root.Instance.AssetService;
    }

    public void HandleCommands(CommandList commandList)
    {
      _assetService.FetchAssets(commandList, () =>
      {
        _currentCommandList = commandList;
        _currentStep = 0;
        StartCoroutine(RunCommandStep());
      });
    }

    IEnumerator RunCommandStep()
    {
      _completionCount = 0;
      _expectedCompletions = 0;

      var actionCount = 0;
      foreach (var command in _currentCommandList.Steps[_currentStep].Commands)
      {
        actionCount++;
        if (command.Wait != null)
        {
          _expectedCompletions++;
          yield return new WaitForSeconds(command.Wait.WaitTimeMilliseconds / 1000f);
          OnComplete();
        }

        if (command.DrawCard != null)
        {
          _expectedCompletions++;
          var player = Root.Instance.GetPlayer(command.DrawCard.Card.Owner);
          player.Hand.DrawCard(command.DrawCard.Card, this);
        }

        if (command.PlayCard != null)
        {
          HandlePlayCard(command);
        }

        if (command.UpdatePlayer != null)
        {
          _expectedCompletions++;
          Root.Instance.GetPlayer(command.UpdatePlayer.Player.PlayerName)
            .UpdatePlayerData(command.UpdatePlayer.Player, this);
        }

        if (command.CreateCreature != null)
        {
          _expectedCompletions++;
          var data = command.CreateCreature.Creature;
          var creature = CreatureService.Create(data);
          creature.FadeIn(this);
        }

        if (command.RemoveCreature != null)
        {
          _expectedCompletions++;
          var creature = _creatureService.Get(command.RemoveCreature.CreatureId);
          creature.FadeOut(this);
        }

        if (command.MeleeEngage != null)
        {
          HandleMeleeEngage(command.MeleeEngage);
        }

        if (command.Attack != null)
        {
          HandleAttack(command.Attack);
          yield return new WaitForSeconds(actionCount * 0.1f);
        }
      }

      if (_expectedCompletions == 0)
      {
        OnComplete();
      }
    }

    void HandlePlayCard(Command command)
    {
      _expectedCompletions++;
      var owner = command.PlayCard.Card.Owner;
      var player = Root.Instance.GetPlayer(owner);
      var rank = command.PlayCard.RankPosition;
      var file = command.PlayCard.FilePosition;
      var delay = 2.0f;
      if (command.PlayCard.RevealDelayMilliseconds > 0)
      {
        delay = command.PlayCard.RevealDelayMilliseconds / 1000f;
      }

      player.Hand.RevealMatchingCard(command.PlayCard.Card, card =>
      {
        var sequence = DOTween.Sequence();
        sequence.Insert(delay, card.transform.DOScale(0, 0.2f));
        if (rank != RankValue.Unknown && file != FileValue.Unknown)
        {
          var canvasPosition =
            ScreenUtils.WorldToCanvasPosition(new Vector2(rank.ToXPosition(owner), file.ToYPosition()));
          sequence.Insert(delay,
            card.transform.DOLocalMove(canvasPosition, 0.2f));
        }

        sequence.AppendCallback(OnComplete);
      });
    }

    void HandleMeleeEngage(MeleeEngageCommand meleeEngage)
    {
      _expectedCompletions++;
      _creatureService.Get(meleeEngage.CreatureId)
        .MeleeEngageWithTarget(_creatureService.Get(meleeEngage.TargetCreatureId), this);
    }

    void HandleAttack(AttackCommand attack)
    {
      _expectedCompletions += attack.HitCount;
      _creatureService.Get(attack.CreatureId).AttackTarget(_creatureService.Get(attack.TargetCreatureId), attack, this);
    }

    public void OnComplete()
    {
      _completionCount++;

      if (_completionCount >= _expectedCompletions)
      {
        _currentStep++;
        if (_currentStep < _currentCommandList.Steps.Count)
        {
          StartCoroutine(RunCommandStep());
        }
      }
    }
  }
}