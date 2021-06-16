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
using Nighthollow.Interface.Components.Core;
using Nighthollow.Items;
using Nighthollow.Rules;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Interface.Components.Library
{
  public sealed record UserCreatureTooltip : LayoutComponent
  {
    public UserCreatureTooltip(CreatureItemData creature, Rect anchor)
    {
      Creature = creature;
      Anchor = anchor;
    }

    public CreatureItemData Creature { get; }
    public Rect Anchor { get; }

    protected override BaseComponent OnRender(Scope scope)
    {
      var gameData = scope.Get(Key.GameData);
      var userState = UserState.BuildUserState(gameData);
      var ownerStats = userState.Stats;
      var built = Creature.BuildCreature(gameData, userState);
      var content = ImmutableList.CreateBuilder<Tooltip.ITooltipContent>();
      var group = ImmutableList.CreateBuilder<string>();

      group.Add($"Health: {built.GetInt(Stat.Health)}");

      var baseDamage = built.Stats.Get(Stat.BaseDamage);
      foreach (var damageType in CollectionUtils.AllNonDefaultEnumValues<DamageType>(typeof(DamageType)))
      {
        var range = baseDamage.GetOrReturnDefault(damageType, IntRangeValue.Zero);
        if (range != IntRangeValue.Zero)
        {
          group.Add($"Base Attack: {range} {damageType} Damage");
        }
      }

      if (built.GetInt(Stat.Accuracy) != ownerStats.Get(Stat.Accuracy))
      {
        group.Add($"Accuracy: {built.GetInt(Stat.Accuracy)}");
      }

      if (built.GetInt(Stat.Evasion) != ownerStats.Get(Stat.Evasion))
      {
        group.Add($"Evasion: {built.GetInt(Stat.Evasion)}");
      }

      if (built.Get(Stat.CritChance) != ownerStats.Get(Stat.CritChance))
      {
        group.Add($"Critical Hit Chance: {built.Get(Stat.CritChance)}");
      }

      if (built.Get(Stat.CritMultiplier) != ownerStats.Get(Stat.CritMultiplier))
      {
        group.Add($"Critical Hit Multiplier: {built.Get(Stat.CritMultiplier)}");
      }

      content.Add(new Tooltip.TextGroup(group.ToImmutable()));
      content.Add(new Tooltip.Divider());

      content.Add(RenderModifierGroup(gameData, Creature.ImplicitModifiers));

      foreach (var skill in Creature.Skills)
      {
        content.Add(RenderModifierGroup(
          gameData,
          skill.ImplicitModifiers.Concat(skill.Affixes.SelectMany(a => a.Modifiers)).ToImmutableList(),
          $"Skill: {skill.Name}"));
      }

      content.Add(new Tooltip.Divider());

      foreach (var affix in Creature.Affixes)
      {
        content.Add(RenderModifierGroup(gameData, affix.Modifiers));
      }

      return new Tooltip
      {
        Title = Creature.Name,
        TitleColor = ColorPalette.CommonItem,
        Anchor = Anchor,
        Content = content.ToImmutable(),
      };
    }

    static Tooltip.ITooltipContent RenderModifierGroup(
      GameData gameData,
      ImmutableList<ModifierData> modifiers,
      string? initialLine = null)
    {
      var result = ImmutableList.CreateBuilder<string>();
      if (initialLine != null)
      {
        result.Add(initialLine);
      }

      foreach (var modifier in ModifierDescriptionUtil.RenderModifiers(gameData, modifiers))
      {
        result.Add(modifier);
      }

      return new Tooltip.TextGroup(result.ToImmutable());
    }
  }
}