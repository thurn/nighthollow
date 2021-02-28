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

using System.Collections.Generic;
using System.Linq;
using Nighthollow.Data;
using Nighthollow.Delegates.Handlers;
using Nighthollow.Services;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.Rendering;

#nullable enable

namespace Nighthollow.Components
{
  public sealed class Creature : MonoBehaviour
  {
    static readonly int Skill1 = Animator.StringToHash("Skill1");
    static readonly int Skill2 = Animator.StringToHash("Skill2");
    static readonly int Skill3 = Animator.StringToHash("Skill3");
    static readonly int Skill4 = Animator.StringToHash("Skill4");
    static readonly int Skill5 = Animator.StringToHash("Skill5");
    static readonly int Death = Animator.StringToHash("Death");
    static readonly int Moving = Animator.StringToHash("Moving");
    static readonly int Hit = Animator.StringToHash("Hit");
    static readonly int SkillSpeed = Animator.StringToHash("SkillSpeedMultiplier");

    [SerializeField] Transform _projectileSource = null!;
    [SerializeField] AttachmentDisplay _attachmentDisplay = null!;
    [SerializeField] Transform _healthbarAnchor = null!;
    [SerializeField] float _animationSpeedMultiplier;
    [SerializeField] CreatureAnimation _animationState;
    [SerializeField] Animator _animator = null!;
    [SerializeField] Collider2D _collider = null!;
    [SerializeField] StatusBarsHolder _statusBars = null!;
    [SerializeField] SortingGroup _sortingGroup = null!;

    Coroutine _coroutine = null!;
    CreatureId _creatureId;
    GameServiceRegistry _registry = null!;

    public CreatureId CreatureId => _creatureId;
    public Transform ProjectileSource => _projectileSource;
    public Collider2D Collider => _collider;

    public void Initialize(GameServiceRegistry registry, CreatureId creatureId, PlayerName owner)
    {
      _creatureId = creatureId;
      _registry = registry;
      _animationState = CreatureAnimation.Placing;
      _animator = GetComponent<Animator>();
      _collider = GetComponent<Collider2D>();
      _collider.enabled = false;
      _sortingGroup = GetComponent<SortingGroup>();
      _statusBars = Root.Instance.Prefabs.CreateStatusBars();
      _statusBars.HealthBar.gameObject.SetActive(value: false);
      gameObject.layer = Constants.LayerForCreatures(owner);
      _animator.speed = _animationSpeedMultiplier;
    }

    public void OnUpdate(CreatureState state)
    {
      if (_animationState == CreatureAnimation.Dying)
      {
        _attachmentDisplay.ClearAttachments();
        return;
      }

      // Ensure lower Y creatures are always rendered on top of higher Y creatures
      _sortingGroup.sortingOrder =
        100 - Mathf.RoundToInt(transform.position.y * 10) - Mathf.RoundToInt(transform.position.x);

      if (_animationState == CreatureAnimation.Placing)
      {
        return;
      }

      if (CanUseSkill())
      {
        TryToUseSkill(state);
      }

      transform.eulerAngles = state.Owner == PlayerName.Enemy ? new Vector3(x: 0, y: 180, z: 0) : Vector3.zero;

      if (transform.position.x > Constants.CreatureDespawnRightX ||
          transform.position.x < Constants.CreatureDespawnLeftX)
      {
        _registry.CreatureService.DespawnCreature(CreatureId);
      }
      else if (state.Owner == PlayerName.Enemy &&
               transform.position.x < Constants.EnemyCreatureEndzoneX)
      {
        _registry.Invoke(CreatureId, new IOnEnemyCreatureAtEndzone.Data(state));
      }

      var health = state.GetInt(Stat.Health);
      if (health > 0)
      {
        _statusBars.HealthBar.Value = (health - state.DamageTaken) / (float) health;
      }

      _statusBars.HealthBar.gameObject.SetActive(state.DamageTaken > 0);

      var pos = Root.Instance.MainCamera.WorldToScreenPoint(_healthbarAnchor.position);
      _statusBars.transform.position = pos;

      var speedMultiplier = state.Get(Stat.SkillSpeedMultiplier).AsMultiplier();
      Errors.CheckState(speedMultiplier > 0.05f, "Skill speed must be > 5%");
      _animator.SetFloat(SkillSpeed, speedMultiplier);

      if (_animationState == CreatureAnimation.Moving)
      {
        _animator.SetBool(Moving, value: true);
        transform.Translate(Vector3.right * (Time.deltaTime * (state.GetInt(Stat.CreatureSpeed) / 1000f)));
      }
      else
      {
        _animator.SetBool(Moving, value: false);
      }

      _attachmentDisplay.SetStatusEffects(_registry, state.Data.Stats.StatusEffects);
    }

    public void EditorSetReferences(Transform projectileSource,
      Transform healthbarAnchor,
      AttachmentDisplay attachmentDisplay)
    {
      _projectileSource = projectileSource;
      _healthbarAnchor = healthbarAnchor;
      _attachmentDisplay = attachmentDisplay;
    }

    public void ActivateCreature(
      CreatureState state,
      float? startingX = 0f)
    {
      var filePosition = Errors.CheckNotNull(state.FilePosition);
      if (state.RankPosition.HasValue)
      {
        transform.position = new Vector2(
          state.RankPosition.Value.ToXPosition(),
          filePosition.ToYPosition());
      }
      else if (startingX.HasValue)
      {
        transform.position = new Vector2(
          startingX.Value,
          filePosition.ToYPosition());
      }

      _collider.enabled = true;

      ToDefaultState();

      _registry.Invoke(CreatureId, new IOnCreatureActivated.Data(state));
      Root.Instance.HelperTextService.OnCreaturePlayed();

      _coroutine = StartCoroutine(RunCoroutine());
    }

    void TryToUseSkill(CreatureState state)
    {
      var skill = state.Data.DelegateList.FirstNonNull(_registry.Context, new ISelectSkill.Data(state));
      if (skill != null)
      {
        _animationState = CreatureAnimation.UsingSkill;
        state = _registry.CreatureService.Mutate(CreatureId, s => s.WithCurrentSkill(skill));

        var skillAnimation = SelectAnimation(state, skill);
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

        _registry.Invoke(CreatureId, new IOnSkillStarted.Data(state, skill));
      }
      else
      {
        ToDefaultState();
      }
    }

    SkillAnimationNumber SelectAnimation(CreatureState state, SkillData skill)
    {
      var choices = state.Data.BaseType.SkillAnimations
        .Where(a => a.SkillAnimationType == skill.BaseType.SkillAnimationType).ToList();
      Errors.CheckState(choices.Count > 0,
        $"Creature is unable to perform animation of type {skill.BaseType.SkillAnimationType}");
      return choices[Random.Range(minInclusive: 0, choices.Count)].SkillAnimationNumber;
    }

    void ToDefaultState()
    {
      _animationState = _registry.CreatureService[CreatureId].GetInt(Stat.CreatureSpeed) > 0
        ? CreatureAnimation.Moving
        : CreatureAnimation.Idle;
    }

    public void Kill()
    {
      StopCoroutine(_coroutine);
      _statusBars.HealthBar.gameObject.SetActive(value: false);
      _animator.SetTrigger(Death);
      _collider.enabled = false;
      _animationState = CreatureAnimation.Dying;
    }

    public void SetAnimationPaused(bool paused)
    {
      _animator.speed = paused ? 0 : 1;
    }

    public void Despawn()
    {
      Destroy(gameObject);
      Destroy(_statusBars.gameObject);
    }

    // Called by skill animations on their 'start impact' frame
    public void AttackStart()
    {
      var state = _registry.CreatureService[CreatureId];
      if (!IsAlive() || state.CurrentSkill == null)
      {
        return;
      }

      _registry.Invoke(CreatureId, new IOnSkillUsed.Data(state, state.CurrentSkill));

      if (state.CurrentSkill.IsMelee())
      {
        _registry.Invoke(CreatureId, new IOnSkillImpact.Data(state, state.CurrentSkill, projectile: null));
      }
    }

    public void OnActionAnimationCompleted()
    {
      if (!IsAlive() || IsStunned())
        // Ignore exit states from skills that ended early due to stun
      {
        return;
      }

      ToDefaultState();
    }

    public void OnDeathAnimationCompleted()
    {
      _registry.CreatureService.DespawnCreature(CreatureId);
    }

    public void Stun(float durationSeconds)
    {
      if (!IsAlive() || IsStunned())
      {
        return;
      }

      StartCoroutine(StunAsync(durationSeconds));
    }

    IEnumerator<YieldInstruction> StunAsync(float durationSeconds)
    {
      _animator.SetTrigger(Hit);
      _animationState = CreatureAnimation.Stunned;
      yield return new WaitForSeconds(durationSeconds);
      ToDefaultState();
    }

    public bool IsAlive() => _animationState != CreatureAnimation.Placing && _animationState != CreatureAnimation.Dying;

    bool IsStunned() => _animationState == CreatureAnimation.Stunned;

    bool CanUseSkill() => _animationState == CreatureAnimation.Idle || _animationState == CreatureAnimation.Moving;

    IEnumerator<YieldInstruction> RunCoroutine()
    {
      while (true)
      {
        yield return new WaitForSeconds(seconds: 1);
        _registry.CreatureService.Heal(CreatureId,
          _registry.CreatureService[CreatureId].GetInt(Stat.HealthRegenerationPerSecond));
      }

      // ReSharper disable once IteratorNeverReturns
    }
  }
}
