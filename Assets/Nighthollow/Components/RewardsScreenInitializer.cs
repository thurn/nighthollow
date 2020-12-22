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
using Nighthollow.Data;
using Nighthollow.Generated;
using Nighthollow.Interface;
using Nighthollow.Services;
using UnityEngine;

#nullable enable

namespace Nighthollow.Components
{
  public sealed class RewardsScreenInitializer : MonoBehaviour
  {
    [SerializeField] ScreenController _screenController = null!;

    void Start()
    {
      Database.OnReady(data =>
      {
        _screenController.Initialize();
        // _screenController.Get(ScreenController.RewardsWindow)
        //   .Show(new RewardsWindow.Args(new List<CreatureItemData>()));
        _screenController.Get(ScreenController.RewardChoiceWindow)
          .Show(new RewardChoiceWindow.Args(data.GameData.GetStaticCardList(StaticCardList.StartingDeck)));
      });
    }
  }
}
