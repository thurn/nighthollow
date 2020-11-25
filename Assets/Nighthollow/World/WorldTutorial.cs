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

#nullable enable

using Nighthollow.Interface;
using Nighthollow.Services;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Nighthollow.World
{
  public sealed class WorldTutorial : MonoBehaviour
  {
#pragma warning disable 0649
    [SerializeField] WorldMap _worldMap = null!;
    [SerializeField] ScreenController _screenController = null!;
    [SerializeField] Tile _fightIcon = null!;
#pragma warning restore 0649

    public static readonly Vector2Int StartingHex = new Vector2Int(-12, 7);
    public static readonly Vector2Int TutorialAttackHex = new Vector2Int(-11, 7);

    const string IntroText =
      "The sleeper awakes... we have been preparing for your return for many years, my lord. We will once again " +
      "bring the Eternal Night to the world of the living!";

    public void Initialize()
    {
      Database.OnReady(data =>
      {
        if (data.UserData.TutorialState == UserDataService.Tutorial.Starting)
        {
          _screenController.ShowDialog("ocerak", IntroText);
          _worldMap.ShowIcon(TutorialAttackHex, _fightIcon);
          data.UserData.TutorialState = UserDataService.Tutorial.InitialWorldScreen;
        }
      });
    }
  }
}
