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
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Nighthollow.Data
{
  [Serializable]
  public sealed class AffixModifier
  {
    [SerializeField] StatName _stat;
    public StatName Stat => _stat;

    [SerializeField] Modifier _modifier;
    public Modifier Modifier => _modifier;

    [SerializeField] int _range;
    public int Range => _range;

    public void Roll()
    {
      _modifier = _modifier.CloneWithValue(Random.Range(_modifier.Value, _modifier.Value + _range + 1));
    }
  }

  [CreateAssetMenu(menuName = "Data/Stat Affix")]
  public sealed class StatAffixData : AffixData
  {
    [SerializeField] List<AffixModifier> _modifiers;
    public IReadOnlyCollection<AffixModifier> Modifiers => _modifiers;

    public override void ApplyTo(CardData result)
    {
      foreach (var affixModifier in _modifiers)
      {
        result.Creature.Get(affixModifier.Stat).AddModifier(affixModifier.Modifier);
      }
    }

    public override void Roll()
    {
      _modifiers.ForEach(m => m.Roll());
    }
  }
}