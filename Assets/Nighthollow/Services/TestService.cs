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
using System.Collections.Immutable;
using MessagePack;
using Nighthollow.Data;
using Nighthollow.Generated;
using Nighthollow.Stats;
using UnityEngine;

#nullable enable

namespace Nighthollow.Services
{
  public sealed class TestService : MonoBehaviour
  {
    void Start()
    {
      var gameData = TestData();
      var bin = MessagePackSerializer.Serialize(gameData);
    }

    GameData TestData()
    {
      var creatures = ImmutableDictionary.CreateBuilder<int, CreatureTypeData>();
      var testCreature = new CreatureTypeData(
        name: "Test",
        prefabAddress: "/My/Address",
        owner: PlayerName.User,
        health: new IntRangeValue(100, 200),
        skillAnimations: new Dictionary<SkillAnimationNumber, SkillAnimationType>
          {{SkillAnimationNumber.Skill1, SkillAnimationType.MeleeSkill}}.ToImmutableDictionary(),
        baseManaCost: 50,
        implicitAffix: new AffixTypeData(
          minLevel: 1,
          weight: 1,
          manaCost: IntRangeValue.Zero,
          modifiers: ImmutableList.Create(
            new ModifierTypeData(
              statId: StatId.Health,
              statOperator: Operator.Add,
              valueLow: new IntValueData(25),
              valueHigh: new IntValueData(75)))));

      var testCreature2 = new CreatureTypeData(
        name: "Test Two",
        prefabAddress: "/My/Address/Two",
        owner: PlayerName.Enemy,
        health: new IntRangeValue(50, 200),
        skillAnimations: new Dictionary<SkillAnimationNumber, SkillAnimationType>
          {{SkillAnimationNumber.Skill3, SkillAnimationType.CastSkill}}.ToImmutableDictionary(),
        baseManaCost: 100,
        implicitAffix: new AffixTypeData(
          minLevel: 1,
          weight: 1,
          manaCost: IntRangeValue.Zero,
          modifiers: ImmutableList.Create(
            new ModifierTypeData(
              statId: StatId.Health,
              statOperator: Operator.Add,
              valueLow: new IntValueData(50),
              valueHigh: new IntValueData(75)))));

      creatures[1] = testCreature;
      creatures[2] = testCreature2;

      return new GameData(
        creatureTypes: creatures.ToImmutable(),
        affixTypes: ImmutableDictionary<int, AffixTypeData>.Empty,
        skillTypes: ImmutableDictionary<int, SkillTypeData>.Empty);
    }
  }
}
