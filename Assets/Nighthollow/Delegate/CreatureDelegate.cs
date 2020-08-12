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

using Nighthollow.Components;
using Nighthollow.Data;
using Nighthollow.Services;
using Nighthollow.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nighthollow.Delegate
{
  public abstract class CreatureDelegate : ScriptableObject
  {
    /// <summary>Called when a creature is first placed.</summary>
    public virtual void OnActivate(Creature self)
    {
    }

    public virtual CreatureDelegate Clone()
    {
      return Instantiate(this);
    }

    /// <summary>
    /// Called when a creature is ready to use a skill in order to determine which skill
    /// to select. This happens on activation, when a previous skill completes, or on
    /// collision while the creature is idle.
    /// </summary>
    public virtual bool ShouldUseMeleeSkill(Creature self)
    {
      return self.HasMelee() && GetCollidingCreatures(self.Owner, self.Collider).Any();
    }

    /// <summary>
    /// Equivalent to 'ShouldUseMeleeSkill' for projectile attacks. Note that melee skills
    /// take precedence if both methods return true.
    /// </summary>
    public virtual bool ShouldUseProjectileSkill(Creature self)
    {
      return self.HasProjectile() &&
        GetCollidingCreatures(self.Owner, self.ProjectileCollider.Collider).Any();
    }

    /// <summary>Called when the creature reaches the firing frame of its cast animation.</summary>
    public virtual void OnFireProjectile(Creature self)
    {
      var projectile = Root.Instance.ObjectPoolService.Create(
        self.Data.Projectile.Prefab, self.ProjectileSource.position);
      projectile.Initialize(self, self.Data.Projectile, self.ProjectileSource);
    }

    /// <summary>Called when the creature reaches the hit frame of its attack animation.</summary>
    public virtual void OnMeleeHit(Creature self)
    {
      foreach (var creature in GetCollidingCreatures(self.Owner, self.Collider))
      {
        creature.ApplyDamage(self.Data.BaseAttack.Total());
      }
    }

    /// <summary>Called when a projectile fired by this creature hits a target.</summary>
    public virtual void OnProjectileImpact(Creature self, Projectile projectile)
    {
      foreach (var creature in OverlapBox(
        self.Owner,
        projectile.transform.position,
        new Vector2(self.Data.Projectile.HitboxSize / 1000f, self.Data.Projectile.HitboxSize / 1000f)))
      {
        creature.ApplyDamage(self.Data.BaseAttack.Total());
      }
    }

    /// <summary>Returns a list of the enemies of 'owner' overlapping wtih 'collider'.</summary>
    protected static IEnumerable<Creature> GetCollidingCreatures(PlayerName owner, Collider2D collider)
    {
      return OverlapBox(
        owner,
        collider.bounds.center,
        collider.bounds.size);
    }

    /// <summary>
    /// Returns a list of the enemies of 'owner' overlapping wtih a rectangle at 'center' of
    /// size 'size'.
    /// </summary>
    protected static IEnumerable<Creature> OverlapBox(PlayerName owner, Vector2 center, Vector2 size)
    {
      var hits = Physics2D.OverlapBoxAll(
        center,
        size,
        angle: 0,
        Constants.LayerMaskForCreatures(owner.GetOpponent()));

      foreach (var hit in hits)
      {
        var creature = hit.GetComponent<Creature>();
        if (creature)
        {
          yield return creature;
        }
      }
    }
  }
}
