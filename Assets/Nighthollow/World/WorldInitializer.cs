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

using Nighthollow.Interface;
using UnityEngine;

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
    }
  }
}
