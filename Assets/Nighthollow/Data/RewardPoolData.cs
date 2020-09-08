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
  [CreateAssetMenu(menuName = "Data/Reward Pool")]
  public sealed class RewardPoolData : ScriptableObject
  {
    [SerializeField] List<CardData> _baseCards;
    public IReadOnlyCollection<CardData> BaseCards => _baseCards.AsReadOnly();

    [SerializeField] List<AffixData> _affixes;
    public IReadOnlyCollection<AffixData> Affixes => _affixes.AsReadOnly();

    public RewardPoolData Clone()
    {
      var result = CreateInstance<RewardPoolData>();

      if (_baseCards != null)
      {
        result._baseCards = _baseCards.Select(c => c.Clone()).ToList();
      }

      if (_affixes != null)
      {
        result._affixes = _affixes.Select(a => a.Clone()).ToList();
      }

      return result;
    }
  }
}