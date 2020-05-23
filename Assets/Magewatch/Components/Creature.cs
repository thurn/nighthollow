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

using DG.Tweening;
using Magewatch.API;
using Magewatch.Services;
using Magewatch.Utils;
using UnityEngine;
using UnityEngine.Rendering;

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
    [Header("Config")] [SerializeField] bool _debugMode;
    [SerializeField] float _speed = 2;
    [SerializeField] Transform _meleePosition;
    [SerializeField] AttachmentDisplay _attachmentDisplay;
    [SerializeField] Transform _healthbarAnchor;

    [Header("State")] [SerializeField] bool _initialized;
    [SerializeField] CreatureState _state;
    [SerializeField] Animator _animator;
    [SerializeField] Collider2D _collider;
    [SerializeField] HealthBar _healthBar;
    [SerializeField] CreatureData _creatureData;
    [SerializeField] Creature _currentTarget;
    [SerializeField] bool _touchedTarget;
    [SerializeField] AttackCommand _currentAttack;
    [SerializeField] CreatureService _creatureService;
    [SerializeField] SpriteRenderer[] _renderers;
    [SerializeField] SortingGroup _sortingGroup;

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
      _creatureService = Root.Instance.CreatureService;
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
        _creatureService.AddCreatureAtPosition(this,
          BoardPositions.ClosestRankForXPosition(transform.position.x, _creatureData.Owner),
          BoardPositions.ClosestFileForYPosition(transform.position.y));
      }
    }

    void OnValidate()
    {
      _renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public CreatureId CreatureId => _creatureData.CreatureId;

    public PlayerName Owner => _creatureData.Owner;

    public RankValue RankPosition => _creatureData.RankPosition;

    public FileValue FilePosition => _creatureData.FilePosition;

    public bool AnimationPaused
    {
      set => _animator.speed = value ? 0 : 1;
    }

    public void UpdateCreatureData(CreatureData newData, IOnComplete onComplete = null)
    {
      transform.eulerAngles = newData.Owner == PlayerName.Enemy ? new Vector3(0, 180, 0) : Vector3.zero;

      if (newData.RankPosition != RankValue.RankUnspecified && newData.FilePosition != FileValue.FileUnspecified)
      {
        transform.position = new Vector2(
          newData.RankPosition.ToXPosition(newData.Owner),
          newData.FilePosition.ToYPosition());
      }

      _attachmentDisplay.SetAttachments(newData.Attachments);

      _creatureData = newData;
      onComplete?.OnComplete();
    }

    public void SetPosition(RankValue rankValue, FileValue fileValue)
    {
      _creatureData.RankPosition = rankValue;
      _creatureData.FilePosition = fileValue;
      UpdateCreatureData(_creatureData);
    }

    public void AddAttachment(Attachment attachment)
    {
      Errors.CheckNotNull(attachment);

      _attachmentDisplay.AddAttachment(attachment, () =>
      {
        _creatureData.Attachments.Add(attachment.AttachmentData);
        UpdateCreatureData(_creatureData);
      });
    }

    public void MeleeEngageWithTarget(Creature target, IOnComplete onComplete)
    {
      Errors.CheckNotNull(target);
      Errors.CheckNotNull(onComplete);

      _currentTarget = target;
      _onComplete = onComplete;
      _touchedTarget = false;
      _state = CreatureState.MeleeEngage;
    }

    public void AttackTarget(Creature target, AttackCommand attack, IOnComplete onComplete)
    {
      Errors.CheckNotNull(target);
      Errors.CheckNotNull(attack);
      Errors.CheckNotNull(onComplete);

      _currentTarget = target;
      _currentAttack = attack;
      _onComplete = onComplete;
      _state = CreatureState.Attack;

      switch (attack.Skill)
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
          throw Errors.UnknownEnumValue(attack.Skill);
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

    public bool Highlighted
    {
      set
      {
        foreach (var spriteRenderer in _renderers)
        {
          spriteRenderer.color = value ? Color.green : Color.white;
        }
      }
    }

    public void FadeIn(IOnComplete onComplete)
    {
      var sequence = DOTween.Sequence();
      foreach (var spriteRenderer in _renderers)
      {
        spriteRenderer.color = new Color(1, 1, 1, 0);
        sequence.Insert(0, spriteRenderer.DOFade(1.0f, 0.3f));
      }

      sequence.InsertCallback(0.4f, () =>
      {
        _creatureService.AddCreatureAtPosition(this, RankPosition, FilePosition);
        onComplete.OnComplete();
      });
    }

    public void FadeOut(IOnComplete onComplete)
    {
      _healthBar.gameObject.SetActive(false);
      var sequence = DOTween.Sequence();
      foreach (var spriteRenderer in _renderers)
      {
        sequence.Insert(0, spriteRenderer.DOFade(0.0f, 0.3f));
      }

      sequence.InsertCallback(0.4f, () =>
      {
        _creatureService.Destroy(CreatureId);
        onComplete.OnComplete();
      });
    }

    void ApplyAttackDamage(ApplyDamageEffect damage, IOnComplete onComplete)
    {
      _healthBar.Value -= damage.Damage;
      _healthBar.gameObject.SetActive(_healthBar.Value < 100);
      if (damage.KillsTarget)
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

    // Called by skill animations on their 'start impact' frame
    // ReSharper disable once UnusedMember.Local
    void AttackStart()
    {
      Errors.CheckNotNull(_currentAttack);
      Errors.CheckNotNull(_currentTarget);

      switch (_currentAttack.AttackEffectCase)
      {
        case AttackCommand.AttackEffectOneofCase.ApplyDamage:
          _currentTarget.ApplyAttackDamage(_currentAttack.ApplyDamage, _onComplete);
          if (_currentAttack.ApplyDamage.KillsTarget)
          {
            _currentTarget = null;
          }

          _onComplete = null;
          break;
        case AttackCommand.AttackEffectOneofCase.FireProjectile:
          var fireProjectile = _currentAttack.FireProjectile;
          var target = fireProjectile.AtOpponent
            ? Root.Instance.GetPlayer(Owner.GetOpponent()).Collider
            : _currentTarget._collider;
          Projectile.Instantiate(fireProjectile, _meleePosition, target, () =>
          {
            if (fireProjectile.AtOpponent)
            {
              _onComplete.OnComplete();
            }
            else
            {
              _currentTarget.ApplyAttackDamage(fireProjectile.ApplyDamage, _onComplete);
              if (fireProjectile.ApplyDamage.KillsTarget)
              {
                _currentTarget = null;
              }

              _onComplete = null;
            }
          });
          break;
        default:
          throw Errors.UnknownEnumValue(_currentAttack.AttackEffectCase);
      }
    }

    Transform MeleePosition => _meleePosition;

    Creature CurrentTarget => _currentTarget;

    void Update()
    {
      var pos = Root.Instance.MainCamera.WorldToScreenPoint(_healthbarAnchor.position);
      _healthBar.transform.position = pos;

      // Ensure lower Y creatures are always rendered on top of higher Y creatures
      _sortingGroup.sortingOrder =
        100 - Mathf.RoundToInt(transform.position.y * 10) - Mathf.RoundToInt(transform.position.x);

      if (_state == CreatureState.MeleeEngage)
      {
        // To prevent creatures from overlapping too much, in cases where a creature is *not* the primary target
        // of its opponent, we add a small Y offset to its target position to stagger the creatures in a more visually
        // pleasing way
        var yOffset = Vector3.zero;
        if (_currentTarget.CurrentTarget != this)
        {
          yOffset = new Vector3(0, new[] {0.5f, -0.5f, -0.3f, 0.3f}[CreatureId.Value % 4], 0);
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