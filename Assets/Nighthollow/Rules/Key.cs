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

using Nighthollow.Data;
using Nighthollow.Interface;
using Nighthollow.Interface.Components.Core;
using Nighthollow.Interface.Components.Windows;
using Nighthollow.Rules.Events;
using Nighthollow.Services;
using Nighthollow.World;
using UnityEngine;

#nullable enable

namespace Nighthollow.Rules
{
  public static class Key
  {
    public static readonly MutatorKey<Database> Database = new MutatorKey<Database>("Database");

    public static readonly DerivedKey<Database, GameData> GameData =
      new DerivedKey<Database, GameData>("GameData", Database, db => db.Snapshot());

    public static readonly MutatorKey<AssetService> AssetService = new MutatorKey<AssetService>("AssetService");

    public static readonly MutatorKey<WorldMapRenderer> WorldMapRenderer =
      new MutatorKey<WorldMapRenderer>("WorldMapRenderer");

    public static readonly MutatorKey<WorldMapController> WorldMapController =
      new MutatorKey<WorldMapController>("WorldMapController");

    public static readonly MutatorKey<Camera> MainCamera = new MutatorKey<Camera>("MainCamera");

    public static readonly MutatorKey<ScreenController> ScreenController =
      new MutatorKey<ScreenController>("ScreenController");

    public static readonly MutatorKey<RulesEngine> RulesEngine = new MutatorKey<RulesEngine>("RulesEngine");

    public static readonly MutatorKey<CreatureService.Controller> CreatureController =
      new MutatorKey<CreatureService.Controller>("CreatureController");

    public static readonly ReaderKey<CreatureId> Creature = new ReaderKey<CreatureId>("Creature");

    public static readonly ReaderKey<int> Hex = new ReaderKey<int>("Hex");

    public static readonly ReaderKey<HotkeyPressedEvent.KeyName> Hotkey =
      new ReaderKey<HotkeyPressedEvent.KeyName>("Hotkey");

    public static readonly ReaderKey<int> ItemList = new ReaderKey<int>("ItemList");

    public static readonly MutatorKey<BattleServiceRegistry> BattleServiceRegistry =
      new MutatorKey<BattleServiceRegistry>("BattleServiceRegistry");

    public static readonly DerivedKey<BattleServiceRegistry, UserService> User =
      new DerivedKey<BattleServiceRegistry, UserService>("User", BattleServiceRegistry, r => r.UserService);

    public static readonly MutatorKey<PlayerPrefsService> PlayerPrefs =
      new MutatorKey<PlayerPrefsService>("PlayerPrefs");

    public static readonly MutatorKey<RewardsService> RewardsService = new MutatorKey<RewardsService>("RewardsService");

    public static readonly MutatorKey<IComponentController<RootComponent>> ComponentController =
      new MutatorKey<IComponentController<RootComponent>>("ComponentController");
  }
}