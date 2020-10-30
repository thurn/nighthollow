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

using Nighthollow.Services;
using Nighthollow.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Nighthollow.Data;
using Nighthollow.Delegates.Core;
using Nighthollow.Generated;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace Nighthollow.Components
{
  public enum CreatureState
  {
    Placing,
    Idle,
    Moving,
    UsingSkill,
    Dying,
    Stunned
  }

  public sealed class Creature : MonoBehaviour
  {
    [Header("Config")] [SerializeField] bool _debugMode;
    [SerializeField] Transform _projectileSource;
    [SerializeField] AttachmentDisplay _attachmentDisplay;
    [SerializeField] Transform _healthbarAnchor;
    [SerializeField] float _animationSpeedMultiplier;

    [Header("State")] [SerializeField] bool _initialized;
    CreatureData _data;
    [SerializeField] bool _useSkill;
    [SerializeField] int _damageTaken;
    [SerializeField] CreatureState _state;
    SkillData _currentSkill;
    [SerializeField] RankValue? _rankPosition;
    [SerializeField] FileValue? _filePosition;
    [SerializeField] Animator _animator;
    [SerializeField] Collider2D _collider;
    [SerializeField] StatusBarsHolder _statusBars;
    [SerializeField] CreatureService _creatureService;
    [SerializeField] SortingGroup _sortingGroup;
    [SerializeField] CustomTriggerCollider _projectileCollider;
    Coroutine _coroutine;
    [SerializeField] bool _appliedLifeLoss;
    readonly Dictionary<int, float> _skillLastUsedTimes = new Dictionary<int, float>();

    static readonly int Skill1 = Animator.StringToHash("Skill1");
    static readonly int Skill2 = Animator.StringToHash("Skill2");
    static readonly int Skill3 = Animator.StringToHash("Skill3");
    static readonly int Skill4 = Animator.StringToHash("Skill4");
    static readonly int Skill5 = Animator.StringToHash("Skill5");
    static readonly int Death = Animator.StringToHash("Death");
    static readonly int Moving = Animator.StringToHash("Moving");
    static readonly int Hit = Animator.StringToHash("Hit");
    static readonly int SkillSpeed = Animator.StringToHash("SkillSpeedMultiplier");

    public void Initialize(CreatureData creatureData)
    {
      _creatureService = Root.Instance.CreatureService;
      SetState(CreatureState.Placing);
      _animator = GetComponent<Animator>();
      _collider = GetComponent<Collider2D>();
      _collider.enabled = false;
      _sortingGroup = GetComponent<SortingGroup>();
      _statusBars = Root.Instance.Prefabs.CreateStatusBars();
      _statusBars.HealthBar.gameObject.SetActive(false);
      gameObject.layer = Constants.LayerForCreatures(creatureData.BaseType.Owner);
      _animator.speed = _animationSpeedMultiplier;

      _data = creatureData;
      _initialized = true;
    }

    void Start()
    {
      if (!_initialized && _debugMode)
      {
        Initialize(_data.Clone(Root.Instance.StatsForPlayer(Owner)));
        _creatureService.AddUserCreatureAtPosition(this,
          BoardPositions.ClosestRankForXPosition(transform.position.x),
          BoardPositions.ClosestFileForYPosition(transform.position.y));
      }
    }

    public CreatureData Data => _data;

    public PlayerName Owner => _data.BaseType.Owner;

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

    public bool IsMoving => !_rankPosition.HasValue;

    public int DamageTaken => _damageTaken;

    public Transform ProjectileSource => _projectileSource;

    public Collider2D Collider => _collider;

    public CustomTriggerCollider ProjectileCollider => _projectileCollider;

    public SkillData CurrentSkill => _currentSkill;

    public CreatureState State => _state;

    public void EditorSetReferences(Transform projectileSource,
      Transform healthbarAnchor,
      AttachmentDisplay attachmentDisplay)
    {
      _projectileSource = projectileSource;
      _healthbarAnchor = healthbarAnchor;
      _attachmentDisplay = attachmentDisplay;
    }

    public bool AnimationPaused
    {
      set => _animator.speed = value ? 0 : 1;
    }

    public void ActivateCreature(RankValue? rankValue,
      FileValue fileValue,
      float? startingX = 0f,
      float? yOffset = 0)
    {
      _filePosition = fileValue;

      if (rankValue.HasValue)
      {
        _rankPosition = rankValue;
        transform.position = new Vector2(
          rankValue.Value.ToXPosition(),
          fileValue.ToYPosition());
      }
      else if (startingX.HasValue)
      {
        transform.position = new Vector2(
          startingX.Value,
          // We need to offset the Y position for enemy creatures because they are center-anchored:
          // TODO handle this in a better way
          fileValue.ToYPosition() + (yOffset ?? 0));
      }

      _collider.enabled = true;
      if (HasProjectileSkill())
      {
        _projectileCollider = CustomTriggerCollider.Add(
          this, new Vector2(15, 0), new Vector2(30, 1));
      }

      ToDefaultState();

      _useSkill = true;
      _data.Delegate.OnActivate(new CreatureContext(this));

      _coroutine = StartCoroutine(RunCoroutine());
    }

    void UseSkill()
    {
      Errors.CheckState(CanUseSkill(), "Cannot use skill");

      var skill = _data.Delegate.SelectSkill(new CreatureContext(this));
      if (skill != null)
      {
        SetState(CreatureState.UsingSkill);
        _currentSkill = skill;

        var skillAnimation = SelectAnimation(_currentSkill);
        switch (skillAnimation)
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
          case SkillAnimationNumber.Unknown:
          default:
            throw Errors.UnknownEnumValue(skillAnimation);
        }

        _currentSkill.Delegate.OnStart(new SkillContext(this, _currentSkill));
      }
      else
      {
        ToDefaultState();
      }
    }

    SkillAnimationNumber SelectAnimation(SkillData skill)
    {
      var choices = Data.BaseType.SkillAnimations.Where(a => a.Type == skill.BaseType.SkillAnimationType).ToList();
      Errors.CheckState(choices.Count > 0,
        $"Creature is unable to perform animation of type {skill.BaseType.SkillAnimationType}");
      return choices[Random.Range(0, choices.Count)].Number;
    }

    void ToDefaultState()
    {
      SetState(_data.GetInt(Stat.CreatureSpeed) > 0 ? CreatureState.Moving : CreatureState.Idle);
    }

    void Kill()
    {
      if (_coroutine != null)
      {
        StopCoroutine(_coroutine);
      }

      _statusBars.HealthBar.gameObject.SetActive(false);
      _animator.SetTrigger(Death);
      _collider.enabled = false;

      Data.Delegate.OnDeath(new CreatureContext(this));

      SetState(CreatureState.Dying);
      _creatureService.RemoveCreature(this);
    }

    public void DestroyCreature()
    {
      Destroy(gameObject);
      Destroy(_statusBars.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
      // Collision could be with a Projectile or with a TriggerCollider:
      if (CanUseSkill() && other.GetComponent<Creature>())
      {
        _useSkill = true;
      }
    }

    public void OnCustomTriggerCollision(Creature enemy)
    {
      if (CanUseSkill())
      {
        _useSkill = true;
      }
    }

    // Called by skill animations on their 'start impact' frame
    public void AttackStart()
    {
      if (!IsAlive()) return;
      _currentSkill.Delegate.OnUse(new SkillContext(this, _currentSkill));
      if (_currentSkill.BaseType.IsMelee)
      {
        _currentSkill.Delegate.OnImpact(new SkillContext(this, _currentSkill));
      }
    }

    public void AddDamage(Creature appliedBy, int damage)
    {
      Errors.CheckArgument(damage >= 0, "Damage must be non-negative");
      var health = _data.GetInt(Stat.Health);
      _damageTaken = Mathf.Clamp(0, _damageTaken + damage, health);
      if (_damageTaken >= health)
      {
        appliedBy.Data.Delegate.OnKilledEnemy(new CreatureContext(appliedBy), this, damage);
        Kill();
      }
    }

    public void Heal(int healing)
    {
      Errors.CheckArgument(healing >= 0, "Healing must be non-negative");
      var health = _data.GetInt(Stat.Health);
      _damageTaken = Mathf.Clamp(0, _damageTaken - healing, health);
    }

    public void OnActionAnimationCompleted()
    {
      if (!IsAlive() || IsStunned())
      {
        // Ignore exit states from skills that ended early due to stun
        return;
      }

      ToDefaultState();
      _useSkill = true;
    }

    public void OnDeathAnimationCompleted()
    {
      DestroyCreature();
    }

    public void Stun(float durationSeconds)
    {
      if (!IsAlive() || IsStunned()) return;
      StartCoroutine(StunAsync(durationSeconds));
    }

    IEnumerator<YieldInstruction> StunAsync(float durationSeconds)
    {
      _animator.SetTrigger(Hit);
      SetState(CreatureState.Stunned);
      var attachment = Root.Instance.Prefabs.CreateAttachment();
      attachment.Initialize(Root.Instance.Prefabs.StunIcon());
      _attachmentDisplay.AddAttachment(attachment);

      yield return new WaitForSeconds(durationSeconds);

      _attachmentDisplay.ClearAttachments();
      ToDefaultState();
      _useSkill = true;
    }

    public bool IsAlive() => _state != CreatureState.Placing && _state != CreatureState.Dying;

    public bool IsStunned() => _state == CreatureState.Stunned;

    bool HasProjectileSkill() => _data.Skills.Any(s => s.BaseType.IsProjectile);

    bool CanUseSkill() => _state == CreatureState.Idle || _state == CreatureState.Moving;

    public void MarkSkillUsed(SkillTypeData skill) => _skillLastUsedTimes[skill.Id] = Time.time;

    /// <summary>
    /// Returns the timestamp at which the provided skill was last used by this creature, or 0 if it has never been used
    /// </summary>
    public float? TimeLastUsedSeconds(SkillTypeData skill) =>
      _skillLastUsedTimes.ContainsKey(skill.Id) ? (float?) _skillLastUsedTimes[skill.Id] : null;

    IEnumerator<YieldInstruction> RunCoroutine()
    {
      while (true)
      {
        yield return new WaitForSeconds(1);
        Heal(_data.GetInt(Stat.HealthRegenerationPerSecond));
      }

      // ReSharper disable once IteratorNeverReturns
    }

    void Update()
    {
      if (_state == CreatureState.Dying) return;

      // Ensure lower Y creatures are always rendered on top of higher Y creatures
      _sortingGroup.sortingOrder =
        100 - Mathf.RoundToInt(transform.position.y * 10) - Mathf.RoundToInt(transform.position.x);

      if (_state == CreatureState.Placing) return;

      if (_useSkill && CanUseSkill())
      {
        // SelectSkill() is delayed until the next Update() call instead of being invoked
        // immediately because it gives time for the physics system to correctly update
        // collider positions after activation. This was extremely annoying to debug :)
        UseSkill();
        _useSkill = false;
      }

      transform.eulerAngles = _data.BaseType.Owner == PlayerName.Enemy ? new Vector3(0, 180, 0) : Vector3.zero;

      if (transform.position.x > Constants.CreatureDespawnRightX ||
          transform.position.x < Constants.CreatureDespawnLeftX)
      {
        DestroyCreature();
      }
      else if (Owner == PlayerName.Enemy &&
               !_appliedLifeLoss &&
               transform.position.x < Constants.EnemyCreatureRemoveLifeX)
      {
        Root.Instance.User.LoseLife(1);
        _appliedLifeLoss = true;
      }

      var health = _data.GetInt(Stat.Health);
      if (health > 0)
      {
        _statusBars.HealthBar.Value = (health - _damageTaken) / (float) health;
      }

      _statusBars.HealthBar.gameObject.SetActive(_damageTaken > 0);

      var pos = Root.Instance.MainCamera.WorldToScreenPoint(_healthbarAnchor.position);
      _statusBars.transform.position = pos;


      var speedMultiplier = _data.Stats.Get(Stat.SkillSpeedMultiplier).AsMultiplier();
      Errors.CheckState(speedMultiplier > 0.05f, "Skill speed must be > 5%");
      _animator.SetFloat(SkillSpeed, speedMultiplier);

      if (_state == CreatureState.Moving)
      {
        _animator.SetBool(Moving, true);
        transform.Translate(Vector3.right * (Time.deltaTime * (_data.GetInt(Stat.CreatureSpeed) / 1000f)));
      }
      else
      {
        _animator.SetBool(Moving, false);
      }
    }

    void SetState(CreatureState newState)
    {
      _state = newState;
    }
  }
}