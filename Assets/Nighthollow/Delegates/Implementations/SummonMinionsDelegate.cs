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
using Nighthollow.Data;
using Nighthollow.Delegates.Effects;
using Nighthollow.Delegates.Handlers;
using Nighthollow.Services;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Delegates.Implementations
{
  public sealed class SummonMinionsDelegate : IDelegate, IOnSkillUsed
  {
    public string Describe(IStatDescriptionProvider provider) => "Summons Minions";

    public IEnumerable<Effect> OnSkillUsed(IGameContext c, IOnSkillUsed.Data d)
    {
      var filePosition = Errors.CheckNotNull(c[d.Self].FilePosition);
      var rank = GetOpenForwardRank(c.Creatures, Errors.CheckNotNull(c[d.Self].RankPosition), filePosition);
      if (rank.HasValue)
      {
        var summon = d.Skill.ItemData.Summons[Random.Range(0, d.Skill.ItemData.Summons.Count)];
        yield return new CreateCreatureEffect(
          c[d.Self].Owner,
          summon,
          rank.Value,
          filePosition,
          isMoving: true);
      }
    }

    /// <summary>
    ///   Returns the first open rank position in front of this (rank, file) if one exists
    /// </summary>
    public static RankValue? GetOpenForwardRank(CreatureService service, RankValue rank, FileValue file)
    {
      while (true)
      {
        var result = rank.Increment();
        if (result == null)
        {
          return null;
        }

        if (!service.PlacedCreatures.ContainsKey((result.Value, file)))
        {
          return result.Value;
        }

        rank = result.Value;
      }
    }
  }
}