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
using DG.Tweening;
using Magewatch.API;
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
      if (commandList.CommandGroups.Count > 0)
      {
        _assetService.FetchAssets(commandList, () =>
        {
          _currentCommandList = commandList;
          _currentStep = 0;
          RunCommandStep();
        });
      }
    }

    void RunCommandStep()
    {
      _completionCount = 0;
      _expectedCompletions = 0;
      var numUserCards = 0;
      var numEnemyCards = 0;
      var commandIndex = 0;

      foreach (var command in _currentCommandList.CommandGroups[_currentStep].Commands)
      {
        if (command.DisplayError != null)
        {
          Debug.LogError(command.DisplayError.Error);
          break;
        }

        if (command.Wait != null)
        {
          _expectedCompletions++;
          StartCoroutine(RunDelayed(command.Wait.WaitTimeMilliseconds / 1000f, OnComplete));
        }

        if (command.UpdateInterface != null)
        {
          var button = Root.Instance.MainButton;
          button.SetEnabled(command.UpdateInterface.MainButtonEnabled);
          button.SetText(command.UpdateInterface.MainButtonText);
        }

        if (command.DrawCard != null)
        {
          _expectedCompletions++;
          var owner = command.DrawCard.Card.Owner;
          StartCoroutine(RunDelayed(0.1f * (owner == PlayerName.User ? numUserCards : numEnemyCards), () =>
          {
            var player = Root.Instance.GetPlayer(owner);
            player.Hand.DrawCard(command.DrawCard.Card, this);
          }));

          switch (owner)
          {
            case PlayerName.User:
              numUserCards++;
              break;
            case PlayerName.Enemy:
              numEnemyCards++;
              break;
            default:
              throw Errors.UnknownEnumValue(owner);
          }
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

        if (command.CreateOrUpdateCreature != null)
        {
          _expectedCompletions++;
          var data = command.CreateOrUpdateCreature.Creature;
          if (_creatureService.HasCreature(data.CreatureId))
          {
            _creatureService.Get(data.CreatureId).UpdateCreatureData(data, this);
          }
          else
          {
            var creature = CreatureService.Create(data);
            creature.FadeIn(this);
          }
        }

        if (command.RemoveCreature != null)
        {
          _expectedCompletions++;
          var creature = _creatureService.Get(command.RemoveCreature.CreatureId);
          creature.FadeOut(this);
        }

        // if (command.MeleeEngage != null)
        // {
        //   HandleMeleeEngage(command.MeleeEngage);
        // }
        //
        // if (command.Attack != null)
        // {
        //   HandleAttack(commandIndex, command.Attack);
        // }

        if (command.UseCreatureSkill != null)
        {
          HandleUseCreatureSkill(commandIndex, command.UseCreatureSkill);
        }

        commandIndex++;
      }

      if (_expectedCompletions == 0)
      {
        OnComplete();
      }
    }

    IEnumerator<YieldInstruction> RunDelayed(float seconds, Action action)
    {
      if (seconds > 0)
      {
        yield return new WaitForSeconds(seconds);
      }

      action();
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
        if (rank != RankValue.RankUnspecified && file != FileValue.FileUnspecified)
        {
          var canvasPosition =
            ScreenUtils.WorldToCanvasPosition(new Vector2(rank.ToXPosition(owner), file.ToYPosition()));
          sequence.Insert(delay,
            card.transform.DOLocalMove(canvasPosition, 0.2f));
        }

        sequence.AppendCallback(OnComplete);
      });
    }

    // void HandleMeleeEngage(MeleeEngageCommand meleeEngage)
    // {
    //   _expectedCompletions++;
    //   _creatureService.Get(meleeEngage.CreatureId)
    //     .MeleeEngageWithTarget(_creatureService.Get(meleeEngage.TargetCreatureId), this);
    // }
    //
    // void HandleAttack(int commandNumber, AttackCommand attack)
    // {
    //   _expectedCompletions += attack.HitCount;
    //   StartCoroutine(RunDelayed(0.1f * commandNumber,
    //     () =>
    //     {
    //       _creatureService.Get(attack.CreatureId)
    //         .AttackTarget(_creatureService.Get(attack.TargetCreatureId), attack, this);
    //     }));
    // }

    void HandleUseCreatureSkill(int commandNumber, MUseCreatureSkillCommand command)
    {
      _expectedCompletions += command.Animation.ImpactCount;
      StartCoroutine(RunDelayed(0.1f * commandNumber, () =>
      {
        _creatureService.Get(command.SourceCreature)
          .UseSkill(command.Animation.Skill,
            command.OnImpact,
            command.MeleeTarget == null ? null : _creatureService.Get(command.MeleeTarget));
      }));
    }

    public void AddExpectedCompletion()
    {
      _expectedCompletions++;
    }

    public void OnComplete()
    {
      _completionCount++;

      if (_completionCount == _expectedCompletions)
      {
        _currentStep++;
        if (_currentStep < _currentCommandList.CommandGroups.Count)
        {
          RunCommandStep();
        }
      }
      else if (_completionCount > _expectedCompletions)
      {
        throw new InvalidOperationException("Unexpected OnComplete call");
      }
    }
  }
}