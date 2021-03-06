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
using Nighthollow.Delegates.Effects;
using Nighthollow.Delegates.Handlers;
using Nighthollow.Services;
using Nighthollow.State;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Components
{
  public sealed class Projectile : MonoBehaviour, IKeyValueStoreOwner
  {
    [SerializeField] TimedEffect _flashEffect = null!;
    [SerializeField] TimedEffect _hitEffect = null!;
    [SerializeField] Collider2D _collider = null!;
    CreatureId _firedById;
    SkillData _skillData = null!;
    CreatureId? _trackCreature;
    BattleServiceRegistry _registry = null!;

    public SkillData Data => _skillData;
    public Collider2D Collider => _collider;
    public KeyValueStore KeyValueStore { get; set; } = new KeyValueStore();

    public void Initialize(
      BattleServiceRegistry registry,
      CreatureState firedBy,
      SkillData skillData,
      FireProjectileEffect effect)
    {
      _registry = registry;
      _firedById = firedBy.CreatureId;
      transform.position = effect.FiringPoint;

      if (effect.TrackCreature.HasValue)
      {
        _trackCreature = effect.TrackCreature;
        transform.LookAt(registry.Creatures.GetPosition(_trackCreature.Value));
      }
      else
      {
        transform.forward = effect.FiringDirectionOffset.GetValueOrDefault()
                            + Constants.ForwardDirectionForPlayer(firedBy.Owner);
      }

      gameObject.layer = Constants.LayerForProjectiles(firedBy.Owner);

      _collider = GetComponent<Collider2D>();

      var flash = _registry.ObjectPoolService.Create(_flashEffect.gameObject, transform.position);
      flash.transform.forward = transform.forward;
      _skillData = skillData;
    }

    public void ExecuteMutatation(IMutation mutation)
    {
      KeyValueStore = mutation.Mutate(KeyValueStore);
    }

    void Update()
    {
      if (_trackCreature.HasValue)
      {
        transform.LookAt(_registry.Creatures.GetPosition(_trackCreature.Value));
      }

      transform.position += Errors.CheckPositive(_skillData.GetInt(Stat.ProjectileSpeed)) / 1000f
                            * Time.deltaTime * transform.forward;

      if (Mathf.Abs(transform.position.x) > 25 || Mathf.Abs(transform.position.y) > 25)
      {
        gameObject.SetActive(value: false);
      }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
      var firedBy = _registry.Creatures[_firedById];
      if (firedBy.CurrentSkill != null &&
          firedBy.CurrentSkill.DelegateList.First(
            _registry,
            new IShouldSkipProjectileImpact.Data(_firedById, _skillData, this),
            notFound: false))
      {
        if (_trackCreature == null || _trackCreature != ComponentUtils.GetComponent<Creature>(other).CreatureId)
          // Tracking projectiles cannot skip their target
        {
          return;
        }
      }

      var hit = _registry.ObjectPoolService.Create(_hitEffect.gameObject, transform.position);
      hit.transform.forward = -transform.forward;
      _registry.Invoke(new IOnSkillImpact.Data(_firedById, _skillData, this));
      gameObject.SetActive(value: false);
    }

    void OnValidate()
    {
      foreach (var ps in GetComponentsInChildren<ParticleSystem>())
      {
        // We give projectiles a lifetime of 3 seconds, which is approximately enough
        // time to get across the screen at a speed of 10,000.
        var main = ps.main;
        main.startLifetime = 3.0f;
        ps.GetComponent<Renderer>().sortingOrder = 500;
      }
    }

    public void SetReferences(TimedEffect flashEffect, TimedEffect hitEffect)
    {
      _flashEffect = flashEffect;
      _hitEffect = hitEffect;
    }
  }
}