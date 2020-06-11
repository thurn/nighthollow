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
    [SerializeField] RankValue? _rankPosition;
    [SerializeField] FileValue? _filePosition;
    [SerializeField] Animator _animator;
    [SerializeField] Collider2D _collider;
    [SerializeField] HealthBar _healthBar;
    [SerializeField] CreatureService _creatureService;
    [SerializeField] SpriteRenderer[] _renderers;
    [SerializeField] SortingGroup _sortingGroup;

    static readonly int Skill1 = Animator.StringToHash("Skill1");
    static readonly int Skill2 = Animator.StringToHash("Skill2");
    static readonly int Skill3 = Animator.StringToHash("Skill3");
    static readonly int Skill4 = Animator.StringToHash("Skill4");
    static readonly int Skill5 = Animator.StringToHash("Skill5");
    static readonly int Death = Animator.StringToHash("Death");
    static readonly int Moving = Animator.StringToHash("Moving");

    public void Initialize(CreatureData creatureData)
    {
      _creatureService = Root.Instance.CreatureService;
      _state = CreatureState.Placing;
      _animator = GetComponent<Animator>();
      _collider = GetComponent<Collider2D>();
      _sortingGroup = GetComponent<SortingGroup>();
      _healthBar = Root.Instance.Prefabs.CreateHealthBar();
      _healthBar.gameObject.SetActive(false);

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

    void OnValidate()
    {
      _renderers = GetComponentsInChildren<SpriteRenderer>();
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

    public bool AnimationPaused
    {
      set => _animator.speed = value ? 0 : 1;
    }

    public void UpdateCreatureData(CreatureData newData)
    {
      transform.eulerAngles = newData.Owner == PlayerName.Enemy ? new Vector3(0, 180, 0) : Vector3.zero;

      if (newData.Attachments != null)
      {
        _attachmentDisplay.SetAttachments(newData.Attachments);
      }

      _creatureData = newData;
    }

    public void SetEnemyCreatureFile(FileValue fileValue)
    {
      _filePosition = fileValue;
      _state = _creatureData.Speed > 0 ? CreatureState.Moving : CreatureState.Idle;
      transform.position = new Vector2(
        Constants.EnemyCreatureStartingX,
        // We need to offset the Y position for enemy creatures because they are center-anchored:
        fileValue.ToYPosition() + Constants.EnemyCreatureYOffset);
    }

    public void SetUserCreaturePosition(RankValue rankValue, FileValue fileValue)
    {
      _rankPosition = rankValue;
      _filePosition = fileValue;
      _state = _creatureData.Speed > 0 ? CreatureState.Moving : CreatureState.Idle;
      transform.position = new Vector2(
        rankValue.ToXPosition(),
        fileValue.ToYPosition());
    }

    public void UseSkill(SkillAnimationNumber skill, Creature meleeTarget)
    {
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

    public void OnDeathAnimationCompleted()
    {
      Root.Instance.CreatureService.Destroy(_creatureData.CreatureId);
    }

    public void Destroy()
    {
      Destroy(gameObject);
      Destroy(_healthBar.gameObject);
    }

    // Called by skill animations on their 'start impact' frame
    // ReSharper disable once UnusedMember.Local
    void AttackStart()
    {
    }

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