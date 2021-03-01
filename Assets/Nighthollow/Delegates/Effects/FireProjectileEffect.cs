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
using DG.Tweening;
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
      CreatureId firedBy,
      SkillData skill,
      int delegateIndex,
      Vector2 firingPoint,
      Vector2? firingDirectionOffset = null,
      CreatureId? trackCreature = null,
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

    public CreatureId FiredBy { get; }
    public SkillData Skill { get; }
    public int DelegateIndex { get; }
    public Vector2 FiringPoint { get; }
    public Vector2? FiringDirectionOffset { get; }
    public CreatureId? TrackCreature { get; }
    public int FiringDelayMs { get; }
    public KeyValueStore? Values { get; }

    public override void Execute(GameServiceRegistry registry)
    {
      DOTween.Sequence().InsertCallback(FiringDelayMs / 1000f, () => FireAsync(registry));
    }

    public override IEnumerable<IEventData> Events(IGameContext c)
    {
      yield return new IOnFiredProjectile.Data(c.Creatures[FiredBy], Skill, this);
    }

    void FireAsync(GameServiceRegistry registry)
    {
      var projectile = registry.AssetService.InstantiatePrefab<Projectile>(
        Errors.CheckNotNull(Skill.BaseType.Address));
      if (Values != null)
      {
        projectile.KeyValueStore = Values;
      }

      projectile.Initialize(registry, registry.Creatures[FiredBy], Skill, this);
    }
  }
}
