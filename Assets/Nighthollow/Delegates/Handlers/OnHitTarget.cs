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
using Nighthollow.Services;

#nullable enable

namespace Nighthollow.Delegates.Handlers
{
  public interface IOnHitTarget : IHandler
  {
    public sealed class Data : CreatureEventData<IOnHitTarget>
    {
      public Data(CreatureId self, SkillData skill, CreatureId target, Projectile? projectile, int damage) : base(self)
      {
        Skill = skill;
        Target = target;
        Projectile = projectile;
        Damage = damage;
      }

      public override IEnumerable<Effect> Invoke(IGameContext c, int delegateIndex, IOnHitTarget handler) =>
        handler.OnHitTarget(c, delegateIndex, this);

      public SkillData Skill { get; }
      public CreatureId Target { get; }
      public Projectile? Projectile { get; }
      public int Damage { get; }
    }

    /// <summary>Called after one of the creature's skills has hit a target.</summary>
    IEnumerable<Effect> OnHitTarget(IGameContext context, int delegateIndex, Data data);
  }
}