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
using Nighthollow.Delegates.CreatureDelegates;
using Nighthollow.Generated;
using Nighthollow.Stats;

namespace Nighthollow.Model
{
  public sealed class CreatureData
  {
    public string PrefabAddress { get; }
    public PlayerName Owner { get; }
    public string Name { get; }
    public IReadOnlyCollection<SkillData> Skills { get; }
    public StatTable Stats { get; }
    public CreatureDelegateChain Delegate { get; }

    public CreatureData(
      string prefabAddress,
      PlayerName owner,
      string name,
      IReadOnlyCollection<SkillData> skills,
      StatTable stats,
      List<CreatureDelegateId> delegates)
    {
      PrefabAddress = prefabAddress;
      Owner = owner;
      Name = name;
      Skills = skills;
      Stats = stats;
      Delegate = new CreatureDelegateChain(delegates);
    }
  }
}