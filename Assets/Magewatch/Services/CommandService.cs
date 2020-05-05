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
using Magewatch.Data;
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

    void Start()
    {
      _creatureService = Root.Instance.CreatureService;
      _assetService = Root.Instance.AssetService;
    }

    public void HandleCommands(CommandList commandList)
    {
      _currentCommandList = commandList;
      _currentStep = 0;
      StartCoroutine(RunCommandStep());
    }

    IEnumerator RunCommandStep()
    {
      _completionCount = 0;
      _expectedCompletions = 0;

      var actionCount = 0;
      foreach (var command in _currentCommandList.Steps[_currentStep].Commands)
      {
        actionCount++;
        if (command.DrawCard != null)
        {
          _expectedCompletions++;
          Root.Instance.User.Hand.DrawCard(command.DrawCard.Card, this);
        }

        if (command.CreateCreature != null)
        {
          _expectedCompletions++;
          var data = command.CreateCreature.Creature;
          _assetService.FetchCreatureAssets(data, () =>
          {
            var creature = _creatureService.Create(data);
            _creatureService.AddCreatureAtPosition(creature, data.RankPosition, data.FilePosition);
            OnComplete();
          });
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