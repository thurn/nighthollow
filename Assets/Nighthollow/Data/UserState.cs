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

using System.Collections.Immutable;
using System.Linq;
using Nighthollow.Delegates;
using Nighthollow.Services;
using Nighthollow.State;
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Data
{
  public sealed partial class UserState : StatEntity, IPlayerState
  {
    public UserState(StatTable stats, DelegateList delegateList, KeyValueStore keyValueStore, int mana)
    {
      Stats = stats;
      DelegateList = delegateList;
      KeyValueStore = keyValueStore;
      Mana = mana;
    }

    [Field] public override StatTable Stats { get; }
    [Field] public DelegateList DelegateList { get; }
    [Field] public KeyValueStore KeyValueStore { get; }
    [Field] public int Mana { get; }

    public UserState OnTick(IGameContext c) => WithStats(Stats.OnTick(c));

    public static UserState BuildUserData(GameData gameData)
    {
      var stats = gameData.UserModifiers.Values.Aggregate(
        new StatTable(StatData.BuildDefaultStatTable(gameData)),
        (current, modifier) => current.InsertNullableModifier(modifier.BuildStatModifier()));
      // TODO: Persist user delegate list
      var delegateList = new DelegateList(ImmutableList<IDelegate>.Empty, DelegateList.Root);
      return new UserState(stats, delegateList, new KeyValueStore(), stats.Get(Stat.StartingMana));
    }
  }
}