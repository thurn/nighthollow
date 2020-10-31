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

using Nighthollow.Components;
using Nighthollow.Delegates.Effects;

namespace Nighthollow.Delegates.Core
{
  public interface ICreatureOrSkillDelegate<in TContext> : IDelegate where TContext : DelegateContext<TContext>
  {
    /// <summary>Called when a creature is first placed.</summary>
    void OnActivate(TContext c);

    /// <summary>Called when a creature dies.</summary>
    void OnDeath(TContext c);

    /// <summary>Called when a creature kills an enemy creature.</summary>
    void OnKilledEnemy(
      TContext c,
      Creature enemy,
      int damageAmount);

    /// <summary>Called when the creature fires a projectile.</summary>
    void OnFiredProjectile(TContext c, FireProjectileEffect effect);
  }
}