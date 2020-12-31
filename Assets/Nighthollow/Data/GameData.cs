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
using MessagePack;

#nullable enable

namespace Nighthollow.Data
{
  [MessagePackObject]
  public sealed class GameData
  {
    public GameData(
      ImmutableDictionary<int, CreatureTypeData>? creatureTypes = null,
      ImmutableDictionary<int, AffixTypeData>? affixTypes = null,
      ImmutableDictionary<int, SkillTypeData>? skillTypes = null)
    {
      CreatureTypes = creatureTypes ?? ImmutableDictionary<int, CreatureTypeData>.Empty;
      AffixTypes = affixTypes ?? ImmutableDictionary<int, AffixTypeData>.Empty;
      SkillTypes = skillTypes ?? ImmutableDictionary<int, SkillTypeData>.Empty;
    }

    [Key(0)] public ImmutableDictionary<int, CreatureTypeData> CreatureTypes { get; }
    [Key(1)] public ImmutableDictionary<int, AffixTypeData> AffixTypes { get; }
    [Key(2)] public ImmutableDictionary<int, SkillTypeData> SkillTypes { get; }

    public GameData WithCreatureTypes(ImmutableDictionary<int, CreatureTypeData> creatureTypes) =>
      new GameData(creatureTypes, AffixTypes, SkillTypes);

    public GameData WithAffixTypes(ImmutableDictionary<int, AffixTypeData> affixTypes) =>
      new GameData(CreatureTypes, affixTypes, SkillTypes);

    public GameData WithSkillTypes(ImmutableDictionary<int, SkillTypeData> skillTypes) =>
      new GameData(CreatureTypes, AffixTypes, skillTypes);
  }
}
