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
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;
using Nighthollow.Data;
using Nighthollow.Delegates;
using Nighthollow.Delegates.Handlers;
using Nighthollow.Services;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

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

    [SerializeField] int _creatureId;
    [SerializeField] Transform _projectileSource = null!;
    [SerializeField] AttachmentDisplay _attachmentDisplay = null!;
    [SerializeField] Transform _healthbarAnchor = null!;
    [SerializeField] float _animationSpeedMultiplier;
    [SerializeField] Animator _animator = null!;
    [SerializeField] Collider2D _collider = null!;
    [SerializeField] StatusBarsHolder _statusBars = null!;
    [SerializeField] SortingGroup _sortingGroup = null!;
    [SerializeField] CreatureAnimation _currentAnimation = CreatureAnimation.Placing;

    // [Header("Config")]
    // [SerializeField] Transform _projectileSource = null!;
    //
    // [SerializeField] AttachmentDisplay _attachmentDisplay = null!;
    // [SerializeField] Transform _healthbarAnchor = null!;
    // [SerializeField] float _animationSpeedMultiplier;
    //
    // [Header("State")]
    // [SerializeField] int _damageTaken;
    //
    // [SerializeField] CreatureAnimation _state;
    // [SerializeField] RankValue? _rankPosition;
    // [SerializeField] FileValue? _filePosition;
    // [SerializeField] Animator _animator = null!;
    // [SerializeField] Collider2D _collider = null!;
    // [SerializeField] StatusBarsHolder _statusBars = null!;
    // [SerializeField] CreatureService _creatureService = null!;
    // [SerializeField] SortingGroup _sortingGroup = null!;
    //
    // readonly Dictionary<int, float> _skillLastUsedTimes = new Dictionary<int, float>();
    // Coroutine _coroutine = null!;
    // SkillData? _currentSkill;
    // CreatureData _data = null!;
    // GameServiceRegistry _registry = null!;
    //
    // public CreatureData Data => _data;
    //
    // public PlayerName Owner => _data.BaseType.Owner;
    //
    // public RankValue? RankPosition => _rankPosition;
    //
    // public FileValue FilePosition =>
    //   _filePosition ?? throw new InvalidOperationException("Creature does not have a file position");
    //
    // public bool IsMoving => !_rankPosition.HasValue;
    //
    // public int DamageTaken => _damageTaken;
    //
    // public Transform ProjectileSource => _projectileSource;
    //
    // public Collider2D Collider => _collider;
    //
    // public SkillData? CurrentSkill => _currentSkill;
    //
    // public CreatureAnimation State => _state;
    //
    // public string Name => _data.BaseType.Name;
    //
    // public bool AnimationPaused
    // {
    //   set => _animator.speed = value ? 0 : 1;
    // }
    //
    // public CreatureState AsCreatureState() =>
    //   new CreatureState(
    //     this,
    //     _data,
    //     _state,
    //     _rankPosition,
    //     _filePosition,
    //     _currentSkill,
    //     Owner,
    //     _skillLastUsedTimes.ToImmutableDictionary());

    public void Initialize(Prefabs prefabs, PlayerName owner)
    {
      _animator = ComponentUtils.GetComponent<Animator>(this);
      _animator.speed = _animationSpeedMultiplier;
      _collider = ComponentUtils.GetComponent<Collider2D>(this);
      _collider.enabled = false;
      _sortingGroup = ComponentUtils.GetComponent<SortingGroup>(this);
      _statusBars = prefabs.CreateStatusBars();
      _statusBars.HealthBar.gameObject.SetActive(false);
      gameObject.layer = Constants.LayerForCreatures(owner);
    }

    // public void Initialize(GameServiceRegistry registry, CreatureData creatureData)
    // {
    //   _creatureService = Root.Instance.CreatureService;
    //   _registry = registry;
    //   SetState(CreatureAnimation.Placing);
    //   _animator = GetComponent<Animator>();
    //   _collider = GetComponent<Collider2D>();
    //   _collider.enabled = false;
    //   _sortingGroup = GetComponent<SortingGroup>();
    //   _statusBars = Root.Instance.Prefabs.CreateStatusBars();
    //   _statusBars.HealthBar.gameObject.SetActive(value: false);
    //   gameObject.layer = Constants.LayerForCreatures(creatureData.BaseType.Owner);
    //   _animator.speed = _animationSpeedMultiplier;
    //
    //   _data = creatureData;
    // }

    public static CreatureState NextState(GameContext c, CreatureState state)
    {
      state = state.WithData(state.Data.OnTick());
      state = TryToUseSkill(c, state);
      return state;
    }

    [MustUseReturnValue]
    public IEnumerable<Effect> OnUpdate(GameContext c, CreatureState state)
    {
      var result = ImmutableList<Effect>.Empty;
      var previousAnimation = state.Animation;
      _currentAnimation = state.Animation;

      if (_currentAnimation == CreatureAnimation.Dying)
      {
        if (previousAnimation != CreatureAnimation.Dying)
        {
          result = Kill(c, state, result);
        }

        return result;
      }

      // Ensure lower Y creatures are always rendered on top of higher Y creatures
      _sortingGroup.sortingOrder =
        100 - Mathf.RoundToInt(transform.position.y * 10) - Mathf.RoundToInt(transform.position.x);

      if (_currentAnimation == CreatureAnimation.Placing)
      {
        return result;
      }
      else if (previousAnimation == CreatureAnimation.Placing)
      {
        result = ActivateCreature(c, state, result);
      }

      if (_currentAnimation == CreatureAnimation.UsingSkill &&
          previousAnimation != CreatureAnimation.UsingSkill &&
          state.CurrentSkill != null)
      {
        PlaySkillAnimation(state.Data, state.CurrentSkill);
        result = Raise(c, state, result, new IOnSkillStarted.Data(state, state.CurrentSkill));
      }

      transform.eulerAngles = state.Owner == PlayerName.Enemy ? new Vector3(x: 0, y: 180, z: 0) : Vector3.zero;

      if (transform.position.x > Constants.CreatureDespawnRightX ||
          transform.position.x < Constants.CreatureDespawnLeftX)
      {
        result = Raise(c, state, result, new IOnCreatureDespawn.Data(state));
      }
      else if (state.Owner == PlayerName.Enemy && transform.position.x < Constants.EnemyCreatureEndzoneX)
      {
        result = Raise(c, state, result, new IOnEnemyCreatureAtEndzone.Data(state));
      }

      var health = state.Data.GetInt(Stat.Health);
      if (health > 0)
      {
        _statusBars.HealthBar.Value = (health - state.DamageTaken) / (float) health;
      }

      _statusBars.HealthBar.gameObject.SetActive(state.DamageTaken > 0);
      var pos = Root.Instance.MainCamera.WorldToScreenPoint(_healthbarAnchor.position);
      _statusBars.transform.position = pos;

      var speedMultiplier = state.Data.Stats.Get(Stat.SkillSpeedMultiplier).AsMultiplier();
      Errors.CheckState(speedMultiplier > 0.05f, "Skill speed must be > 5%");
      _animator.SetFloat(SkillSpeed, speedMultiplier);

      if (state.Animation == CreatureAnimation.Moving)
      {
        _animator.SetBool(Moving, value: true);
        transform.Translate(Vector3.right * (Time.deltaTime * (state.GetInt(Stat.CreatureSpeed) / 1000f)));
      }
      else
      {
        _animator.SetBool(Moving, value: false);
      }

      _attachmentDisplay.SetStatusEffects(c, state.Stats.StatusEffects);

      return result;
    }

    // void Update()
    // {
    //   _data = _data.OnTick();
    //
    //   if (_state == CreatureAnimation.Dying)
    //   {
    //     _attachmentDisplay.ClearAttachments();
    //     return;
    //   }
    //
    //   // Ensure lower Y creatures are always rendered on top of higher Y creatures
    //   _sortingGroup.sortingOrder =
    //     100 - Mathf.RoundToInt(transform.position.y * 10) - Mathf.RoundToInt(transform.position.x);
    //
    //   if (_state == CreatureAnimation.Placing)
    //   {
    //     return;
    //   }
    //
    //   if (CanUseSkill())
    //   {
    //     TryToUseSkill();
    //   }
    //
    //   transform.eulerAngles = _data.BaseType.Owner == PlayerName.Enemy ? new Vector3(x: 0, y: 180, z: 0) : Vector3.zero;
    //
    //   if (transform.position.x > Constants.CreatureDespawnRightX ||
    //       transform.position.x < Constants.CreatureDespawnLeftX)
    //   {
    //     DestroyCreature();
    //   }
    //   else if (Owner == PlayerName.Enemy &&
    //            transform.position.x < Constants.EnemyCreatureEndzoneX)
    //   {
    //     Invoke(new IOnEnemyCreatureAtEndzone.Data(AsCreatureState()));
    //   }
    //
    //   var health = _data.GetInt(Stat.Health);
    //   if (health > 0)
    //   {
    //     _statusBars.HealthBar.Value = (health - _damageTaken) / (float) health;
    //   }
    //
    //   _statusBars.HealthBar.gameObject.SetActive(_damageTaken > 0);
    //
    //   var pos = Root.Instance.MainCamera.WorldToScreenPoint(_healthbarAnchor.position);
    //   _statusBars.transform.position = pos;
    //
    //   var speedMultiplier = _data.Stats.Get(Stat.SkillSpeedMultiplier).AsMultiplier();
    //   Errors.CheckState(speedMultiplier > 0.05f, "Skill speed must be > 5%");
    //   _animator.SetFloat(SkillSpeed, speedMultiplier);
    //
    //   if (_state == CreatureAnimation.Moving)
    //   {
    //     _animator.SetBool(Moving, value: true);
    //     transform.Translate(Vector3.right * (Time.deltaTime * (_data.GetInt(Stat.CreatureSpeed) / 1000f)));
    //   }
    //   else
    //   {
    //     _animator.SetBool(Moving, value: false);
    //   }
    //
    //   _attachmentDisplay.SetStatusEffects(_registry, _data.Stats.StatusEffects);
    // }

    static ImmutableList<Effect> Raise(
      GameContext c, CreatureState state, ImmutableList<Effect> effects, IEventData data) =>
      effects.AddRange(data.Raise(c,
        state.CurrentSkill != null ? state.CurrentSkill.DelegateList : state.Data.DelegateList));

    // void Invoke(IEventData data)
    // {
    //   var delegateList = _currentSkill != null ? _currentSkill.DelegateList : _data.DelegateList;
    //   var context = CreateContext();
    //   var eventQueue = new Queue<IEventData>();
    //   eventQueue.Enqueue(data);
    //
    //   while (true)
    //   {
    //     var effects = eventQueue.Dequeue().Raise(context, delegateList);
    //
    //     foreach (var effect in effects)
    //     {
    //       effect.Execute(_registry);
    //       foreach (var eventData in effect.Events())
    //       {
    //         eventQueue.Enqueue(eventData);
    //       }
    //     }
    //
    //     if (eventQueue.Count == 0)
    //     {
    //       break;
    //     }
    //   }
    // }

    public void EditorSetReferences(Transform projectileSource,
      Transform healthbarAnchor,
      AttachmentDisplay attachmentDisplay)
    {
      _projectileSource = projectileSource;
      _healthbarAnchor = healthbarAnchor;
      _attachmentDisplay = attachmentDisplay;
    }

    ImmutableList<Effect> ActivateCreature(
      GameContext c, CreatureState state, ImmutableList<Effect> results)
    {
      if (state.RankPosition.HasValue && state.FilePosition.HasValue)
      {
        transform.position = new Vector2(
          state.RankPosition.Value.ToXPosition(),
          state.FilePosition.Value.ToYPosition());
      }
      else if (state.StartingX.HasValue && state.FilePosition.HasValue)
      {
        transform.position = new Vector2(
          state.StartingX.Value,
          state.FilePosition.Value.ToYPosition());
      }

      _collider.enabled = true;

      Root.Instance.HelperTextService.OnCreaturePlayed();
      return Raise(c, state, results, new IOnCreatureActivated.Data(state));
    }

    // public void ActivateCreature(RankValue? rankValue,
    //   FileValue fileValue,
    //   float? startingX = 0f)
    // {
    //   _filePosition = fileValue;
    //
    //   if (rankValue.HasValue)
    //   {
    //     _rankPosition = rankValue;
    //     transform.position = new Vector2(
    //       rankValue.Value.ToXPosition(),
    //       fileValue.ToYPosition());
    //   }
    //   else if (startingX.HasValue)
    //   {
    //     transform.position = new Vector2(
    //       startingX.Value,
    //       fileValue.ToYPosition());
    //   }
    //
    //   _collider.enabled = true;
    //
    //   ToDefaultState();
    //
    //   Invoke(new IOnCreatureActivated.Data(AsCreatureState()));
    //   Root.Instance.HelperTextService.OnCreaturePlayed();
    //
    //   _coroutine = StartCoroutine(RunCoroutine());
    // }

    // public void InsertModifier(IStatModifier modifier)
    // {
    //   _data = _data.WithStats(_data.Stats.InsertModifier(modifier));
    // }
    //
    // public void InsertStatusEffect(StatusEffectData statusEffectData)
    // {
    //   _data = _data.WithStats(_data.Stats.InsertStatusEffect(statusEffectData));
    // }
    //
    // public void ExecuteMutatation(IMutation mutation)
    // {
    //   _data = _data.WithKeyValueStore(mutation.Mutate(_data.KeyValueStore));
    // }
    //
    // DelegateContext CreateContext() => new DelegateContext(_registry);

    static CreatureState TryToUseSkill(GameContext c, CreatureState state)
    {
      if (!CanUseSkill(state)) return state;

      var skill = state.Data.DelegateList.FirstNonNull(c, new ISelectSkill.Data(state));
      return skill == null
        ? ToDefaultState(state)
        : state.WithAnimation(CreatureAnimation.UsingSkill).WithCurrentSkill(skill);
    }

    void PlaySkillAnimation(CreatureData data, SkillData currentSkill)
    {
      var skillAnimation = SelectAnimation(data, currentSkill);
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
    }

    // void TryToUseSkill()
    // {
    //   Errors.CheckState(CanUseSkill(), "Cannot use skill");
    //
    //   var skill = _data.DelegateList.FirstNonNull(CreateContext(), new ISelectSkill.Data(AsCreatureState()));
    //   if (skill != null)
    //   {
    //     SetState(CreatureAnimation.UsingSkill);
    //     _currentSkill = skill;
    //
    //     var skillAnimation = SelectAnimation(_currentSkill);
    //     switch (skillAnimation)
    //     {
    //       case SkillAnimationNumber.Skill1:
    //         _animator.SetTrigger(Skill1);
    //         break;
    //       case SkillAnimationNumber.Skill2:
    //         _animator.SetTrigger(Skill2);
    //         break;
    //       case SkillAnimationNumber.Skill3:
    //         _animator.SetTrigger(Skill3);
    //         break;
    //       case SkillAnimationNumber.Skill4:
    //         _animator.SetTrigger(Skill4);
    //         break;
    //       case SkillAnimationNumber.Skill5:
    //         _animator.SetTrigger(Skill5);
    //         break;
    //       case SkillAnimationNumber.Unknown:
    //       default:
    //         throw Errors.UnknownEnumValue(skillAnimation);
    //     }
    //
    //     Invoke(new IOnSkillStarted.Data(AsCreatureState(), _currentSkill));
    //   }
    //   else
    //   {
    //     ToDefaultState();
    //   }
    // }

    static SkillAnimationNumber SelectAnimation(CreatureData data, SkillData skill)
    {
      var choices = data.BaseType.SkillAnimations
        .Where(a => a.SkillAnimationType == skill.BaseType.SkillAnimationType).ToList();
      Errors.CheckState(choices.Count > 0,
        $"Creature is unable to perform animation of type {skill.BaseType.SkillAnimationType}");
      return choices[Random.Range(minInclusive: 0, choices.Count)].SkillAnimationNumber;
    }

    static CreatureState ToDefaultState(CreatureState state) =>
      state.WithAnimation(state.GetInt(Stat.CreatureSpeed) > 0 ? CreatureAnimation.Moving : CreatureAnimation.Idle);

    // void ToDefaultState()
    // {
    //   SetState(_data.GetInt(Stat.CreatureSpeed) > 0 ? CreatureAnimation.Moving : CreatureAnimation.Idle);
    // }

    ImmutableList<Effect> Kill(GameContext c, CreatureState state, ImmutableList<Effect> results)
    {
      _statusBars.HealthBar.gameObject.SetActive(value: false);
      _animator.SetTrigger(Death);
      _collider.enabled = false;
      _attachmentDisplay.ClearAttachments();
      return Raise(c, state, results, new IOnCreatureDeath.Data(state));
    }

    // void Kill()
    // {
    //   StopCoroutine(_coroutine);
    //
    //   _statusBars.HealthBar.gameObject.SetActive(value: false);
    //   _animator.SetTrigger(Death);
    //   _collider.enabled = false;
    //
    //   Invoke(new IOnCreatureDeath.Data(AsCreatureState()));
    //
    //   SetState(CreatureAnimation.Dying);
    //   _creatureService.RemoveCreature(this);
    // }

    public void DestroyCreature()
    {
      Destroy(gameObject);
      Destroy(_statusBars.gameObject);
    }

    // public void OnImpact(IOnSkillImpact.Data data) => Invoke(data);
    //
    // // Called by skill animations on their 'start impact' frame
    // public void AttackStart()
    // {
    //   if (!IsAlive() || _currentSkill == null)
    //   {
    //     return;
    //   }
    //
    //   Invoke(new IOnSkillUsed.Data(AsCreatureState(), _currentSkill));
    //
    //   if (_currentSkill.IsMelee())
    //   {
    //     Invoke(new IOnSkillImpact.Data(AsCreatureState(), _currentSkill, projectile: null));
    //   }
    // }
    //
    // public void AddDamage(Creature2 appliedBy, int damage)
    // {
    //   Errors.CheckArgument(damage >= 0, "Damage must be non-negative");
    //   var health = _data.GetInt(Stat.Health);
    //   _damageTaken = Mathf.Clamp(value: 0, _damageTaken + damage, health);
    //   if (_damageTaken >= health)
    //   {
    //     appliedBy.Invoke(new IOnKilledEnemy.Data(appliedBy.AsCreatureState()));
    //     Kill();
    //   }
    // }
    //
    // public void Heal(int healing)
    // {
    //   Errors.CheckArgument(healing >= 0, "Healing must be non-negative");
    //   var health = _data.GetInt(Stat.Health);
    //   _damageTaken = Mathf.Clamp(value: 0, _damageTaken - healing, health);
    // }
    //
    // /// <summary>Called when the creature's animator state machine completes an action animation.</summary>
    // public void OnActionAnimationCompleted()
    // {
    //   if (!IsAlive() || IsStunned())
    //     // Ignore exit states from skills that ended early, e.g. due to stun
    //   {
    //     return;
    //   }
    //
    //   ToDefaultState();
    // }
    //
    // /// <summary>Called when the creature's animator state machine completes playing the death animation.</summary>
    // public void OnDeathAnimationCompleted()
    // {
    //   DestroyCreature();
    // }
    //
    // public void Stun(float durationSeconds)
    // {
    //   if (!IsAlive() || IsStunned())
    //   {
    //     return;
    //   }
    //
    //   StartCoroutine(StunAsync(durationSeconds));
    // }
    //
    // IEnumerator<YieldInstruction> StunAsync(float durationSeconds)
    // {
    //   _animator.SetTrigger(Hit);
    //   SetState(CreatureAnimation.Stunned);
    //   yield return new WaitForSeconds(durationSeconds);
    //   ToDefaultState();
    // }
    //
    // public bool IsAlive() => _state != CreatureAnimation.Placing && _state != CreatureAnimation.Dying;
    //
    // public bool IsStunned() => _state == CreatureAnimation.Stunned;
    //
    // public bool HasProjectileSkill()
    // {
    //   return _data.Skills.Any(s => s.IsProjectile());
    // }
    //
    static bool CanUseSkill(CreatureState state) =>
      state.Animation == CreatureAnimation.Idle || state.Animation == CreatureAnimation.Moving;

    // bool CanUseSkill() => _state == CreatureAnimation.Idle || _state == CreatureAnimation.Moving;
    //
    // public void MarkSkillUsed(int skillId)
    // {
    //   _skillLastUsedTimes[skillId] = Time.time;
    // }
    //
    // /// <summary>
    // ///   Returns the timestamp at which the provided skill was last used by this creature, or 0 if it has never been used
    // /// </summary>
    // public float? TimeLastUsedSeconds(int skillId) =>
    //   _skillLastUsedTimes.ContainsKey(skillId) ? (float?) _skillLastUsedTimes[skillId] : null;
    //
    // IEnumerator<YieldInstruction> RunCoroutine()
    // {
    //   while (true)
    //   {
    //     yield return new WaitForSeconds(seconds: 1);
    //     Heal(_data.GetInt(Stat.HealthRegenerationPerSecond));
    //   }
    //
    //   // ReSharper disable once IteratorNeverReturns
    // }
    //
    // void SetState(CreatureAnimation @new)
    // {
    //   _state = @new;
    // }
  }
}