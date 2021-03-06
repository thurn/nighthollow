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

using System.Collections.Immutable;
using System.Linq;
using Nighthollow.Data;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Items
{
  public static class TooltipUtil2
  {
    public static TooltipBuilder2 CreateTooltip(GameData gameData, Vector2 anchorPosition, IItemData item) =>
      item.Switch(
        creature => CreateUserCreatureTooltip(gameData, anchorPosition, creature),
        resource => CreateResourceTooltip(gameData, anchorPosition, resource));

    public static TooltipBuilder2 CreateResourceTooltip(
      GameData gameData,
      Vector2 anchorPosition,
      ResourceItemData data)
    {
      var builder = new TooltipBuilder2(data.Name, anchorPosition);
      builder.AppendText(gameData.ResourceTypes[data.ResourceTypeId].Description ?? "");
      return builder;
    }

    public static TooltipBuilder2 CreateUserCreatureTooltip(
      GameData gameData,
      Vector2 anchorPosition,
      CreatureItemData data)
    {
      var userState = UserState.BuildUserState(gameData);
      var ownerStats = userState.Stats;
      var builder = new TooltipBuilder2(data.Name, anchorPosition);
      var built = data.BuildCreature(gameData, userState);
      builder.AppendText($"Health: {built.GetInt(Stat.Health)}");

      var baseDamage = built.Stats.Get(Stat.BaseDamage);
      foreach (var damageType in CollectionUtils.AllNonDefaultEnumValues<DamageType>(typeof(DamageType)))
      {
        var range = baseDamage.GetOrReturnDefault(damageType, IntRangeValue.Zero);
        if (range != IntRangeValue.Zero)
        {
          builder.AppendText($"Base Attack: {range} {damageType} Damage");
        }
      }

      if (built.GetInt(Stat.Accuracy) != ownerStats.Get(Stat.Accuracy))
      {
        builder.AppendText($"Accuracy: {built.GetInt(Stat.Accuracy)}");
      }

      if (built.GetInt(Stat.Evasion) != ownerStats.Get(Stat.Evasion))
      {
        builder.AppendText($"Evasion: {built.GetInt(Stat.Evasion)}");
      }

      if (built.Get(Stat.CritChance) != ownerStats.Get(Stat.CritChance))
      {
        builder.AppendText($"Critical Hit Chance: {built.Get(Stat.CritChance)}");
      }

      if (built.Get(Stat.CritMultiplier) != ownerStats.Get(Stat.CritMultiplier))
      {
        builder.AppendText($"Critical Hit Multiplier: {built.Get(Stat.CritMultiplier)}");
      }

      builder.AppendDivider();
      RenderModifierGroup(gameData, builder, data.ImplicitModifiers);

      foreach (var skill in data.Skills)
      {
        RenderModifierGroup(
          gameData,
          builder,
          skill.ImplicitModifiers.Concat(skill.Affixes.SelectMany(a => a.Modifiers)).ToImmutableList(),
          $"Skill: {skill.Name}");
      }

      builder.AppendDivider();

      foreach (var affix in data.Affixes)
      {
        RenderModifierGroup(gameData, builder, affix.Modifiers);
      }

      return builder;
    }

    static void RenderModifierGroup(
      GameData gameData,
      TooltipBuilder2 builder,
      ImmutableList<ModifierData> modifiers,
      string? initialLine = null)
    {
      builder.StartGroup();
      if (initialLine != null)
      {
        builder.AppendText(initialLine);
      }

      foreach (var modifier in ModifierDescriptionUtil.RenderModifiers(gameData, modifiers))
      {
        builder.AppendText(modifier);
      }
    }
  }
}