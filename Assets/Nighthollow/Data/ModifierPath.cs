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

namespace Nighthollow.Data
{
  public readonly struct ModifierPath
  {
    public readonly int CardId;
    public readonly int AffixId;
    public readonly int ModifierId;
    public readonly int? SkillId;

    public ModifierPath(int cardId, int affixId, int modifierId, int? skillId = null)
    {
      CardId = cardId;
      AffixId = affixId;
      ModifierId = modifierId;
      SkillId = skillId;
    }

    public bool Equals(ModifierPath other)
    {
      return CardId == other.CardId &&
             AffixId == other.AffixId &&
             ModifierId == other.ModifierId &&
             SkillId == other.SkillId;
    }

    public override bool Equals(object? obj)
    {
      return obj is ModifierPath other && Equals(other);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = CardId;
        hashCode = (hashCode * 397) ^ AffixId;
        hashCode = (hashCode * 397) ^ ModifierId;
        hashCode = (hashCode * 397) ^ SkillId.GetHashCode();
        return hashCode;
      }
    }
  }
}