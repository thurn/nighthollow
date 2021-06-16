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
using Nighthollow.Interface.Components.Core;
using Nighthollow.Rules;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface.Components.Library
{
  public enum TextType
  {
    Headline,
    Body,
    Meta,
    Button
  }

  public sealed record Text : LayoutComponent
  {
    public Text(string value)
    {
      Value = value;
    }

    public string Value { get; }
    public TextType Type { get; init; } = TextType.Body;
    public HierarchyLevel HierarchyLevel { get; init; } = HierarchyLevel.Three;
    public Color? TextColor { get; init; }
    public bool Outline { get; init; }
    public TextAnchor TextAlign { get; init; }

    protected override BaseComponent OnRender(Scope scope)
    {
      var sizeMultiplier = HierarchyLevel switch
      {
        HierarchyLevel.One => 1.5f,
        HierarchyLevel.Two => 1.25f,
        HierarchyLevel.Three => 1f,
        HierarchyLevel.Four => 0.75f,
        _ => throw new ArgumentOutOfRangeException()
      };

      return new LabelComponent
      {
        Text = Value,
        Font = (Type, HierarchyLevel) switch
        {
          (TextType.Headline, HierarchyLevel.One) => "Fonts/BluuNextBold",
          (TextType.Headline, HierarchyLevel.Two) => "Fonts/BluuNextBold",
          _ => "Fonts/RobotoRegular"
        },
        FontColor = TextColor ?? ColorPalette.PrimaryText,
        WhiteSpace = WhiteSpace.Normal,
        FontSize = Mathf.RoundToInt(sizeMultiplier * Type switch
        {
          TextType.Headline => 36,
          TextType.Body => 30,
          TextType.Meta => 20,
          TextType.Button => 26,
          _ => throw new ArgumentOutOfRangeException()
        }),
        OutlineColor = Outline ? Color.black : null,
        OutlineWidth = Outline ? 1 : 0,
        TextAlign = TextAlign
      };
    }
  }
}