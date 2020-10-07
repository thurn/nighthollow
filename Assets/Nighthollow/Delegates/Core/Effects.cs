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

namespace Nighthollow.Delegates.Core
{
  public abstract class DefaultEffect : CreatureEffect
  {
    // https://steve-yegge.blogspot.com/2006/03/execution-in-kingdom-of-nouns.html
    public abstract void Execute();

    public override void Execute(Creature _)
    {
      Execute();
    }
  }

  public abstract class CreatureEffect : SkillEffect
  {
    public abstract void Execute(Creature self);

    public virtual void InvokeEvent(CreatureDelegateChain chain, Creature self)
    {
    }

    public override void Execute(Creature self, SkillData skill)
    {
      Execute(self);
    }
  }

  public abstract class SkillEffect : TargetedSkillEffect
  {
    public abstract void Execute(Creature self, SkillData skill);

    public override void Execute(Creature self, SkillData skill, Creature target)
    {
      Execute(self, skill);
    }
  }

  public abstract class TargetedSkillEffect
  {
    public abstract void Execute(Creature self, SkillData skill, Creature target);
  }
}