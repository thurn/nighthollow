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
using Nighthollow.Editing;
using Nighthollow.Generated;
using Nighthollow.Interface;
using Nighthollow.Stats;
using UnityEngine;

#nullable enable

namespace Nighthollow.Components
{
  [MessagePackObject]
  public sealed class GameDataHolder
  {
    public GameDataHolder(ImmutableDictionary<int, CreatureTypeData> creatures)
    {
      Creatures = creatures.ToImmutableDictionary();
    }

    [Key(0)] public ImmutableDictionary<int, CreatureTypeData> Creatures { get; }
  }

  public sealed class RewardsScreenInitializer : MonoBehaviour, IOnDatabaseReadyListener
  {
    [SerializeField] ScreenController _screenController = null!;
    [SerializeField] DataService _dataService = null!;

    void Start()
    {
      _screenController.Initialize();
      _dataService.OnReady(this);

      // Database.OnReady(data =>
      // {
      // _screenController.Get(ScreenController.RewardsWindow)
      //   .Show(new RewardsWindow.Args(new List<CreatureItemData>()));
      //   var list = data.GameData.GetStaticCardList(StaticCardList.StartingDeck);
      //   var result = list.Take<IItemData>(3).Prepend(Database.Instance.GameData.GetResource(1));
      //   _screenController.Get(ScreenController.RewardChoiceWindow)
      //     .Show(new RewardChoiceWindow.Args(result.ToList()));
      // });
    }

    public void OnDatabaseReady(Database database)
    {
      _screenController.Get(ScreenController.GameDataEditor).Show(new GameDataEditor.Args(database));
      // database.Insert(TableId.CreatureTypes, 0, TestCreatures()[0]);
      // database.Insert(TableId.CreatureTypes, 1, TestCreatures()[1]);
    }

    // List<CreatureTypeData> TestCreatures()
    // {
    //   var testCreature = new CreatureTypeData(
    //     name: "Test",
    //     prefabAddress: "/My/Address",
    //     owner: PlayerName.User,
    //     health: new IntRangeValue(100, 200),
    //     skillAnimations: ImmutableList.Create(
    //       new CreatureSkillAnimation(SkillAnimationNumber.Skill1, SkillAnimationType.MeleeSkill)),
    //     baseManaCost: 50,
    //     implicitAffix: new AffixTypeData(
    //       minLevel: 1,
    //       weight: 1,
    //       manaCost: IntRangeValue.Zero,
    //       modifiers: ImmutableList.Create(
    //         new ModifierTypeData(
    //           statId: StatId.Health,
    //           statOperator: Operator.Add,
    //           valueLow: new IntValueData(25),
    //           valueHigh: new IntValueData(75)))));
    //
    //   var testCreature2 = new CreatureTypeData(
    //     name: "Test Two",
    //     prefabAddress: "/My/Address/Two",
    //     owner: PlayerName.Enemy,
    //     health: new IntRangeValue(50, 200),
    //     skillAnimations: ImmutableList.Create(
    //       new CreatureSkillAnimation(SkillAnimationNumber.Skill3, SkillAnimationType.CastSkill),
    //       new CreatureSkillAnimation(SkillAnimationNumber.Skill5, SkillAnimationType.MeleeSkill)),
    //     baseManaCost: 100,
    //     implicitAffix: new AffixTypeData(
    //       minLevel: 1,
    //       weight: 1,
    //       manaCost: IntRangeValue.Zero,
    //       modifiers: ImmutableList.Create(
    //         new ModifierTypeData(
    //           statId: StatId.Health,
    //           statOperator: Operator.Add,
    //           valueLow: new IntValueData(50),
    //           valueHigh: new IntValueData(75)))));
    //
    //   return new List<CreatureTypeData> {testCreature, testCreature2};
    // }
  }
}
