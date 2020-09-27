using System.Linq;
using System.Text;
using Nighthollow.Generated;
using Nighthollow.Model;
using Nighthollow.Utils;

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