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
  public sealed partial class StaticItemListData
  {
    public enum IdentifierTag
    {
      Unknown = 0,
      StartingDeck = 1
    }

    public StaticItemListData(
      string name,
      ImmutableList<CreatureItemData>? creatures = null,
      ImmutableList<ResourceItemData>? resources = null,
      IdentifierTag tag = IdentifierTag.Unknown)
    {
      Name = name;
      Creatures = creatures ?? ImmutableList<CreatureItemData>.Empty;
      Resources = resources ?? ImmutableList<ResourceItemData>.Empty;
      Tag = tag;
    }

    [Key(0)] public string Name { get; }
    [Key(1)] public ImmutableList<CreatureItemData> Creatures { get; }
    [Key(2)] public ImmutableList<ResourceItemData> Resources { get; }
    [Key(3)] public IdentifierTag Tag { get; }

    public override string ToString() => Name;
  }
}