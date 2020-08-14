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

using Nighthollow.Data;
using Nighthollow.Services;
using Nighthollow.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] CreatureData _data;
    [SerializeField] bool _selectSkill;
    [SerializeField] int _damageTaken;
    [SerializeField] int _currentEnergy;
    [SerializeField] CreatureState _state;
    [SerializeField] SkillData _currentSkill;
    [SerializeField] ProjectileData _currentProjectile;
    [SerializeField] RankValue? _rankPosition;
    [SerializeField] FileValue? _filePosition;
    [SerializeField] Animator _animator;
    [SerializeField] Collider2D _collider;
    [SerializeField] StatusBarsHolder _statusBars;
    [SerializeField] CreatureService _creatureService;
    [SerializeField] SortingGroup _sortingGroup;
    [SerializeField] CustomTriggerCollider _projectileCollider;
    [SerializeField] Coroutine _coroutine;

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
      _statusBars = Root.Instance.Prefabs.CreateStatusBars();
      _statusBars.HealthBar.gameObject.SetActive(false);
      _statusBars.EnergyBar.gameObject.SetActive(false);
      gameObject.layer = Constants.LayerForCreatures(creatureData.Owner);

      // Slow down enemy animations a bit
      _animator.SetFloat(AnimationSpeed, creatureData.Owner == PlayerName.User ? 1.0f : 0.5f);

      _data = creatureData;
      _initialized = true;
    }

    void Start()
    {
      if (!_initialized && _debugMode)
      {
        Initialize(Instantiate(_data));
        _creatureService.AddUserCreatureAtPosition(this,
          BoardPositions.ClosestRankForXPosition(transform.position.x),
          BoardPositions.ClosestFileForYPosition(transform.position.y));
      }
    }

    public CreatureData Data => _data;

    public PlayerName Owner => _data.Owner;

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

    public int DamageTaken => _damageTaken;

    public int CurrentEnergy => _currentEnergy;

    public Transform ProjectileSource => _projectileSource;

    public Collider2D Collider => _collider;

    public CustomTriggerCollider ProjectileCollider => _projectileCollider;

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

    public void ActivateCreature(RankValue? rankValue, FileValue fileValue)
    {
      _filePosition = fileValue;

      if (rankValue.HasValue)
      {
        _rankPosition = rankValue;
        transform.position = new Vector2(
          rankValue.Value.ToXPosition(),
          fileValue.ToYPosition());
      }
      else
      {
        transform.position = new Vector2(
          Constants.EnemyCreatureStartingX,
          // We need to offset the Y position for enemy creatures because they are center-anchored:
          // TODO handle this in a better way
          fileValue.ToYPosition() + Constants.EnemyCreatureYOffset);
      }

      _collider.enabled = true;
      if (HasProjectileSkill())
      {
        _projectileCollider = CustomTriggerCollider.Add(this, new Vector2(15, 0), new Vector2(30, 1));
      }

      _currentEnergy = _data.StartingEnergy.Value;

      _data.Delegate.OnActivate(this);
      _state = CreatureState.Idle;
      _selectSkill = true;
      _data.Events.OnEnteredPlay(this);

      _coroutine = StartCoroutine(RunCoroutine());
    }

    void SelectSkillType()
    {
      if (!IsAlive()) return;

      if (_data.Delegate.ShouldUseMeleeSkill(this))
      {
        UseSkill(SkillType.Melee);
        return;
      }
      else if (_data.Delegate.ShouldUseProjectileSkill(this))
      {
        UseSkill(SkillType.Projectile);
        return;
      }
      else
      {
        ToDefaultState();
      }
    }

    void UseSkill(SkillType skillType)
    {
      if (!IsAlive())
      {
        throw new InvalidOperationException("Attempted to use skill on non-living creature");
      }

      _state = CreatureState.UsingSkill;
      _currentSkill = _data.Delegate.ChooseSkill(this, skillType);
      if (_currentSkill == null)
      {
        ToDefaultState();
        return;
      }

      SpendEnergy(_currentSkill.EnergyCost);

      if (_currentSkill.SkillType == SkillType.Projectile)
      {
        _currentProjectile = _data.Delegate.ChooseProjectile(this);
        SpendEnergy(_currentProjectile.EnergyCost);
      }

      switch (_currentSkill.Animation)
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
          throw Errors.UnknownEnumValue(_currentSkill.Animation);
      }
    }

    void ToDefaultState()
    {
      _state = _data.Speed.Value > 0 ? CreatureState.Moving : CreatureState.Idle;
    }

    void Kill()
    {
      if (_coroutine != null)
      {
        StopCoroutine(_coroutine);
      }

      _statusBars.HealthBar.gameObject.SetActive(false);
      _statusBars.EnergyBar.gameObject.SetActive(false);
      _animator.SetTrigger(Death);
      _collider.enabled = false;
      _state = CreatureState.Dying;
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
        _selectSkill = true;
      }
    }

    public void OnCustomTriggerCollision(Creature enemy)
    {
      if (CanUseSkill())
      {
        _selectSkill = true;
      }
    }

    // Called by skill animations on their 'start impact' frame
    public void AttackStart()
    {
      if (!IsAlive()) return;

      switch (_currentSkill.SkillType)
      {
        case SkillType.Projectile:
          _data.Delegate.OnFireProjectile(this, _currentProjectile);
          break;
        case SkillType.Melee:
          _data.Delegate.OnMeleeHit(this);
          break;
        default:
          throw Errors.UnknownEnumValue(_currentSkill.SkillType);
      }
    }

    public void AddDamage(int damage)
    {
      Errors.CheckArgument(damage >= 0, "Energy must be non-negative");
      var newDamage = _damageTaken + damage;
      _damageTaken = Mathf.Clamp(0, newDamage, _data.Health.Value);
      if (_damageTaken >= _data.Health.Value)
      {
        Kill();
      }
    }

    public void AddEnergy(int energy)
    {
      Errors.CheckArgument(energy >= 0, "Energy must be non-negative");
      var newEnergy = _currentEnergy + energy;
      _currentEnergy = Mathf.Clamp(0, newEnergy, _data.MaximumEnergy.Value);
    }

    public void SpendEnergy(int energy)
    {
      Errors.CheckArgument(energy >= 0, "Spent energy must be non-negative");
      Errors.CheckArgument(energy <= _currentEnergy, "Spent energy must be <= current energy");
      _currentEnergy -= energy;
    }

    public void OnProjectileImpact(Projectile projectile)
    {
      _data.Delegate.OnProjectileImpact(this, projectile);
    }

    public void OnSkillAnimationCompleted(SkillAnimationNumber animationNumber)
    {
      if (!IsAlive()) return;
      _selectSkill = true;
    }

    public void OnDeathAnimationCompleted()
    {
      DestroyCreature();
    }

    public bool IsAlive() => _state != CreatureState.Placing && _state != CreatureState.Dying;

    public bool HasMeleeSkill() => _data.Skills.Any(s => s.SkillType == SkillType.Melee);

    public bool HasProjectileSkill() =>
      _data.Skills.Any(s => s.SkillType == SkillType.Projectile) &&
      _data.Projectiles.Any();

    bool CanUseSkill() => _state == CreatureState.Idle || _state == CreatureState.Moving;

    IEnumerator<YieldInstruction> RunCoroutine()
    {
      while (true)
      {
        var interval = _data.EnergyGainIntervalMs.Value;
        if (interval <= 0)
        {
          break;
        }

        yield return new WaitForSeconds(interval / 1000f);

        _currentEnergy += _data.EnergyGain.Value;
      }
    }

    void Update()
    {
      if (_state == CreatureState.Dying) return;

      // Ensure lower Y creatures are always rendered on top of higher Y creatures
      _sortingGroup.sortingOrder =
        100 - Mathf.RoundToInt(transform.position.y * 10) - Mathf.RoundToInt(transform.position.x);

      if (_state == CreatureState.Placing) return;

      if (_selectSkill)
      {
        // SelectSkill() is delayed until the next Update() call instead of being invoked
        // immediately because it gives time for the physics system to correctly update
        // collider positions after activation. This was extremely annoying to debug :)
        SelectSkillType();
        _selectSkill = false;
      }

      transform.eulerAngles = _data.Owner == PlayerName.Enemy ? new Vector3(0, 180, 0) : Vector3.zero;

      if (_data.Health.Value > 0)
      {
        _statusBars.HealthBar.Value = (_data.Health.Value - _damageTaken) / (float)_data.Health.Value;
      }
      _statusBars.HealthBar.gameObject.SetActive(_damageTaken > 0);

      if (_data.MaximumEnergy.Value > 0)
      {
        _statusBars.EnergyBar.Value = _currentEnergy / (float)_data.MaximumEnergy.Value;
      }
      _statusBars.EnergyBar.gameObject.SetActive(_currentEnergy > 0);

      var pos = Root.Instance.MainCamera.WorldToScreenPoint(_healthbarAnchor.position);
      _statusBars.transform.position = pos;

      if (_state == CreatureState.Moving)
      {
        _animator.SetBool(Moving, true);
        transform.Translate(Vector3.right * (Time.deltaTime * (_data.Speed.Value / 1000f)));
      }
      else
      {
        _animator.SetBool(Moving, false);
      }
    }
  }
}
