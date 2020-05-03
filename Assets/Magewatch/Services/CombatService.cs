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

  public sealed class CombatService : MonoBehaviour, IOnComplete
  {
    [SerializeField] CreatureService _creatureService;
    [SerializeField] RunCombatCommand _currentCommand;
    [SerializeField] int _currentCombatStep;
    [SerializeField] int _expectedCompletions;
    [SerializeField] int _completionCount;

    void Start()
    {
      _creatureService = Root.Instance.CreatureService;
    }

    public void RunCombat(RunCombatCommand runCombatCommand)
    {
      _currentCommand = runCombatCommand;
      _currentCombatStep = 0;
      StartCoroutine(RunCombatStep());
    }

    IEnumerator RunCombatStep()
    {
      _completionCount = 0;
      _expectedCompletions = 0;

      var actionCount = 0;
      foreach (var action in _currentCommand.Steps[_currentCombatStep].Actions)
      {
        actionCount++;
        if (action.MeleeEngage != null)
        {
          HandleMeleeEngage(action.MeleeEngage);
        }

        if (action.Attack != null)
        {
          HandleAttack(action.Attack);
          yield return new WaitForSeconds(actionCount * 0.1f);
        }
      }
    }

    void HandleMeleeEngage(MeleeEngage meleeEngage)
    {
      _expectedCompletions++;
      _creatureService.Get(meleeEngage.CreatureId)
        .MeleeEngageWithTarget(_creatureService.Get(meleeEngage.TargetCreatureId), this);
    }

    void HandleAttack(Attack attack)
    {
      _expectedCompletions += attack.HitCount;
      _creatureService.Get(attack.CreatureId).AttackTarget(_creatureService.Get(attack.TargetCreatureId), attack, this);
    }

    public void OnComplete()
    {
      _completionCount++;

      if (_completionCount >= _expectedCompletions)
      {
        _currentCombatStep++;
        if (_currentCombatStep < _currentCommand.Steps.Count)
        {
          StartCoroutine(RunCombatStep());
        }
      }
    }
  }
}