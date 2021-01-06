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

using System;
using System.Collections.Immutable;

#nullable enable

namespace Nighthollow.Data
{
  public sealed class FieldAttribute : Attribute
  {
  }

  public sealed partial class GameData
  {
    public GameData(
      ImmutableDictionary<int, TableMetadata>? tableMetadata = null,
      ImmutableDictionary<int, CreatureTypeData>? creatureTypes = null,
      ImmutableDictionary<int, AffixTypeData>? affixTypes = null,
      ImmutableDictionary<int, SkillTypeData>? skillTypes = null,
      ImmutableDictionary<int, StatData>? statData = null,
      ImmutableDictionary<int, StaticCreatureListData>? creatureLists = null,
      ImmutableDictionary<int, ModifierData>? userModifiers = null,
      ImmutableDictionary<int, CreatureItemData>? collection = null,
      ImmutableDictionary<int, CreatureItemData>? deck = null)
    {
      TableMetadata = tableMetadata ?? ImmutableDictionary<int, TableMetadata>.Empty;
      CreatureTypes = creatureTypes ?? ImmutableDictionary<int, CreatureTypeData>.Empty;
      AffixTypes = affixTypes ?? ImmutableDictionary<int, AffixTypeData>.Empty;
      SkillTypes = skillTypes ?? ImmutableDictionary<int, SkillTypeData>.Empty;
      StatData = statData ?? ImmutableDictionary<int, StatData>.Empty;
      CreatureLists = creatureLists ?? ImmutableDictionary<int, StaticCreatureListData>.Empty;
      UserModifiers = userModifiers ?? ImmutableDictionary<int, ModifierData>.Empty;
      Collection = collection ?? ImmutableDictionary<int, CreatureItemData>.Empty;
      Deck = deck ?? ImmutableDictionary<int, CreatureItemData>.Empty;
    }

    [Field] public ImmutableDictionary<int, TableMetadata> TableMetadata { get; }
    [Field] public ImmutableDictionary<int, CreatureTypeData> CreatureTypes { get; }
    [Field] public ImmutableDictionary<int, AffixTypeData> AffixTypes { get; }
    [Field] public ImmutableDictionary<int, SkillTypeData> SkillTypes { get; }
    [Field] public ImmutableDictionary<int, StatData> StatData { get; }
    [Field] public ImmutableDictionary<int, StaticCreatureListData> CreatureLists { get; }
    [Field] public ImmutableDictionary<int, ModifierData> UserModifiers { get; }
    [Field] public ImmutableDictionary<int, CreatureItemData> Collection { get; }
    [Field] public ImmutableDictionary<int, CreatureItemData> Deck { get; }
  }
}
