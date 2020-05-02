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

  enum AnimatorState
  {
    Idle = 1,
    Walk = 2,
    Run = 3,
    Death = 4,
    Hit1 = 5,
    Hit2 = 6,
    Skill1 = 7,
    Skill2 = 8,
    SKill3 = 9,
    Skill4 = 10,
    Skill5 = 11
  }

  public sealed class Creature : MonoBehaviour
  {
    [SerializeField] bool _debugMode;
    [SerializeField] int _debugCreatureId;
    [SerializeField] CreatureState _state;
    [SerializeField] AnimatorState _animatorState;
    [SerializeField] Animator _animator;
    [SerializeField] Collider2D _collider;
    [SerializeField] int _creatureId;
    [SerializeField] bool _initialized;
    [SerializeField] Creature _meleeTarget;
    [SerializeField] float _speed = 5;

    IOnComplete _onComplete;

    static readonly int State = Animator.StringToHash("State");

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
      _meleeTarget = target;
      _onComplete = onComplete;
      _state = CreatureState.MeleeEngage;
      _animatorState = Vector2.Distance(transform.position, _meleeTarget.transform.position) > 2
        ? AnimatorState.Run
        : AnimatorState.Walk;
    }

    void AttackStart()
    {
      Debug.Log($"Creature::AttackStart>");
    }

    void Update()
    {
      if (_meleeTarget)
      {
        if (_collider.IsTouching(_meleeTarget._collider))
        {
          _animatorState = AnimatorState.Idle;
          _state = CreatureState.Idle;
          _onComplete.OnComplete();
        }
        else
        {
          transform.position = Vector3.MoveTowards(
            transform.position,
            _meleeTarget.transform.position,
            _speed * Time.deltaTime);
        }
      }

      _animator.SetInteger(State, (int) _animatorState);
    }
  }
}