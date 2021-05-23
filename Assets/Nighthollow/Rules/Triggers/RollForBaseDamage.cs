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

namespace Nighthollow.Rules.Triggers
{
  public interface IRollForBaseDamage
  {
    public sealed class Data : ITrigger<IRollForBaseDamage, ImmutableDictionary<DamageType, int>>
    {
      public Data(CreatureId self, SkillData skill, CreatureId target)
      {
        Self = self;
        Skill = skill;
        Target = target;
      }

      public ImmutableDictionary<DamageType, int> Handle(Injector injector, int index, IRollForBaseDamage handler) =>
        handler.RollForBaseDamage(injector, index, this);

      public TriggerName Name => TriggerName.RollForBaseDamage;
      public CreatureId Self { get; }
      public SkillData Skill { get; }
      public CreatureId Target { get; }
    }

    /// <summary>
    /// Should compute the base damage for this skill, randomly selecting from within its base damage range.
    /// </summary>
    ImmutableDictionary<DamageType, int> RollForBaseDamage(Injector injector, int delegateIndex, Data data);
  }
}