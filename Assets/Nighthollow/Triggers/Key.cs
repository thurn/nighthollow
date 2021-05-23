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

#nullable enable

namespace Nighthollow.Triggers
{
  public interface IKey
  {
  }

  public sealed class Key<T> : IKey
  {
  }

  public sealed class MutatorKey<T> : IKey
  {
  }

  public static class Key
  {
    public static readonly MutatorKey<Database> Database = new MutatorKey<Database>();
    public static readonly Key<GameData> GameData = new Key<GameData>();
    public static readonly MutatorKey<AssetService> AssetService = new MutatorKey<AssetService>();
    public static readonly MutatorKey<WorldMapRenderer> WorldMapRenderer = new MutatorKey<WorldMapRenderer>();
    public static readonly MutatorKey<WorldMapController> WorldMapController = new MutatorKey<WorldMapController>();
    public static readonly MutatorKey<Camera> MainCamera = new MutatorKey<Camera>();
    public static readonly MutatorKey<ScreenController> ScreenController = new MutatorKey<ScreenController>();
    public static readonly MutatorKey<TriggerService> TriggerService = new MutatorKey<TriggerService>();

    public static readonly MutatorKey<CreatureService.Controller> CreatureController =
      new MutatorKey<CreatureService.Controller>();
  }
}