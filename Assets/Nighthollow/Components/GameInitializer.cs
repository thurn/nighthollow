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
using UnityEngine;

#nullable enable

namespace Nighthollow.Components
{
  public sealed class GameInitializer : MonoBehaviour, IOnDatabaseReadyListener
  {
    [SerializeField] ScreenController _screenController = null!;
    [SerializeField] DataService _dataService = null!;

    void Start()
    {
      _screenController.Initialize();
      _dataService.OnReady(Root.Instance.CreatureService);
      _dataService.OnReady(Root.Instance.HelperTextService);
      _dataService.OnReady(Root.Instance.User.Hand);
      _dataService.OnReady(this);
    }

    public void OnDatabaseReady(Database database)
    {
      Root.Instance.User.DrawOpeningHand(database);
    }
  }
}
