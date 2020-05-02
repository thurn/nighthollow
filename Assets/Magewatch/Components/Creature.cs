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

using Magewatch.Data;
using Magewatch.Services;
using Magewatch.Utils;
using UnityEngine;

namespace Magewatch.Components
{
  enum CreatureState
  {
    Idle,
    MeleeEngage,
    Attack
  }

  public sealed class Creature : MonoBehaviour
  {
    [SerializeField] bool _debugMode;
    [SerializeField] int _debugCreatureId;
    [SerializeField] CreatureState _state;
    [SerializeField] Animator _animator;
    [SerializeField] Collider2D _collider;
    [SerializeField] int _creatureId;
    [SerializeField] bool _initialized;
    [SerializeField] Creature _currentTarget;
    [SerializeField] float _speed = 2;
    [SerializeField] bool _touchedTarget;
    [SerializeField] Attack _currentAttack;

    IOnComplete _onComplete;

    static readonly int Speed = Animator.StringToHash("Speed");
    static readonly int Skill1 = Animator.StringToHash("Skill1");
    static readonly int Skill2 = Animator.StringToHash("Skill2");
    static readonly int Skill3 = Animator.StringToHash("Skill3");
    static readonly int Skill4 = Animator.StringToHash("Skill4");
    static readonly int Skill5 = Animator.StringToHash("Skill5");

    public void Initialize(int creatureId)
    {
      _creatureId = creatureId;
      _initialized = true;
    }

    void Start()
    {
      if (!_initialized)
      {
        if (_debugMode)
        {
          Initialize(_debugCreatureId);
          Root.Instance.CreatureService.Add(this);
        }
        else
        {
          throw Errors.MustInitialize(name);
        }
      }

      _animator = GetComponent<Animator>();
      _collider = GetComponent<Collider2D>();
    }

    public int CreatureId => _creatureId;

    public void MeleeEngageWithTarget(Creature target, IOnComplete onComplete)
    {
      _currentTarget = target;
      _onComplete = onComplete;
      _touchedTarget = false;
      _state = CreatureState.MeleeEngage;
    }

    public void AttackTarget(Creature target, Attack attack, IOnComplete onComplete)
    {
      _currentTarget = target;
      _currentAttack = attack;
      _onComplete = onComplete;
      _state = CreatureState.Attack;

      switch (attack.SkillNumber)
      {
        case Skill.Skill1:
          _animator.SetTrigger(Skill1);
          break;
        case Skill.Skill2:
          _animator.SetTrigger(Skill2);
          break;
        case Skill.Skill3:
          _animator.SetTrigger(Skill3);
          break;
        case Skill.Skill4:
          _animator.SetTrigger(Skill4);
          break;
        case Skill.Skill5:
          _animator.SetTrigger(Skill5);
          break;
        default:
          throw Errors.UnknownEnumValue(attack.SkillNumber);
      }
    }

    void AttackStart()
    {
      Debug.Log($"Creature::AttackStart>");
      _onComplete.OnComplete();
    }

    void Update()
    {
      if (_state == CreatureState.MeleeEngage)
      {
        if (_collider.IsTouching(_currentTarget._collider))
        {
          _animator.SetInteger(Speed, 0);
          if (!_touchedTarget)
          {
            _onComplete.OnComplete();
            _touchedTarget = true;
          }
        }
        else
        {
          _animator.SetInteger(Speed,
            Vector2.Distance(transform.position, _currentTarget.transform.position) > 2
              ? 2
              : 1);
          transform.position = Vector3.MoveTowards(
            transform.position,
            _currentTarget.transform.position,
            _speed * Time.deltaTime);
        }
      }
    }
  }
}