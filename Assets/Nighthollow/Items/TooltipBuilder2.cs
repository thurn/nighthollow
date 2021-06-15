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
using System.Text;
using Nighthollow.Data;
using Nighthollow.Interface;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Items
{
  public sealed class TooltipBuilder2
  {
    readonly VisualElement _result = new VisualElement();
    bool _appendDivider;

    StringBuilder? _currentText;

    public TooltipBuilder2(string name, Vector2 anchorPoint)
    {
      AnchorPoint = anchorPoint;
      Name = name;
      _result.AddToClassList("tooltip-content");
    }

    public Vector2 AnchorPoint { get; }
    public string Name { get; }
    public Rarity Rarity { get; set; } = Rarity.Common;
    public int XOffset { get; set; } = 16;
    public bool CloseButton { get; set; }
    public Action? OnHide { get; set; }

    public TooltipBuilder2 StartGroup()
    {
      if (_currentText != null)
      {
        var label = new Label {text = _currentText.ToString()};
        label.AddToClassList("text");
        _result.Add(label);
      }

      _currentText = null;
      return this;
    }

    public TooltipBuilder2 AppendDivider()
    {
      StartGroup();
      _appendDivider = true;
      return this;
    }

    public TooltipBuilder2 AppendNullable(string? text)
    {
      if (text != null)
      {
        AppendText(text);
      }

      return this;
    }

    public TooltipBuilder2 AppendText(string text)
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

      return this;
    }

    public TooltipBuilder2 AppendButton(string text, Action onClick)
    {
      StartGroup();
      _result.Add(ButtonUtil.Create(new ButtonUtil.Button(text, onClick)));
      return this;
    }

    public VisualElement BuildContent()
    {
      StartGroup();
      return _result;
    }
  }
}
