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
    [SerializeField] CreatureState _state;
    [SerializeField] Animator _animator;
    [SerializeField] Collider2D _collider;
    [SerializeField] Transform _healthbarAnchor;
    [SerializeField] HealthBar _healthBar;
    [SerializeField] CreatureData _creatureData;
    [SerializeField] bool _initialized;
    [SerializeField] Creature _currentTarget;
    [SerializeField] Transform _meleePosition;
    [SerializeField] float _speed = 2;
    [SerializeField] bool _touchedTarget;
    [SerializeField] AttackCommand _currentAttack;

    IOnComplete _onComplete;
    IOnComplete _onCompleteOnDeath;

    static readonly int Speed = Animator.StringToHash("Speed");
    static readonly int Skill1 = Animator.StringToHash("Skill1");
    static readonly int Skill2 = Animator.StringToHash("Skill2");
    static readonly int Skill3 = Animator.StringToHash("Skill3");
    static readonly int Skill4 = Animator.StringToHash("Skill4");
    static readonly int Skill5 = Animator.StringToHash("Skill5");
    static readonly int Death = Animator.StringToHash("Death");

    public void Initialize(CreatureData creatureData)
    {
      _creatureData = creatureData;
      _animator = GetComponent<Animator>();
      _collider = GetComponent<Collider2D>();
      _healthBar = Root.Instance.Prefabs.CreateHealthBar();
      _healthBar.gameObject.SetActive(false);
      if (creatureData.Owner == PlayerName.Enemy)
      {
        transform.eulerAngles = new Vector3(0, 180, 0);
      }

      _initialized = true;
    }

    void Start()
    {
      if (!_initialized && _debugMode)
      {
        Root.Instance.CreatureService.AddCreatureAtPosition(this,
          BoardPositions.ClosestRankForXPosition(transform.position.x, _creatureData.Owner),
          BoardPositions.ClosestFileForYPosition(transform.position.y));

        Initialize(_creatureData);
      }
    }

    public int CreatureId => _creatureData.CreatureId;

    public PlayerName Owner => _creatureData.Owner;

    public RankValue RankPosition => _creatureData.RankPosition;

    public FileValue FilePosition => _creatureData.FilePosition;

    public bool AnimationPaused
    {
      set => _animator.speed = value ? 0 : 1;
    }

    public bool ColliderContainsPoint(Vector2 point) => _collider.bounds.Contains(point);

    public void SetPosition(RankValue rankValue, FileValue fileValue)
    {
      _creatureData.RankPosition = rankValue;
      _creatureData.FilePosition = fileValue;
      transform.position = new Vector2(rankValue.ToXPosition(_creatureData.Owner), fileValue.ToYPosition());
    }

    public void MeleeEngageWithTarget(Creature target, IOnComplete onComplete)
    {
      _currentTarget = target;
      _onComplete = onComplete;
      _touchedTarget = false;
      _state = CreatureState.MeleeEngage;
    }

    public void AttackTarget(Creature target, AttackCommand attack, IOnComplete onComplete)
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

    public void OnDeathAnimationCompleted()
    {
      _onCompleteOnDeath?.OnComplete();
      Root.Instance.CreatureService.Destroy(_creatureData.CreatureId);
    }

    public void Destroy()
    {
      Destroy(gameObject);
      Destroy(_healthBar.gameObject);
    }

    void ApplyAttack(AttackCommand attack, IOnComplete onComplete)
    {
      _healthBar.Value -= attack.DamagePercent;
      _healthBar.gameObject.SetActive(_healthBar.Value < 100);
      if (_healthBar.Value <= 0)
      {
        _animator.SetTrigger(Death);
        _healthBar.gameObject.SetActive(false);
        _onCompleteOnDeath = onComplete;
      }
      else
      {
        onComplete.OnComplete();
      }
    }

    // Called by skill animations
    // ReSharper disable once UnusedMember.Local
    void AttackStart()
    {
      _currentTarget.ApplyAttack(_currentAttack, _onComplete);
      _onComplete = null;
    }

    Transform MeleePosition => _meleePosition;

    Creature CurrentTarget => _currentTarget;

    void Update()
    {
      var pos = Root.Instance.MainCamera.WorldToScreenPoint(_healthbarAnchor.position);
      _healthBar.transform.position = pos;

      if (_state == CreatureState.MeleeEngage)
      {
        // To prevent creatures from overlapping too much, in cases where a creature is *not* the primary target
        // of its opponent, we add a small Y offset to its target position to stagger the creatures in a more visually
        // pleasing way
        var yOffset = Vector3.zero;
        if (_currentTarget.CurrentTarget != this)
        {
          yOffset = new Vector3(0, new[] {0.5f, -0.5f, -0.3f, 0.3f}[CreatureId % 4], 0);
        }

        if (Vector2.Distance(MeleePosition.position, _currentTarget.MeleePosition.position + yOffset) < 0.05f)
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

          // Each creature has a desired "melee position" in front of it indicating where attackers should be placed.
          // We translate the creature's position until its own melee position matches its target's melee position.
          var meleePositionOffset = transform.position - MeleePosition.position;

          transform.position = Vector3.MoveTowards(
            transform.position,
            _currentTarget.MeleePosition.position + meleePositionOffset + yOffset,
            _speed * Time.deltaTime);
        }
      }
    }
  }
}