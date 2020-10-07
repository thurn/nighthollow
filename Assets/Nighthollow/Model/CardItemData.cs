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

namespace Nighthollow.Model
{
  /// <summary>Represents a card in the inventory.</summary>
  public sealed class CardItemData : AbstractGameEntity
  {
    public CardData Card { get; }
    public IReadOnlyList<AffixData> Affixes { get; }
    public override StatTable Stats => Card.Stats;

    public CardItemData(CardData card, IReadOnlyList<AffixData> affixes)
    {
      Card = card;
      Affixes = affixes;
    }

    public CardItemData Clone() => new CardItemData(Card.Clone(), Affixes.Select(a => a.Clone()).ToList());
  }
}