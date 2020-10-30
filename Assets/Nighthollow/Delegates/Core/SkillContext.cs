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
using Nighthollow.Data;
using Nighthollow.Stats;

namespace Nighthollow.Delegates.Core
{
  public sealed class SkillContext : DelegateContext<SkillContext>
  {
    public Creature Self { get; }
    public SkillData Skill { get; }
    public Projectile? Projectile { get; }
    public ISkillDelegate Delegate => Skill.Delegate;

    public SkillContext(Creature self, SkillData skill, Projectile? projectile = null) : base(new Results())
    {
      Self = self;
      Skill = skill;
      Projectile = projectile;
    }

    public override StatTable Stats => Skill.Stats;

    public override SkillContext New() => new SkillContext(Self, Skill, Projectile);
  }
}