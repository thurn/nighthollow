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
using UnityEngine;

namespace Nighthollow.Delegate
{
  /// <summary>
  /// Class which controls the behavior of creatures.
  /// </summary>
  ///
  /// The Creature component calls its delegate at various points to determine actions to take. A creature can have
  /// multiple Delegates, organized into a list by <see cref="CreatureDelegateList"/>. Each delegate implementation is
  /// responsible for calling the next delegate in the delegate chain when its methods are invoked, unless it is
  /// deliberately replacing the implementation of a delegate callback with new functionality. The final delegate is
  /// always <see cref="DefaultCreatureDelegate"/>, which has the default implementation of creature behavior.
  public abstract class AbstractCreatureDelegate : ScriptableObject
  {
    protected AbstractCreatureDelegate Parent { get; set; }

    public void SetParent(AbstractCreatureDelegate parent)
    {
      Parent = parent;
    }

    public abstract string Description();

    public virtual AbstractCreatureDelegate Clone() => Instantiate(this);

    /// <summary>Called when a creature is first placed.</summary>
    public virtual void OnActivate(Creature self)
    {
      Parent.OnActivate(self);
    }

    /// <summary>Called when a creature dies.</summary>
    public virtual void OnDeath(Creature self)
    {
      Parent.OnDeath(self);
    }

    /// <summary>
    /// Called when a creature is ready to use a skill. Should return true if the creature should
    /// use one of its untargeted skills before attacking.
    /// </summary>
    public virtual bool ShouldUseUntargetedSkill(Creature self)
    {
      return Parent.ShouldUseUntargetedSkill(self);
    }

    /// <summary>
    /// Called when a creature is ready to use a skill in order to determine which skill
    /// to select. Note that untargeted skills take precedence over targeted skills, so this is
    /// only invoked if 'ShouldUseUntargetedSkill' returns false.
    /// </summary>
    public virtual bool ShouldUseMeleeSkill(Creature self)
    {
      return Parent.ShouldUseMeleeSkill(self);
    }

    /// <summary>
    /// Equivalent to 'ShouldUseMeleeSkill' for projectile attacks. Note that melee skills
    /// take precedence if both methods return true.
    /// </summary>
    public virtual bool ShouldUseProjectileSkill(Creature self)
    {
      return Parent.ShouldUseProjectileSkill(self);
    }

    /// <summary>
    /// Called in order to pick a skill to use once the SkillType has been decided. Should
    /// return the skill to use or null to indicate that no appropriate skills are available.
    /// Must return a skill with energy cost less than or equal to self.CurrentEnergy.
    /// </summary>
    public virtual SkillData ChooseSkill(Creature self, SkillType skillType)
    {
      return Parent.ChooseSkill(self, skillType);
    }

    /// <summary>
    /// Called in order to pick a projectile to fire once skill animation reaches its 'cast'
    /// frame. Should return the projectile to fire. Must return a projectile with energy cost
    /// less than or equal to self.CurrentEnergy.
    /// </summary>
    public virtual ProjectileData ChooseProjectile(Creature self)
    {
      return Parent.ChooseProjectile(self);
    }

    /// <summary>Called when the creature reaches the firing frame of its cast animation.</summary>
    public virtual void OnFireProjectile(
      Creature self,
      ProjectileData projectileData,
      Vector2 firingPoint,
      Vector2? direction = null)
    {
      Parent.OnFireProjectile(self, projectileData, firingPoint, direction);
    }

    /// <summary>
    /// Called when the creature reaches the activation frame of an untargeted skill.
    /// </summary>
    public virtual void OnUseUntargetedSkill(Creature self, SkillData skill)
    {
      Parent.OnUseUntargetedSkill(self, skill);
    }

    /// <summary>Called when the creature reaches the hit frame of its attack animation.</summary>
    public virtual void OnMeleeHit(Creature self)
    {
      Parent.OnMeleeHit(self);
    }

    /// <summary>Called when a projectile fired by this creature hits a target.</summary>
    public virtual void OnProjectileImpact(Creature self, Projectile projectile)
    {
      Parent.OnProjectileImpact(self, projectile);
    }

    /// <summary>Called to apply damage as a result of a projectile impact.</summary>
    public virtual void ExecuteSpellAttack(
      Creature self,
      Creature target,
      Damage damage)
    {
      Parent.ExecuteSpellAttack(self, target, damage);
    }

    /// <summary>Called to apply damage as a result of a melee hit.</summary>
    public virtual void ExecuteMeleeAttack(
      Creature self,
      Creature target,
      Damage damage)
    {
      Parent.ExecuteMeleeAttack(self, target, damage);
    }

    /// <summary>Should return true if a melee hit on 'enemy' for 'damageAmount' damage should cause a stun.</summary>
    public virtual bool ShouldStun(Creature self, Creature enemy, int damageAmount)
    {
      return Parent.ShouldStun(self, enemy, damageAmount);
    }

    /// <summary>Called when a creature kills an enemy creature.</summary>
    public virtual void OnKilledEnemy(Creature self, Creature enemy, int damageAmount)
    {
      Parent.OnKilledEnemy(self, enemy, damageAmount);
    }
  }
}