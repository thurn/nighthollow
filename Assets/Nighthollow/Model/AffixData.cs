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
using Nighthollow.Services;
using Nighthollow.Utils;
using SimpleJSON;

#nullable enable

namespace Nighthollow.Model
{
  public sealed class AffixData
  {
    public AffixData(AffixTypeData baseType, IReadOnlyList<ModifierData> modifiers)
    {
      BaseType = baseType;
      Modifiers = modifiers;
    }

    public AffixTypeData BaseType { get; }
    public IReadOnlyList<ModifierData> Modifiers { get; }

    public static AffixData Deserialize(GameDataService gameData, JSONNode node)
    {
      return new AffixData(
        gameData.GetAffixType(node["baseType"].AsInt),
        node["modifiers"].FromJsonArray().Select(c => ModifierData.Deserialize(gameData, c)).ToList());
    }

    public JSONNode Serialize()
    {
      return new JSONObject
      {
        ["baseType"] = BaseType.Id,
        ["modifiers"] = Modifiers.Select(m => m.Serialize()).AsJsonArray()
      };
    }

    public override string ToString()
    {
      return string.Join(", ", Modifiers.Select(m => m.ToString()));
    }
  }
}
