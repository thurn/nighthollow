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

using Nighthollow.Delegates.Core;
using Nighthollow.Delegates.Effects;
using Nighthollow.Generated;
using Nighthollow.Services;
using Nighthollow.Utils;

namespace Nighthollow.Delegates.Implementations
{
  public sealed class SummonMinionsDelegate : AbstractDelegate
  {
    public override void OnUse(SkillContext c)
    {
      var rank = Root.Instance.CreatureService.GetOpenForwardRank(
        Errors.CheckNotNull(c.Self.RankPosition), c.Self.FilePosition);

      if (rank.HasValue)
      {
        var summons = Root.Instance.GameDataService.GetStaticCardList(StaticCardList.Summons);
        Errors.CheckState(summons.Count == 1, "Expected only one summon creature");
        c.Results.Add(new CreateCreatureEffect(
          summons[0],
          rank.Value,
          c.Self.FilePosition,
          isMoving: true));
      }
    }
  }
}