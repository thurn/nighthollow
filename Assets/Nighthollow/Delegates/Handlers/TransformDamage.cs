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

using System.Collections.Immutable;
using Nighthollow.Data;
using Nighthollow.Services;

#nullable enable

namespace Nighthollow.Delegates.Handlers
{
  public interface ITransformDamage : IHandler
  {
    public sealed class Data : IteratedQueryData<ITransformDamage, ImmutableDictionary<DamageType, int>>
    {
      public Data(CreatureId self, SkillData skill, CreatureId target)
      {
        Self = self;
        Skill = skill;
        Target = target;
      }

      public override ImmutableDictionary<DamageType, int> Invoke(
        IGameContext c,
        ITransformDamage handler,
        ImmutableDictionary<DamageType, int> current) =>
        handler.TransformDamage(c, this, current);

      public CreatureId Self { get; }
      public SkillData Skill { get; }
      public CreatureId Target { get; }
    }

    /// <summary>
    /// Given the base damage returned from <see cref="IRollForBaseDamage" />, delegates can transform the damage value
    /// before it is passed to <see cref="IComputeFinalDamage" />. Each delegate's implementation of this method will be
    /// invoked in sequence with the return value of the previous delegate.
    /// </summary>
    ImmutableDictionary<DamageType, int> TransformDamage(
      IGameContext context,
      Data data,
      ImmutableDictionary<DamageType, int> current);
  }
}