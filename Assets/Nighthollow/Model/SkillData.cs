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
using Nighthollow.Delegates.SkillDelegates;
using Nighthollow.Generated;
using Nighthollow.Stats;

namespace Nighthollow.Model
{
  public sealed class SkillData : AbstractGameEntity
  {
    public Optional<string> Address { get; }
    public SkillType SkillType { get; }
    public SkillAnimationNumber Animation { get; }
    public override StatTable Stats { get; }
    public SkillDelegateChain Delegate { get; }

    public SkillData(
      Optional<string> address,
      SkillType skillType,
      SkillAnimationNumber animation,
      StatTable stats,
      List<SkillDelegateId> delegateIds)
    {
      Address = address;
      SkillType = skillType;
      Animation = animation;
      Stats = stats;
      Delegate = new SkillDelegateChain(delegateIds);
    }

    SkillData(
      Optional<string> address,
      SkillType skillType,
      SkillAnimationNumber animation,
      StatTable stats,
      SkillDelegateChain delegates)
    {
      Address = address;
      SkillType = skillType;
      Animation = animation;
      Stats = stats;
      Delegate = delegates;
    }

    public SkillData Clone() => new SkillData(Address, SkillType, Animation, Stats.Clone(), Delegate);
  }
}