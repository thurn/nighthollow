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
using Nighthollow.Data;
using Nighthollow.Generated;
using Nighthollow.Interface;
using Nighthollow.Stats;
using UnityEngine;
using Operator = Nighthollow.Data.Operator;
using PlayerName = Nighthollow.Data.PlayerName;
using SkillAnimationNumber = Nighthollow.Data.SkillAnimationNumber;
using SkillAnimationType = Nighthollow.Data.SkillAnimationType;

#nullable enable

namespace Nighthollow.World
{
  public sealed class WorldInitializer : MonoBehaviour
  {
    [SerializeField] WorldMap _worldMap = null!;
    [SerializeField] ScreenController _screenController = null!;
    [SerializeField] WorldTutorial _worldTutorial = null!;

    void Start()
    {
      _worldMap.Initialize();
      _screenController.Initialize();
      _screenController.Show(ScreenController.AdvisorBar);
      _worldTutorial.Initialize();

      _screenController.Get(ScreenController.GameDataEditor).Show(new GameDataEditor.Args(TestData()));
    }

    GameData TestData()
    {
      var creatures = ImmutableDictionary.CreateBuilder<int, CreatureTypeData>();
      var testCreature = new CreatureTypeData(
        id: 1,
        name: "Test",
        prefabAddress: "/My/Address",
        owner: PlayerName.User,
        health: new IntRangeValue(100, 200),
        skillAnimations: new Dictionary<SkillAnimationNumber, SkillAnimationType>
          {{SkillAnimationNumber.Skill1, SkillAnimationType.MeleeSkill}}.ToImmutableDictionary(),
        baseManaCost: 50,
        implicitAffix: new AffixTypeData(
          id: 2,
          minLevel: 1,
          weight: 1,
          manaCost: IntRangeValue.Zero,
          affixPoolId: 17,
          modifiers: ImmutableList.Create(
            new ModifierTypeData(
              statId: StatId.Health,
              statOperator: Operator.Add,
              valueLow: new IntValueData(25),
              valueHigh: new IntValueData(75)))));
      creatures[testCreature.Id] = testCreature;

      return new GameData(
        creatureTypes: creatures.ToImmutable(),
        affixTypes: ImmutableDictionary<int, AffixTypeData>.Empty,
        skillTypes: ImmutableDictionary<int, SkillTypeData>.Empty);
    }
  }
}
