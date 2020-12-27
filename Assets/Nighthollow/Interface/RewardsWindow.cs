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

using System.Collections.Generic;
using Nighthollow.Items;
using Nighthollow.Model;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public sealed class RewardsWindow : HideableElement<RewardsWindow.Args>
  {

    VisualElement _content = null!;

    public readonly struct Args
    {
      public readonly IReadOnlyList<CreatureItemData> Items;

      public Args(IReadOnlyList<CreatureItemData> items)
      {
        Items = items;
      }
    }

    public new sealed class UxmlFactory : UxmlFactory<RewardsWindow, UxmlTraits>
    {
    }

    protected override void Initialize()
    {
      _content = FindElement("Content");
    }

    protected override void OnShow(Args argument)
    {
      ItemRenderer.AddItems(Controller, _content, argument.Items,
        new ItemRenderer.Config(count: 20, shouldAddTooltip: false));
    }
  }
}
