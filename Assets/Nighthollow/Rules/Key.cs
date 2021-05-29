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
using Nighthollow.Services;
using Nighthollow.World;
using UnityEngine;

// ReSharper disable UnusedTypeParameter

#nullable enable

namespace Nighthollow.Rules
{
  public interface IKey
  {
    string Name { get; }
  }

  public sealed class ReaderKey<T> : IKey
  {
    public string Name { get; }

    public ReaderKey(string name)
    {
      Name = name;
    }
  }

  public sealed class MutatorKey<T> : IKey
  {
    public string Name { get; }

    public MutatorKey(string name)
    {
      Name = name;
    }
  }

  public static class Key
  {
    public static readonly MutatorKey<Database> Database = new MutatorKey<Database>("Database");

    public static readonly ReaderKey<GameData> GameData = new ReaderKey<GameData>("GameData");

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
  }
}