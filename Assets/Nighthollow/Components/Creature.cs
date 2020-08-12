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
using Nighthollow.Data;
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
    [SerializeField] CreatureData _data;
    [SerializeField] bool _selectSkill;
    [SerializeField] int _damageTaken;
    [SerializeField] CreatureState _state;
    [SerializeField] SkillType _currentSkillType;
    [SerializeField] RankValue? _rankPosition;
    [SerializeField] FileValue? _filePosition;
    [SerializeField] Animator _animator;
    [SerializeField] Collider2D _collider;
    [SerializeField] HealthBar _healthBar;
    [SerializeField] CreatureService _creatureService;
    [SerializeField] SortingGroup _sortingGroup;

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

    public Transform ProjectileSource => _projectileSource;

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

      if (HasProjectile())
      {
        CustomTriggerCollider.Add(this, new Vector2(15, 0), new Vector2(30, 1));
      }

      _state = CreatureState.Idle;
      _selectSkill = true;

      _data.Events.OnEnteredPlay(this);
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

    void Kill()
    {
      _healthBar.gameObject.SetActive(false);
      _animator.SetTrigger(Death);
      _collider.enabled = false;
      _state = CreatureState.Dying;
      _creatureService.RemoveCreature(this);
    }

    public void DestroyCreature()
    {
      Destroy(gameObject);
      Destroy(_healthBar.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
      if (!CanUseSkill()) return;

      // Collision could be with a Projectile or with a TriggerCollider:
      var creature = other.GetComponent<Creature>();

      if (creature)
      {
        if (_data.Handlers.SkillSelectionHandler)
        {
          _data.Handlers.SkillSelectionHandler.OnCollision(this, creature);
        }
        else if (_data.DefaultMeleeSkill != SkillAnimationNumber.Unknown)
        {
          UseSkill(_data.DefaultMeleeSkill, SkillType.Melee);
        }
      }
    }

    // Called by skill animations on their 'start impact' frame
    public void AttackStart()
    {
      if (!IsAlive()) return;

      switch (_currentSkillType)
      {
        case SkillType.Ranged:
          if (_data.Handlers.FireProjectileHandler)
          {
            _data.Handlers.FireProjectileHandler.Execute(this);
          }
          else
          {
            var projectile = Root.Instance.ObjectPoolService.Create(
              _data.Projectile.Prefab, _projectileSource.position);
            projectile.Initialize(this, _data.Projectile, _projectileSource);
          }
          break;
        case SkillType.Melee:
          if (_data.Handlers.MeleeHitHandler)
          {
            _data.Handlers.MeleeHitHandler.Execute(this);
          }
          else
          {
            foreach (var creature in GetColliderOverlaps())
            {
              creature.AddDamage(_data.BaseAttack.Total());
            }
          }
          break;
        default:
          throw Errors.UnknownEnumValue(_currentSkillType);
      }
    }

    List<Creature> GetColliderOverlaps()
    {
      var hits = Physics2D.OverlapBoxAll(
        _collider.bounds.center,
        _collider.bounds.size,
        angle: 0,
        Constants.LayerMaskForCreatures(Owner.GetOpponent()));

      var hitCreatures = new List<Creature>();
      foreach (var hit in hits)
      {
        var creature = hit.GetComponent<Creature>();
        if (creature)
        {
          hitCreatures.Add(creature);
        }
      }

      return hitCreatures;
    }

    public void AddDamage(int damage)
    {
      _damageTaken += damage;
      if (_damageTaken >= _data.Health.Value)
      {
        Kill();
      }
    }

    public void OnProjectileImpact(Projectile projectile)
    {
      if (_data.Handlers.ProjectileImpactHandler)
      {
        _data.Handlers.ProjectileImpactHandler.OnProjectileImpact(this, projectile);
      }
      else
      {
        var hits = Physics2D.OverlapBoxAll(
          projectile.transform.position,
          new Vector2(_data.Projectile.HitboxSize / 1000f, _data.Projectile.HitboxSize / 1000f),
          angle: 0,
          Constants.LayerMaskForCreatures(Owner.GetOpponent()));

        var hitCreatures = new List<Creature>(hits.Length);
        hitCreatures.AddRange(hits.Select(ComponentUtils.GetComponent<Creature>));

        foreach (var creature in hitCreatures)
        {
          creature.AddDamage(_data.BaseAttack.Total());
        }
      }
    }

    public void OnSkillAnimationCompleted(SkillAnimationNumber animationNumber)
    {
      if (!IsAlive()) return;

      _selectSkill = true;
    }

    void SelectSkill()
    {
      if (_data.Handlers.SkillSelectionHandler)
      {
        _data.Handlers.SkillSelectionHandler.OnSkillCompleted(this);
        return;
      }

      var overlaps = GetColliderOverlaps();

      if (_data.DefaultMeleeSkill != SkillAnimationNumber.Unknown && overlaps.Count > 0)
      {
        UseSkill(_data.DefaultMeleeSkill, SkillType.Melee);
        return;
      }
      else if (HasProjectile())
      {
        var hit = Physics2D.BoxCast(
          _collider.bounds.center,
          _collider.bounds.size,
          angle: 0,
          direction: Constants.ForwardDirectionForPlayer(Owner),
          distance: 25f,
          layerMask: Constants.LayerMaskForCreatures(Owner.GetOpponent()));

        if (hit.collider)
        {
          UseSkill(_data.DefaultCastSkill, SkillType.Ranged);
          return;
        }
      }

      // Default state
      _state = _data.Speed.Value > 0 ? CreatureState.Moving : CreatureState.Idle;
    }

    public void OnDeathAnimationCompleted()
    {
      DestroyCreature();
    }

    public void OnCustomTriggerCollision(Creature enemy)
    {
      if (!CanUseSkill()) return;

      if (_data.Handlers.SkillSelectionHandler)
      {
        _data.Handlers.SkillSelectionHandler.OnCustomTriggerCollision(this, enemy);
      }
      else if (HasProjectile())
      {
        UseSkill(_data.DefaultCastSkill, SkillType.Ranged);
      }
    }

    public bool IsAlive() => _state != CreatureState.Placing && _state != CreatureState.Dying;

    public bool HasProjectile() => _data.DefaultCastSkill != SkillAnimationNumber.Unknown && _data.Projectile;

    bool CanUseSkill() => _state == CreatureState.Idle || _state == CreatureState.Moving;

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
        SelectSkill();
        _selectSkill = false;
      }

      transform.eulerAngles = _data.Owner == PlayerName.Enemy ? new Vector3(0, 180, 0) : Vector3.zero;

      _healthBar.Value = (_data.Health.Value - _damageTaken) / (float)_data.Health.Value;
      _healthBar.gameObject.SetActive(_damageTaken > 0);

      var pos = Root.Instance.MainCamera.WorldToScreenPoint(_healthbarAnchor.position);
      _healthBar.transform.position = pos;

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