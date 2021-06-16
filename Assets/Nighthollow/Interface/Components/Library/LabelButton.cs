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

using Nighthollow.Interface.Components.Core;
using Nighthollow.Rules;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface.Components.Library
{
  public sealed record LabelButton : LayoutComponent
  {
    public LabelButton(string text)
    {
      Text = text;
    }

    public string Text { get; }
    public HierarchyLevel HierarchyLevel { get; init; } = HierarchyLevel.Three;

    enum State
    {
      Default,
      Hover,
      Active
    }

    protected override BaseComponent OnRender(Scope scope)
    {
      var state = UseState(State.Default);
      return new Row
      {
        Height = 72,
        PaddingLeft = 64,
        PaddingRight = 64,
        BackgroundImage = state.Value switch
        {
          State.Default => "GUI/LongButtonOff",
          _ => "GUI/LongButtonOn",
        },
        BackgroundImageTintColor = state.Value switch
        {
          State.Active => new Color(0.8f, 0.8f, 0.8f),
          _ => null
        },
        BackgroundSliceLeftRight = 20,
        BackgroundSliceTopBottom = 10,
        AlignItems = Align.Center,
        JustifyContent = Justify.SpaceAround,
        OnMouseOver = _ => { state.Set(State.Hover); },
        OnMouseOut = _ => { state.Set(State.Default); },
        OnMouseDown = _ => { state.Set(State.Active); },
        OnMouseUp = _ => { state.Set(State.Hover); },
        Children = List(
          new Text(Text)
          {
            Type = TextType.Button,
            HierarchyLevel = HierarchyLevel
          }
        )
      };
    }
  }
}