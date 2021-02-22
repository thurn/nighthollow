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
using Nighthollow.Delegates;
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
    [Header("Config")] [SerializeField] TimedEffect _flashEffect = null!;
    [SerializeField] TimedEffect _hitEffect = null!;

    [Header("State")] [SerializeField] Creature _firedBy = null!;
    [SerializeField] Collider2D _collider = null!;
    [SerializeField] Creature _trackCreature = null!;
    SkillData _skillData = null!;
    GameServiceRegistry _registry = null!;

    public SkillData Data => _skillData;
    public Collider2D Collider => _collider;
    public KeyValueStore KeyValueStore { get; set; } = new KeyValueStore();

    public void Initialize(
      GameServiceRegistry registry,
      Creature firedBy,
      SkillData skillData,
      FireProjectileEffect effect)
    {
      _registry = registry;
      _firedBy = firedBy;
      transform.position = effect.FiringPoint;

      if (effect.TrackCreature)
      {
        transform.LookAt(effect.TrackCreature!.transform.position);
        _trackCreature = effect.TrackCreature;
      }
      else
      {
        transform.forward = effect.FiringDirectionOffset.GetValueOrDefault()
                            + Constants.ForwardDirectionForPlayer(firedBy.Owner);
      }

      gameObject.layer = Constants.LayerForProjectiles(firedBy.Owner);

      _collider = GetComponent<Collider2D>();

      var flash = Root.Instance.ObjectPoolService.Create(_flashEffect.gameObject, transform.position);
      flash.transform.forward = transform.forward;
      _skillData = skillData;
    }

    public void ExecuteMutatation(IMutation mutation)
    {
      KeyValueStore = mutation.Mutate(KeyValueStore);
    }

    void Update()
    {
      if (_trackCreature)
      {
        transform.LookAt(_trackCreature.transform.position);
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
      if (_firedBy.CurrentSkill!.NewDelegate.First(
        new DelegateContext(_registry),
        new IShouldSkipProjectileImpact.Data(_firedBy.AsCreatureState(), _skillData, this),
        notFound: false))
      {
        if (!_trackCreature || _trackCreature.gameObject != other.gameObject)
          // Tracking projectiles cannot skip their target
        {
          return;
        }
      }

      var hit = Root.Instance.ObjectPoolService.Create(_hitEffect.gameObject, transform.position);
      hit.transform.forward = -transform.forward;
      _firedBy.OnImpact(new IOnSkillImpact.Data(_firedBy.AsCreatureState(), _skillData, this));
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
