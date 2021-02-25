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
using Nighthollow.Components;
using Nighthollow.Data;
using Nighthollow.Delegates.Handlers;
using Nighthollow.Services;
using Nighthollow.State;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Delegates.Effects
{
  public sealed class FireProjectileEffect : Effect
  {
    public FireProjectileEffect(
      CreatureState firedBy,
      SkillData skill,
      int delegateIndex,
      Vector2 firingPoint,
      Vector2? firingDirectionOffset = null,
      Creature2? trackCreature = null,
      int firingDelayMs = 0,
      KeyValueStore? values = null)
    {
      FiredBy = firedBy;
      Skill = skill;
      DelegateIndex = delegateIndex;
      FiringPoint = firingPoint;
      FiringDirectionOffset = firingDirectionOffset;
      TrackCreature = trackCreature;
      FiringDelayMs = firingDelayMs;
      Values = values;
    }

    public CreatureState FiredBy { get; }
    public SkillData Skill { get; }
    public int DelegateIndex { get; }
    public Vector2 FiringPoint { get; }
    public Vector2? FiringDirectionOffset { get; }
    public Creature2? TrackCreature { get; }
    public int FiringDelayMs { get; }
    public KeyValueStore? Values { get; }

    public override void Execute(GameServiceRegistry registry)
    {
      FiredBy.Creature.StartCoroutine(FireAsync(registry));
    }

    public override IEnumerable<Effect> RaiseTriggeredEvents(GameContext c) =>
      new IOnFiredProjectile.Data(FiredBy, Skill, this).Raise(null!, null!);

    IEnumerator<YieldInstruction> FireAsync(GameServiceRegistry registry)
    {
      yield return new WaitForSeconds(FiringDelayMs / 1000f);
      var projectile = registry.AssetService.InstantiatePrefab<Projectile>(
        Errors.CheckNotNull(Skill.BaseType.Address));
      if (Values != null)
      {
        projectile.KeyValueStore = Values;
      }

      projectile.Initialize(registry, FiredBy.Creature, Skill, this);
    }
  }
}