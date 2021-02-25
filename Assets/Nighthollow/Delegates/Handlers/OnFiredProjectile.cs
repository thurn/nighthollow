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
using Nighthollow.Data;
using Nighthollow.Delegates.Effects;

#nullable enable

namespace Nighthollow.Delegates.Handlers
{
  public interface IOnFiredProjectile : IHandler
  {
    public sealed class Data : EventData<IOnFiredProjectile>
    {
      public Data(CreatureState self, SkillData skill, FireProjectileEffect effect)
      {
        Self = self;
        Skill = skill;
        Effect = effect;
      }

      public override IEnumerable<Effect> Invoke(DelegateContext c, int delegateIndex, IOnFiredProjectile handler) =>
        handler.OnFiredProjectile(c, delegateIndex, this);

      public CreatureState Self { get; }
      public SkillData Skill { get; }
      public FireProjectileEffect Effect { get; }
    }

    /// <summary>Called when the creature fires a projectile.</summary>
    IEnumerable<Effect> OnFiredProjectile(DelegateContext context, int delegateIndex, Data data);
  }
}