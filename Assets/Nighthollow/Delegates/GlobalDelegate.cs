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
using System.Collections.Immutable;
using Nighthollow.Delegates.Effects;
using Nighthollow.Delegates.Handlers;
using Nighthollow.Services;
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Delegates
{
  public sealed class GlobalDelegate : AbstractDelegate, IOnEnemyCreatureAtEndzone
  {
    public static readonly DelegateList List = new DelegateList(
      ImmutableList.Create(DelegateMap.Get(DelegateId.GlobalDelegate)), parent: null);

    public override string Describe(IStatDescriptionProvider provider) => "Global Delegate";

    public IEnumerable<Effect> OnEnemyCreatureAtEndzone(GameContext c, IOnEnemyCreatureAtEndzone.Data d)
    {
      yield return new GameOverEffect();
    }
  }
}