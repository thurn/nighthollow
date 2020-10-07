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

using System.Text;
using Nighthollow.Generated;
using Nighthollow.Model;

namespace Nighthollow.Data
{
  public static class DescriptiveTextHelper
  {
    public static string TextForCardItem(CardItemData item)
    {
      var card = CardBuilder.Build(item);
      var creature = card.Creature;
      var result = new StringBuilder($"<u><b>{creature.Name}</b></u>");

      AppendLine(result, "");
      AppendLine(result, $"<b>Health:</b> {creature.GetInt(Stat.Health)}");

      return result.ToString();
    }

    static void AppendLine(StringBuilder result, string text)
    {
      result.Append("\n");
      result.Append(text);
    }
  }
}