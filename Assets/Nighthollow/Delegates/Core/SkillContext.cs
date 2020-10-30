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
using Nighthollow.Utils;

namespace Nighthollow.Delegates.Core
{
  public sealed class SkillContext
  {
    public Creature Self { get; }
    public SkillData Skill { get; }
    public Projectile? Projectile { get; }
    readonly int? _delegateIndex;
    public int DelegateIndex => Errors.CheckNotNull(_delegateIndex);

    public SkillContext(Creature self, SkillData skill, Projectile? projectile = null, int? delegateIndex = null)
    {
      Self = self;
      Skill = skill;
      Projectile = projectile;
      _delegateIndex = delegateIndex;
    }

    public SkillContext WithIndex(int index) => new SkillContext(Self, Skill, Projectile, index);
  }
}