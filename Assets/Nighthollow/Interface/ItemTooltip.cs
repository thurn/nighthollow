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

using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.Ast;
using Nighthollow.Data;
using Nighthollow.Generated;
using Nighthollow.Stats;
using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nighthollow.Interface
{
  public sealed class TooltipBuilder
  {
    readonly VisualElement _result;
    StringBuilder? _currentText;
    bool _appendDivider;

    public TooltipBuilder(VisualElement result)
    {
      _result = result;
    }

    public void StartGroup()
    {
      if (_currentText != null)
      {
        var label = new Label {text = _currentText.ToString()};
        label.AddToClassList("text");
        _result.Add(label);
      }

      _currentText = null;
    }

    public void AppendDivider()
    {
      StartGroup();
      _appendDivider = true;
    }

    public void AppendNullable(string? text)
    {
      if (text != null)
      {
        AppendText(text);
      }
    }

    public void AppendText(string text)
    {
      if (_appendDivider)
      {
        var divider = new VisualElement();
        divider.AddToClassList("divider");
        _result.Add(divider);
        _appendDivider = false;
      }

      if (_currentText == null)
      {
        _currentText = new StringBuilder(text);
      }
      else
      {
        _currentText.Append("\n").Append(text);
      }
    }

    public VisualElement Build()
    {
      StartGroup();
      return _result;
    }
  }

  public sealed class ItemTooltip : VisualElement
  {
    Label _title = null!;
    VisualElement _content = null!;
    Rect? _anchor;

    public WorldScreenController Controller { get; set; } = null!;

    public new sealed class UxmlFactory : UxmlFactory<ItemTooltip, UxmlTraits>
    {
    }

    public new sealed class UxmlTraits : VisualElement.UxmlTraits
    {
    }

    public ItemTooltip()
    {
      if (Application.isPlaying)
      {
        RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
      }
    }

    public void Show(CreatureItemData data, Rect anchor)
    {
      style.visibility = new StyleEnum<Visibility>(Visibility.Visible);
      style.left = new StyleLength(anchor.x - worldBound.width - 16);
      style.top = new StyleLength(anchor.y);
      _anchor = anchor;

      Render(data);
    }

    public void Hide()
    {
      style.visibility = new StyleEnum<Visibility>(Visibility.Hidden);
      _anchor = null;
    }

    void OnGeometryChange(GeometryChangedEvent evt)
    {
      _title = InterfaceUtils.FindByName<Label>(this, "Title");
      _content = InterfaceUtils.FindByName<VisualElement>(this, "Content");

      if (_anchor != null)
      {
        style.left = new StyleLength(_anchor.Value.x - worldBound.width - 16);
        style.top = new StyleLength(_anchor.Value.y);
      }
    }

    void Render(CreatureItemData data)
    {
      _content.Clear();
      _title.text = data.Name;

      var user = Controller.DataService.UserDataService.UserStats;
      var built = CreatureUtil.Build(user, data);
      var builder = new TooltipBuilder(_content);
      builder.AppendText($"Health: {built.GetInt(Stat.Health)}");

      var baseDamage = built.Stats.Get(Stat.BaseDamage);
      foreach (var damageType in CollectionUtils.AllNonDefaultEnumValues<DamageType>(typeof(DamageType)))
      {
        var range = baseDamage.Get(damageType, IntRangeValue.Zero);
        if (range != IntRangeValue.Zero)
        {
          builder.AppendText($"Base Attack: {range} {damageType} Damage");
        }
      }

      if (built.GetInt(Stat.Accuracy) != user.Get(Stat.Accuracy))
      {
        builder.AppendText($"Accuracy: {built.GetInt(Stat.Accuracy)}");
      }

      if (built.GetInt(Stat.Evasion) != user.Get(Stat.Evasion))
      {
        builder.AppendText($"Evasion: {built.GetInt(Stat.Evasion)}");
      }

      if (built.GetStat(Stat.CritChance) != user.Get(Stat.CritChance))
      {
        builder.AppendText($"Critical Hit Chance: {built.GetStat(Stat.CritChance)}");
      }

      if (built.GetStat(Stat.CritMultiplier) != user.Get(Stat.CritMultiplier))
      {
        builder.AppendText($"Critical Hit Multiplier: {built.GetStat(Stat.CritMultiplier)}");
      }

      builder.AppendDivider();

      foreach (var affix in data.Affixes.Where(affix => affix.BaseType.Id == data.BaseType.ImplicitAffix?.Id))
      {
        RenderAffix(builder, built, affix);
      }

      foreach (var skill in data.Skills.Where(skill => skill.BaseType.Id != 1))
      {
        builder.AppendText($"Skill: {skill.BaseType.Name}");
      }

      builder.AppendDivider();

      foreach (var affix in data.Affixes.Where(affix => affix.BaseType.Id != data.BaseType.ImplicitAffix?.Id))
      {
        RenderAffix(builder, built, affix);
      }

      builder.Build();
    }

    void RenderAffix(TooltipBuilder builder, StatEntity built, AffixData affix)
    {
      builder.StartGroup();

      foreach (var modifier in affix.Modifiers)
      {
        if (modifier.DelegateId.HasValue)
        {
          builder.AppendNullable(DelegateMap.Get(modifier.DelegateId.Value).Describe(built));
        }

        if (modifier.StatModifier != null)
        {
          builder.AppendNullable(modifier.StatModifier.Describe());
        }
      }
    }
  }
}
