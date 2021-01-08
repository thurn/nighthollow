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

namespace Nighthollow.Model
{
  public enum Operator
  {
    Unknown = 0,
    Add = 1,
    Increase = 2,
    SetFalse = 3,
    SetTrue = 4,
    Overwrite = 5
  }

  public enum StaticCardList
  {
    Unknown = 0,
    StartingDeck = 1,
    TutorialDraws = 2,
    TutorialEnemy = 3,
    TutorialReward1 = 4,
    TutorialReward2 = 5,
    TutorialReward3 = 6,
    Summons = 7
  }

  public enum AffixPool
  {
    Unknown = 0,
    CreatureImplicits = 1,
    SkillImplicits = 2
  }
}
