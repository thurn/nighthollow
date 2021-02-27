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
using System.Linq;
using Nighthollow.Delegates;
using Nighthollow.Services;
using Nighthollow.State;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Data
{
  public enum CreatureAnimation
  {
    Placing,
    Idle,
    Moving,
    UsingSkill,
    Dying,
    Stunned
  }

  public sealed partial class CreatureData : StatEntity
  {
    public CreatureData(
      DelegateList delegateList,
      StatTable stats,
      ImmutableList<SkillData> skills,
      CreatureTypeData baseType,
      CreatureItemData itemData,
      KeyValueStore keyValueStore)
    {
      DelegateList = delegateList;
      Stats = stats;
      Skills = skills;
      BaseType = baseType;
      ItemData = itemData;
      KeyValueStore = keyValueStore;
    }

    [Field] public DelegateList DelegateList { get; }
    [Field] public override StatTable Stats { get; }
    [Field] public ImmutableList<SkillData> Skills { get; }
    [Field] public CreatureTypeData BaseType { get; }
    [Field] public CreatureItemData ItemData { get; }
    [Field] public KeyValueStore KeyValueStore { get; }

    public CreatureData OnTick() => WithStats(Stats.OnTick())
      .WithSkills(Skills.Select(skill => skill.WithStats(skill.Stats.OnTick())).ToImmutableList());
  }
}