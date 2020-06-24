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
using System.Linq;
using Nighthollow.Model;
using Nighthollow.Services;
using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.Rendering;

namespace Nighthollow.Components
{
  public enum CreatureState
  {
    Placing,
    Idle,
    Moving,
    UsingSkill,
    Dying
  }

  public sealed class Creature : MonoBehaviour
  {
    [Header("Config")] [SerializeField] bool _debugMode;
    [SerializeField] Transform _projectileSource;
    [SerializeField] AttachmentDisplay _attachmentDisplay;
    [SerializeField] Transform _healthbarAnchor;

    [Header("State")] [SerializeField] bool _initialized;
    [SerializeField] CreatureData _creatureData;
    [SerializeField] CreatureState _state;
    [SerializeField] SkillType _currentSkillType;
    [SerializeField] RankValue? _rankPosition;
    [SerializeField] FileValue? _filePosition;
    [SerializeField] Animator _animator;
    [SerializeField] Collider2D _collider;
    [SerializeField] HealthBar _healthBar;
    [SerializeField] CreatureService _creatureService;
    [SerializeField] SortingGroup _sortingGroup;

    static readonly Collider2D[] ColliderArray = Enumerable.Repeat<Collider2D>(null, 128).ToArray();

    static readonly int Skill1 = Animator.StringToHash("Skill1");
    static readonly int Skill2 = Animator.StringToHash("Skill2");
    static readonly int Skill3 = Animator.StringToHash("Skill3");
    static readonly int Skill4 = Animator.StringToHash("Skill4");
    static readonly int Skill5 = Animator.StringToHash("Skill5");
    static readonly int Death = Animator.StringToHash("Death");
    static readonly int Moving = Animator.StringToHash("Moving");
    static readonly int AnimationSpeed = Animator.StringToHash("AnimationSpeed");

    public void Initialize(CreatureData creatureData)
    {
      _creatureService = Root.Instance.CreatureService;
      _state = CreatureState.Placing;
      _animator = GetComponent<Animator>();
      _collider = GetComponent<Collider2D>();
      _collider.enabled = false;
      _sortingGroup = GetComponent<SortingGroup>();
      _healthBar = Root.Instance.Prefabs.CreateHealthBar();
      _healthBar.gameObject.SetActive(false);
      gameObject.layer = Constants.LayerForPlayerCreatures(creatureData.Owner);

      // Slow down enemy animations a bit
      _animator.SetFloat(AnimationSpeed, creatureData.Owner == PlayerName.User ? 1.0f : 0.5f);

      UpdateCreatureData(creatureData);
      _initialized = true;
    }

    void Start()
    {
      if (!_initialized && _debugMode)
      {
        Initialize(_creatureData);
        _creatureService.AddUserCreatureAtPosition(this,
          BoardPositions.ClosestRankForXPosition(transform.position.x),
          BoardPositions.ClosestFileForYPosition(transform.position.y));
      }
    }

    public CreatureId CreatureId => _creatureData.CreatureId;

    public PlayerName Owner => _creatureData.Owner;

    public RankValue? RankPosition => _rankPosition;

    public FileValue FilePosition
    {
      get
      {
        if (_filePosition.HasValue)
        {
          return _filePosition.Value;
        }

        throw new InvalidOperationException("Creature does not have a file position");
      }
    }

    public Transform ProjectileSource => _projectileSource;

    public bool AnimationPaused
    {
      set => _animator.speed = value ? 0 : 1;
    }

    public void UpdateCreatureData(CreatureData newData)
    {
      if (_state == CreatureState.Dying) return;

      transform.eulerAngles = newData.Owner == PlayerName.Enemy ? new Vector3(0, 180, 0) : Vector3.zero;

      _healthBar.Value = newData.HealthPercent;
      _healthBar.gameObject.SetActive(_healthBar.Value < 1);

      if (newData.Attachments != null)
      {
        _attachmentDisplay.SetAttachments(newData.Attachments);
      }

      _creatureData = newData;
    }

    public void ActivateEnemyCreature(FileValue fileValue)
    {
      _filePosition = fileValue;
      _state = _creatureData.Speed > 0 ? CreatureState.Moving : CreatureState.Idle;
      transform.position = new Vector2(
        Constants.EnemyCreatureStartingX,
        // We need to offset the Y position for enemy creatures because they are center-anchored:
        fileValue.ToYPosition() + Constants.EnemyCreatureYOffset);
      _collider.enabled = true;
    }

    public void ActivateUserCreature(RankValue rankValue, FileValue fileValue)
    {
      _rankPosition = rankValue;
      _filePosition = fileValue;
      _state = _creatureData.Speed > 0 ? CreatureState.Moving : CreatureState.Idle;
      transform.position = new Vector2(
        rankValue.ToXPosition(),
        fileValue.ToYPosition());
      _collider.enabled = true;
    }

    public void UseSkill(SkillAnimationNumber skill, SkillType skillType)
    {
      if (!IsAlive())
      {
        throw new InvalidOperationException("Attempted to use skill on non-living creature");
      }

      _state = CreatureState.UsingSkill;
      _currentSkillType = skillType;

      switch (skill)
      {
        case SkillAnimationNumber.Skill1:
          _animator.SetTrigger(Skill1);
          break;
        case SkillAnimationNumber.Skill2:
          _animator.SetTrigger(Skill2);
          break;
        case SkillAnimationNumber.Skill3:
          _animator.SetTrigger(Skill3);
          break;
        case SkillAnimationNumber.Skill4:
          _animator.SetTrigger(Skill4);
          break;
        case SkillAnimationNumber.Skill5:
          _animator.SetTrigger(Skill5);
          break;
        default:
          throw Errors.UnknownEnumValue(skill);
      }
    }

    public void PlayDeathAnimation()
    {
      _healthBar.gameObject.SetActive(false);
      _animator.SetTrigger(Death);
      _collider.enabled = false;
      _state = CreatureState.Dying;
    }

    public void Destroy()
    {
      Destroy(gameObject);
      Destroy(_healthBar.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
      if (!IsIdle()) return;

      // Collision could be with a projectile or with a creature:
      var creature = other.GetComponent<Creature>();
      if (creature)
      {
        Root.Instance.RequestService.OnCreatureCollision(CreatureId, creature.CreatureId);
      }
    }

    // Called by skill animations on their 'start impact' frame
    // ReSharper disable once UnusedMember.Local
    void AttackStart()
    {
      if (!IsAlive()) return;

      switch (_currentSkillType)
      {
        case SkillType.Ranged:
          Root.Instance.RequestService.OnRangedSkillFire(CreatureId);
          break;
        case SkillType.Melee:
          var hitCount = Physics2D.OverlapBoxNonAlloc(
            _collider.bounds.center,
            _collider.bounds.size,
            angle: 0,
            ColliderArray,
            Constants.LayerMaskForPlayerCreatures(Owner.GetOpponent()));
          var hits = new List<int>(hitCount);
          for (var i = 0; i < hitCount; ++i)
          {
            hits.Add(ComponentUtils.GetComponent<Creature>(ColliderArray[i]).CreatureId.Value);
          }

          Root.Instance.RequestService.OnMeleeSkillImpact(CreatureId, hits);

          break;
        default:
          throw Errors.UnknownEnumValue(_currentSkillType);
      }
    }

    public void OnSkillAnimationCompleted(SkillAnimationNumber animationNumber)
    {
      if (!IsAlive()) return;

      var result = Physics2D.OverlapBox(
        _collider.bounds.center,
        _collider.bounds.size,
        angle: 0,
        Constants.LayerMaskForPlayerCreatures(Owner.GetOpponent()));
      _state = _creatureData.Speed > 0 ? CreatureState.Moving : CreatureState.Idle;
      Root.Instance.RequestService.OnSkillComplete(CreatureId, result);
    }

    public void OnDeathAnimationCompleted()
    {
      Root.Instance.CreatureService.Destroy(_creatureData.CreatureId);
    }

    bool IsAlive() => _state != CreatureState.Placing && _state != CreatureState.Dying;

    bool IsIdle() => _state == CreatureState.Idle || _state == CreatureState.Moving;

    void Update()
    {
      var pos = Root.Instance.MainCamera.WorldToScreenPoint(_healthbarAnchor.position);
      _healthBar.transform.position = pos;

      // Ensure lower Y creatures are always rendered on top of higher Y creatures
      _sortingGroup.sortingOrder =
        100 - Mathf.RoundToInt(transform.position.y * 10) - Mathf.RoundToInt(transform.position.x);

      if (_state == CreatureState.Moving)
      {
        _animator.SetBool(Moving, true);
        transform.Translate(Vector3.right * (Time.deltaTime * (_creatureData.Speed / 1000f)));
      }
      else
      {
        _animator.SetBool(Moving, false);
      }
    }
  }
}