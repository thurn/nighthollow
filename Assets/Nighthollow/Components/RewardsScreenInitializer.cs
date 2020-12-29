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
using UnityEngine;

#nullable enable

namespace Nighthollow.Components
{
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
    }
  }
}
