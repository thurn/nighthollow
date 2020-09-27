using System.Collections.Generic;
using Nighthollow.Model;

namespace Nighthollow.Data
{
  public static class CardBuilder
  {
    public static CardData Build(CardItemData item) => item.Clone().Card;

    public static CardItemData RollStats(CardTemplateData template, List<AffixTemplateData> affixes)
    {
      return template.Low.Clone();
    }
  }
}