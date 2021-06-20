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
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface.Components.Windows
{
  public sealed record RootComponent : AbstractRootComponent<RootComponent>
  {
    public BaseComponent? CurrentlyOpenWindow { get; init; }
    public BaseComponent? CurrentTooltip { get; init; }

    protected override BaseComponent OnRender(Scope scope)
    {
      return new Column
      {
        FlexPosition = Position.Absolute,
        InsetAll = 0,
        AlignItems = Align.Center,
        JustifyContent = Justify.Center,
        Children = List(
          CurrentlyOpenWindow,
          CurrentTooltip
        )
      };
    }

    protected override RootComponent This => this;

    public override RootComponent OnDragBegin() => this with {CurrentTooltip = null};
  }
}