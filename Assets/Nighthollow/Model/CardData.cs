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

using Nighthollow.Stats;

namespace Nighthollow.Model
{
  public sealed class CardData
  {
    public string CardPrefabAddress { get; }
    public string ImageAddress { get; }
    public CreatureData Creature { get; }
    public StatTable Stats { get; }

    public CardData(
      string cardPrefabAddress,
      string imageAddress,
      CreatureData creature,
      StatTable stats)
    {
      CardPrefabAddress = cardPrefabAddress;
      ImageAddress = imageAddress;
      Creature = creature;
      Stats = stats;
    }
  }
}