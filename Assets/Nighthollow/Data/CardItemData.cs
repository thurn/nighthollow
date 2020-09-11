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
using UnityEngine;

namespace Nighthollow.Data
{
  /// <summary>
  /// Represents the model for card and its associated affixes while it is not actively part of a game. Can be converted
  /// into <see cref="CardData"/>, which is the card representation used while a game is in progress, by calling the
  /// <see cref="Build"/> method.
  /// </summary>
  [CreateAssetMenu(menuName = "Data/CardItem")]
  public sealed class CardItemData : ScriptableObject
  {
    [SerializeField] CardData _baseCard;
    public CardData BaseCard => _baseCard;

    [SerializeField] List<AffixData> _affixes;
    public IReadOnlyCollection<AffixData> Affixes => _affixes.AsReadOnly();

    public CardData Build()
    {
      var result = _baseCard.Clone();
      foreach (var affix in _affixes)
      {
        affix.ApplyTo(result);
      }

      return result;
    }

    public void Initialize(CardData baseCard, List<AffixData> affixes)
    {
      _baseCard = baseCard;
      _affixes = affixes;
    }

    public CardItemData Roll()
    {
      var result = CreateInstance<CardItemData>();
      result._baseCard = _baseCard.Roll();
      result._affixes = _affixes.Select(a => a.Clone()).ToList();
      return result;
    }
  }
}