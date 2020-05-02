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

using System;
using Magewatch.Data;
using UnityEngine;

namespace Magewatch.Components
{
  public sealed class Creature : MonoBehaviour
  {
    [SerializeField] Animator _animator;
    [SerializeField] CreatureState _state;
    [SerializeField] Vector2 _startPosition;
    [SerializeField] Vector2? _targetPosition;
    [SerializeField] float _percentageMoved;
    [SerializeField] float _moveDuration;

    static readonly int Idle1 = Animator.StringToHash("idle_1");
    static readonly int Walk = Animator.StringToHash("walk");
    static readonly int Run = Animator.StringToHash("run");

    void Start()
    {
      _animator = GetComponent<Animator>();
    }

    public void MoveToPosition(Vector2 position, float duration, bool shouldWalk = false)
    {
      SetState(shouldWalk ? CreatureState.Walking : CreatureState.Running);
      _startPosition = transform.position;
      _targetPosition = position;
      _percentageMoved = 0;
      _moveDuration = duration;
    }

    void Update()
    {
      if (Input.GetMouseButtonDown(0))
      {
        MoveToPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition), 5);
      }

      if (_targetPosition.HasValue)
      {
        _percentageMoved += Time.deltaTime / _moveDuration;
        if (_percentageMoved >= 1)
        {
          SetState(CreatureState.Idle);
          _targetPosition = null;
        }
        else
        {
          transform.position = Vector3.Lerp(_startPosition, _targetPosition.Value, _percentageMoved);
        }
      }
    }

    void SetState(CreatureState state)
    {
      if (_state != state)
      {
        switch (state)
        {
          case CreatureState.Idle:
            _animator.SetTrigger(Idle1);
            break;
          case CreatureState.Walking:
            _animator.SetTrigger(Walk);
            break;
          case CreatureState.Running:
            _animator.SetTrigger(Run);
            break;
          default:
            throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }

        _state = state;
      }
    }
  }
}