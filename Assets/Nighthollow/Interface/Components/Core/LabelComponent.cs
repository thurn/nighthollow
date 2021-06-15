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

using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface.Components.Core
{
  public sealed record LabelComponent : MountComponent<Label>
  {
    public string? Text { get; init; }
    public string? Font { get; init; }
    public int? FontSize { get; init; }
    public Color? FontColor { get; init; }
    public WhiteSpace? WhiteSpace { get; init; }
    public Color? OutlineColor { get; init; }
    public int? OutlineWidth { get; init; }

    public override string Type => "Text";

    protected override Label OnCreateMountContent() => new();

    protected override void OnMount(Label element)
    {
      var font = UseResource<Font>(Font);
      element.text = Text;
      element.style.unityFontDefinition =
        font ? new StyleFontDefinition(font) : new StyleFontDefinition(StyleKeyword.Null);
      element.style.fontSize = FontSize is { } fs ? fs : new StyleLength(StyleKeyword.Null);
      element.style.color = FontColor is { } c ? c : new StyleColor(StyleKeyword.Null);
      element.style.unityTextOutlineColor = OutlineColor ?? new StyleColor(StyleKeyword.Null);
      element.style.unityTextOutlineWidth = OutlineWidth ?? new StyleFloat(StyleKeyword.Null);
      element.style.whiteSpace = WhiteSpace is { } ws ? ws : new StyleEnum<WhiteSpace>(StyleKeyword.Undefined);
    }
  }
}