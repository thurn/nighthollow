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
      RunCombatStep();
    }

    void RunCombatStep()
    {
      Debug.Log($"CombatService::RunCombatStep> Running combat step {_currentCombatStep}");
      _completionCount = 0;
      _expectedCompletions = 0;

      foreach (var action in _currentCommand.Steps[_currentCombatStep].Actions)
      {
        if (action.MeleeEngage != null)
        {
          HandleMeleeEngage(action.MeleeEngage);
        }

        if (action.Attack != null)
        {
          HandleAttack(action.Attack);
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
      Debug.Log($"CombatService::OnComplete> expected {_expectedCompletions}");
      _completionCount++;

      if (_completionCount >= _expectedCompletions)
      {
        Debug.Log($"CombatService::OnComplete> incrementing step");
        _currentCombatStep++;
        if (_currentCombatStep < _currentCommand.Steps.Count)
        {
          Debug.Log($"CombatService::OnComplete> running");
          RunCombatStep();
        }
        else
        {
          Debug.Log($"CombatService::OnComplete> Done running combat");
        }
      }
    }
  }
}