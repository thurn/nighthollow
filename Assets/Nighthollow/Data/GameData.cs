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
using MessagePack;

#nullable enable

namespace Nighthollow.Data
{
  [MessagePackObject]
  public sealed class GameData
  {
    public GameData(
      IReadOnlyDictionary<int, CreatureTypeData>? creatureTypes = null,
      IReadOnlyDictionary<int, AffixTypeData>? affixTypes = null,
      IReadOnlyDictionary<int, SkillTypeData>? skillTypes = null)
    {
      CreatureTypes = creatureTypes?.ToImmutableDictionary() ?? ImmutableDictionary<int, CreatureTypeData>.Empty;
      AffixTypes = affixTypes?.ToImmutableDictionary() ?? ImmutableDictionary<int, AffixTypeData>.Empty;
      SkillTypes = skillTypes?.ToImmutableDictionary() ?? ImmutableDictionary<int, SkillTypeData>.Empty;
    }

    [Key(0)] public IReadOnlyDictionary<int, CreatureTypeData> CreatureTypes { get; }

    [Key(1)] public IReadOnlyDictionary<int, AffixTypeData> AffixTypes { get; }

    [Key(2)] public IReadOnlyDictionary<int, SkillTypeData> SkillTypes { get; }
  }
}
