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
using Nighthollow.Delegates;
using Nighthollow.Delegates.Handlers;
using Nighthollow.Services;
using Nighthollow.State;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

#nullable enable

namespace Nighthollow.Components
{
  public sealed class Creature : MonoBehaviour, IStatOwner, IKeyValueStoreOwner
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

    [Header("Config")]
    [SerializeField] Transform _projectileSource = null!;

    [SerializeField] AttachmentDisplay _attachmentDisplay = null!;
    [SerializeField] Transform _healthbarAnchor = null!;
    [SerializeField] float _animationSpeedMultiplier;

    [Header("State")]
    [SerializeField] int _damageTaken;

    [SerializeField] CreatureAnimation _state;
    [SerializeField] Animator _animator = null!;
    [SerializeField] Collider2D _collider = null!;
    [SerializeField] StatusBarsHolder _statusBars = null!;
    [SerializeField] CreatureService _creatureService = null!;
    [SerializeField] SortingGroup _sortingGroup = null!;

    readonly Dictionary<int, float> _skillLastUsedTimes = new Dictionary<int, float>();
    Coroutine _coroutine = null!;
    CreatureId _creatureId;
    GameServiceRegistry _registry = null!;

    public CreatureId CreatureId => _creatureId;

    public Transform ProjectileSource => _projectileSource;

    public Collider2D Collider => _collider;

    public bool AnimationPaused
    {
      set => _animator.speed = value ? 0 : 1;
    }

    public CreatureState AsCreatureState() => _creatureService.GetCreatureState(_creatureId);

    public void Initialize(GameServiceRegistry registry, CreatureId creatureId, PlayerName owner)
    {
      _creatureId = creatureId;
      _creatureService = Root.Instance.CreatureService;
      _registry = registry;
      SetState(CreatureAnimation.Placing);
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
      if (_state == CreatureAnimation.Dying)
      {
        _attachmentDisplay.ClearAttachments();
        return;
      }

      // Ensure lower Y creatures are always rendered on top of higher Y creatures
      _sortingGroup.sortingOrder =
        100 - Mathf.RoundToInt(transform.position.y * 10) - Mathf.RoundToInt(transform.position.x);

      if (_state == CreatureAnimation.Placing)
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
        DestroyCreature();
      }
      else if (state.Owner == PlayerName.Enemy &&
               transform.position.x < Constants.EnemyCreatureEndzoneX)
      {
        Invoke(state, new IOnEnemyCreatureAtEndzone.Data(AsCreatureState()));
      }

      var health = state.GetInt(Stat.Health);
      if (health > 0)
      {
        _statusBars.HealthBar.Value = (health - _damageTaken) / (float) health;
      }

      _statusBars.HealthBar.gameObject.SetActive(_damageTaken > 0);

      var pos = Root.Instance.MainCamera.WorldToScreenPoint(_healthbarAnchor.position);
      _statusBars.transform.position = pos;

      var speedMultiplier = state.Get(Stat.SkillSpeedMultiplier).AsMultiplier();
      Errors.CheckState(speedMultiplier > 0.05f, "Skill speed must be > 5%");
      _animator.SetFloat(SkillSpeed, speedMultiplier);

      if (_state == CreatureAnimation.Moving)
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

    void Invoke(CreatureState state, IEventData arg)
    {
      var delegateList = state.CurrentSkill != null ? state.CurrentSkill.DelegateList : state.Data.DelegateList;
      var context = CreateContext();
      var eventQueue = new Queue<IEventData>();
      eventQueue.Enqueue(arg);

      while (true)
      {
        var effects = eventQueue.Dequeue().Raise(context, delegateList);

        foreach (var effect in effects)
        {
          effect.Execute(_registry);
          foreach (var eventData in effect.Events())
          {
            eventQueue.Enqueue(eventData);
          }
        }

        if (eventQueue.Count == 0)
        {
          break;
        }
      }
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

      ToDefaultState(state);

      Invoke(state, new IOnCreatureActivated.Data(AsCreatureState()));
      Root.Instance.HelperTextService.OnCreaturePlayed();

      _coroutine = StartCoroutine(RunCoroutine());
    }

    public void InsertModifier(IStatModifier modifier)
    {
      _creatureService.InsertModifier(CreatureId, modifier);
    }

    public void InsertStatusEffect(StatusEffectData statusEffectData)
    {
      _creatureService.InsertStatusEffect(CreatureId, statusEffectData);
    }

    public void ExecuteMutatation(IMutation mutation)
    {
      _creatureService.ExecuteMutatation(CreatureId, mutation);
    }

    GameContext CreateContext() => new GameContext(_registry);

    void TryToUseSkill(CreatureState state)
    {
      Errors.CheckState(CanUseSkill(), "Cannot use skill");

      var skill = state.Data.DelegateList.FirstNonNull(CreateContext(), new ISelectSkill.Data(AsCreatureState()));
      if (skill != null)
      {
        SetState(CreatureAnimation.UsingSkill);
        state = _creatureService.SetCurrentSkill(CreatureId, skill);

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

        Invoke(state, new IOnSkillStarted.Data(AsCreatureState(), skill));
      }
      else
      {
        ToDefaultState(state);
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

    void ToDefaultState(CreatureState state)
    {
      SetState(state.GetInt(Stat.CreatureSpeed) > 0 ? CreatureAnimation.Moving : CreatureAnimation.Idle);
    }

    void Kill(CreatureState state)
    {
      Invoke(state, new IOnCreatureDeath.Data(AsCreatureState()));

      StopCoroutine(_coroutine);

      _statusBars.HealthBar.gameObject.SetActive(value: false);
      _animator.SetTrigger(Death);
      _collider.enabled = false;

      SetState(CreatureAnimation.Dying);

      _creatureService.OnDeath(this);
    }

    public void DestroyCreature()
    {
      _creatureService.OnDestroyed(this);
      Destroy(gameObject);
      Destroy(_statusBars.gameObject);
    }

    public void OnImpact(IOnSkillImpact.Data arg) => Invoke(_creatureService.GetCreatureState(CreatureId), arg);

    // Called by skill animations on their 'start impact' frame
    public void AttackStart()
    {
      var state = _creatureService.GetCreatureState(CreatureId);
      if (!IsAlive() || state.CurrentSkill == null)
      {
        return;
      }

      Invoke(state, new IOnSkillUsed.Data(AsCreatureState(), state.CurrentSkill));

      if (state.CurrentSkill.IsMelee())
      {
        Invoke(state, new IOnSkillImpact.Data(AsCreatureState(), state.CurrentSkill, projectile: null));
      }
    }

    public void AddDamage(Creature appliedBy, int damage)
    {
      var state = _creatureService.GetCreatureState(CreatureId);
      Errors.CheckArgument(damage >= 0, "Damage must be non-negative");
      var health = state.GetInt(Stat.Health);
      _damageTaken = Mathf.Clamp(value: 0, _damageTaken + damage, health);
      if (_damageTaken >= health)
      {
        appliedBy.Invoke(state, new IOnKilledEnemy.Data(appliedBy.AsCreatureState()));
        Kill(state);
      }
    }

    public void Heal(int healing)
    {
      var data = _creatureService.GetCreatureState(CreatureId);
      Errors.CheckArgument(healing >= 0, "Healing must be non-negative");
      var health = data.GetInt(Stat.Health);
      _damageTaken = Mathf.Clamp(value: 0, _damageTaken - healing, health);
    }

    public void OnActionAnimationCompleted()
    {
      var state = _creatureService.GetCreatureState(CreatureId);
      if (!IsAlive() || IsStunned())
        // Ignore exit states from skills that ended early due to stun
      {
        return;
      }

      ToDefaultState(state);
    }

    public void OnDeathAnimationCompleted()
    {
      DestroyCreature();
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
      SetState(CreatureAnimation.Stunned);
      yield return new WaitForSeconds(durationSeconds);
      ToDefaultState(_creatureService.GetCreatureState(CreatureId));
    }

    public bool IsAlive() => _state != CreatureAnimation.Placing && _state != CreatureAnimation.Dying;

    public bool IsStunned() => _state == CreatureAnimation.Stunned;

    bool CanUseSkill() => _state == CreatureAnimation.Idle || _state == CreatureAnimation.Moving;

    public void MarkSkillUsed(int skillId)
    {
      _skillLastUsedTimes[skillId] = Time.time;
    }

    /// <summary>
    ///   Returns the timestamp at which the provided skill was last used by this creature, or 0 if it has never been used
    /// </summary>
    public float? TimeLastUsedSeconds(int skillId) =>
      _skillLastUsedTimes.ContainsKey(skillId) ? (float?) _skillLastUsedTimes[skillId] : null;

    IEnumerator<YieldInstruction> RunCoroutine()
    {
      while (true)
      {
        yield return new WaitForSeconds(seconds: 1);
        Heal(_creatureService.GetCreatureState(CreatureId).GetInt(Stat.HealthRegenerationPerSecond));
      }

      // ReSharper disable once IteratorNeverReturns
    }

    void SetState(CreatureAnimation @new)
    {
      _state = @new;
    }
  }
}