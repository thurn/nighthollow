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

using Nighthollow.Services;
using UnityEngine;

#nullable enable

namespace Nighthollow.World
{
  public sealed class WorldTutorial
  {
    public static readonly Vector2Int StartingHex = new Vector2Int(x: -12, y: 7);
    public static readonly Vector2Int TutorialAttackHex = new Vector2Int(x: -11, y: 7);

    readonly WorldServiceRegistry _registry;

    public WorldTutorial(WorldServiceRegistry registry)
    {
      _registry = registry;
    }

    public void OnWorldSceneLoaded()
    {
      // _registry.ScreenController.ShowDialog("ocerak", IntroText);
      // _registry.WorldMap.ShowIcon(TutorialAttackHex, _registry.StaticAssets.FightIcon);
    }
  }
}