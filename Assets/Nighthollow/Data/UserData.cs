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

using System.Collections.Generic;
using System.Linq;
using Nighthollow.Stats;

namespace Nighthollow.Data
{
  public sealed class UserData : StatEntity
  {
    public IReadOnlyList<CreatureData> Deck { get; }
    public override StatTable Stats { get; }
    public bool OrderedDraws { get; }

    public UserData(
      IReadOnlyList<CreatureData> deck,
      StatTable stats,
      bool orderedDraws)
    {
      Deck = deck;
      Stats = stats;
      OrderedDraws = orderedDraws;
    }

    public UserData Clone(StatTable parentStats)
    {
      var statTable = Stats.Clone(parentStats);
      return new UserData(Deck.Select(d => d.Clone(statTable)).ToList(), statTable, OrderedDraws);
    }
  }
}