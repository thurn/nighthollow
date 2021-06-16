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
using Nighthollow.Data;
using Nighthollow.Interface.Components.Core;
using Nighthollow.Rules;
using UnityEngine;

#nullable enable

namespace Nighthollow.Interface.Components.Library
{
  public sealed record ResourceTooltip : LayoutComponent
  {
    public ResourceTooltip(ResourceItemData resource, Rect anchor)
    {
      Resource = resource;
      Anchor = anchor;
    }

    public ResourceItemData Resource { get; }
    public Rect Anchor { get; }

    protected override BaseComponent OnRender(Scope scope)
    {
      var gameData = scope.Get(Key.GameData);
      return new Tooltip
      {
        Title = Resource.Name,
        Anchor = Anchor,
        Content = ImmutableList.Create<Tooltip.ITooltipContent>(
          new Tooltip.TextGroup(gameData.ResourceTypes[Resource.ResourceTypeId].Description ?? ""))
      };
    }
  }
}