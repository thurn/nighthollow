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
using System.Linq;
using Nighthollow.Data;
using Nighthollow.Delegates.Effects;
using Nighthollow.Delegates.Handlers;
using Nighthollow.Services;
using Nighthollow.Stats;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Delegates.Implementations
{
  public sealed class ApplyStatusEffectsToAdjacentAlliesDelegate : IDelegate, IOnSkillUsed
  {
    public string Describe(IStatDescriptionProvider provider) => "Buffs Adjacent Allies With:";

    public IEnumerable<Effect> OnSkillUsed(IGameContext c, int delegateIndex, IOnSkillUsed.Data d)
    {
      var adjacent = GetAdjacentUserCreatures(c.Creatures,
        Errors.CheckNotNull(d.Self.RankPosition),
        Errors.CheckNotNull(d.Self.FilePosition));
      return (
        from target in adjacent
        from statusEffect in d.Skill.ItemData.StatusEffects
        select new ApplyStatusEffectEffect(target, statusEffect));
    }

    /// <summary>
    ///   Returns all User creatures in the 9 squares around the given (rank, file) position (including the creature at
    ///   that position, if any).
    /// </summary>
    public static IEnumerable<CreatureId> GetAdjacentUserCreatures(
      ICreatureService service, RankValue inputRank, FileValue inputFile) =>
      from rank in BoardPositions.AdjacentRanks(inputRank)
      from file in BoardPositions.AdjacentFiles(inputFile)
      where service.PlacedCreatures.ContainsKey((rank, file))
      select service.PlacedCreatures[(rank, file)];
  }
}