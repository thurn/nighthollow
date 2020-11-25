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

using System;
using System.Text;
using Nighthollow.Generated;
using UnityEngine.UIElements;

namespace Nighthollow.Interface
{
  public sealed class TooltipBuilder
  {
    public string Name { get; }
    public Rarity Rarity { get; set; } = Rarity.Common;
    public int XOffset { get; set; } = 16;
    public bool CloseButton { get; set; }
    public Action? OnClose { get; set; }
    readonly VisualElement _result = new VisualElement();

    StringBuilder? _currentText;
    bool _appendDivider;

    public TooltipBuilder(string name)
    {
      Name = name;

      _result.AddToClassList("tooltip-content");
    }

    public TooltipBuilder StartGroup()
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

    public TooltipBuilder AppendDivider()
    {
      StartGroup();
      _appendDivider = true;
      return this;
    }

    public TooltipBuilder AppendNullable(string? text)
    {
      if (text != null)
      {
        AppendText(text);
      }
      return this;
    }

    public TooltipBuilder AppendText(string text)
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

    public TooltipBuilder AppendButton(string text, Action onClick)
    {
      StartGroup();
      var button = new VisualElement();
      button.AddToClassList("button");
      button.RegisterCallback<ClickEvent>(e => onClick());
      var label = new Label {text = text};
      button.Add(label);
      _result.Add(button);
      return this;
    }

    public VisualElement BuildContent()
    {
      StartGroup();
      return _result;
    }
  }
}
