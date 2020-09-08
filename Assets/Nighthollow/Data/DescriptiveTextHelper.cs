using System.Text;
using Nighthollow.Components;
using Nighthollow.Utils;

namespace Nighthollow.Data
{
  public static class DescriptiveTextHelper
  {
    public static string TextForCardItem(CardItemData item)
    {
      var card = item.Build();
      var creature = card.Creature;
      var result = new StringBuilder($"<u><b>{creature.Name}</b></u>");

      AppendParagraph(result, $"<b>Health:</b> {creature.Health.Value}");
      foreach (var type in Damage.AllTypes)
      {
        var damage = creature.BaseAttack.Get(type).Value;
        if (damage > 0)
        {
          AppendLine(result, $"<b>{type} Damage:</b> {DamageRange(creature, damage)}");
        }
      }

      AppendLine(result,
        $"<b>Critical Hit:</b> {Constants.MultiplierStringBasisPoints(creature.CritMultiplier.Value)}" +
        $" @ {Constants.PercentageStringBasisPoints(creature.CritChance.Value)}");

      AppendLine(result, $"<b>Accuracy/Evasion:</b> {creature.Accuracy.Value}/{creature.Evasion.Value}");

      return result.ToString();
    }

    static void AppendLine(StringBuilder result, string text)
    {
      result.Append("\n");
      result.Append(text);
    }

    static void AppendParagraph(StringBuilder result, string text)
    {
      result.Append("\n\n");
      result.Append(text);
    }

    static string DamageRange(CreatureData creature, int damage)
    {
      var range = Constants.FractionBasisPoints(damage, creature.DamageRange.Value);
      return $"{damage - range}-{damage + range}";
    }
  }
}