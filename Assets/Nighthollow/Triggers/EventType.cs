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

namespace Nighthollow.Triggers
{
  public enum EventType
  {
    Unknown = 0,
    BattleStarted = 1,
    DrewOpeningHand = 2,
    EnemyCreatureSpawned = 3,
    HexAttacked = 4,
    WorldSceneReady = 5,
    BattleSceneReady = 6,
    SchoolSelectionSceneReady = 7,
    TriggerInvoked = 8,
    UserCreaturePlayed = 9
  }
}