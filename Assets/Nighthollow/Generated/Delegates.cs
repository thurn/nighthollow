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
using Nighthollow.Delegates.Core;
using Nighthollow.Delegates.Creatures;
using Nighthollow.Delegates.Skills;

// Generated Code - Do Not Modify!
namespace Nighthollow.Generated
{
  public enum CreatureDelegateId
  {
    Unknown = 0,
    DefaultCreatureDelegate = 1,
    MultipleProjectilesDelegate = 2,
    ProjectileArcDelegate = 3,
    AdjacentFileProjectilesDelegate = 4,
    ChainingProjectilesDelegate = 5,
    MeleeKnockbackDelegate = 6,
    ManaGenerationDelegate = 7,
    InfluenceAddedDelegate = 8,
  }

  public static class CreatureDelegateMap
  {
    static readonly Dictionary<CreatureDelegateId, ICreatureDelegate> Delegates = new
        Dictionary<CreatureDelegateId, ICreatureDelegate>
    {
      {CreatureDelegateId.DefaultCreatureDelegate, new DefaultCreatureDelegate()},
      {CreatureDelegateId.MultipleProjectilesDelegate, new MultipleProjectilesDelegate()},
      {CreatureDelegateId.ProjectileArcDelegate, new ProjectileArcDelegate()},
      {CreatureDelegateId.AdjacentFileProjectilesDelegate, new AdjacentFileProjectilesDelegate()},
      {CreatureDelegateId.ChainingProjectilesDelegate, new ChainingProjectilesDelegate()},
      {CreatureDelegateId.MeleeKnockbackDelegate, new MeleeKnockbackDelegate()},
      {CreatureDelegateId.ManaGenerationDelegate, new ManaGenerationDelegate()},
      {CreatureDelegateId.InfluenceAddedDelegate, new InfluenceAddedDelegate()},
    };

    public static ICreatureDelegate Get(CreatureDelegateId id) => Delegates[id];
  }

  public enum SkillDelegateId
  {
    Unknown = 0,
    DefaultSkillDelegate = 1,
  }

  public static class SkillDelegateMap
  {
    static readonly Dictionary<SkillDelegateId, ISkillDelegate> Delegates = new
        Dictionary<SkillDelegateId, ISkillDelegate>
    {
      {SkillDelegateId.DefaultSkillDelegate, new DefaultSkillDelegate()},
    };

    public static ISkillDelegate Get(SkillDelegateId id) => Delegates[id];
  }

}
