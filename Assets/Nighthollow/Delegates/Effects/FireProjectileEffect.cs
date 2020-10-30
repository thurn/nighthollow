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

#nullable enable

using System.Collections.Generic;
using Nighthollow.Components;
using Nighthollow.Data;
using Nighthollow.Services;
using Nighthollow.Utils;
using UnityEngine;

namespace Nighthollow.Delegates.Effects
{
  public sealed class FireProjectileEffect : Effect
  {
    public Creature FiredBy { get; }
    public SkillData SkillData { get; }
    public int DelegateIndex { get; }
    public Vector2 FiringPoint { get; }
    public Vector2 FiringDirectionOffset { get; }
    public int FiringDelayMs { get; }

    public FireProjectileEffect(
      Creature firedBy,
      SkillData skillData,
      int delegateIndex,
      Vector2 firingPoint,
      Vector2 firingDirectionOffset,
      int firingDelayMs = 0)
    {
      FiredBy = firedBy;
      SkillData = skillData;
      DelegateIndex = delegateIndex;
      FiringPoint = firingPoint;
      FiringDirectionOffset = firingDirectionOffset;
      FiringDelayMs = firingDelayMs;
    }

    public override void Execute()
    {
      FiredBy.StartCoroutine(FireAsync());
    }

    public override void RaiseEvents()
    {

    }

    IEnumerator<YieldInstruction> FireAsync()
    {
      yield return new WaitForSeconds(FiringDelayMs / 1000f);
      var projectile = Root.Instance.AssetService.InstantiatePrefab<Projectile>(
        Errors.CheckNotNull(SkillData.BaseType.Address));
      projectile.Initialize(FiredBy, SkillData, FiringPoint, FiringDirectionOffset);
    }
  }
}