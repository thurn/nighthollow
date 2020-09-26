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
using SkillData = Nighthollow.Model.SkillData;

namespace Nighthollow.Delegates.CreatureDelegates
{
  public abstract class CreatureDelegate
  {
    /// <summary>Called when a creature is first placed.</summary>
    public virtual void OnActivate(CreatureContext c)
    {
    }

    /// <summary>Called when a creature dies.</summary>
    public virtual void OnDeath(CreatureContext c)
    {
    }

    /// <summary>
    /// Called when a creature wants to decide which skill to use. The *first* delegate to return a non-empty
    /// value will have its value be used, other delegates in the sequence will not be invoked.
    /// </summary>
    public virtual Optional<SkillData> SelectSkill(Creature self) => Optional<SkillData>.None();

    /// <summary>Called when the creature has fired a projectile.</summary>
    public virtual void OnFireProjectile(CreatureContext c, FireProjectileEffect effect)
    {
    }

    /// <summary>Called when the creature has hit an enemy with a melee attack.</summary>
    public virtual void OnMeleeHit(CreatureContext c, ApplyMeleeHitEffect effect)
    {
    }

    /// <summary>Called when the creature's projectile has hit an enemy.</summary>
    public virtual void OnProjectileImpact(CreatureContext c, Projectile projectile)
    {
    }

    /// <summary>Called when a creature kills an enemy creature.</summary>
    public virtual void OnKilledEnemy(CreatureContext c, Creature enemy, int damageAmount)
    {
    }
  }
}