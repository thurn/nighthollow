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
using Nighthollow.Interface;
using Nighthollow.Model;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Items
{
  public static class ItemRenderer
  {
    public readonly struct Config
    {
      public readonly int? Count;
      public readonly ItemSlot.Size Size;
      public readonly bool ShouldAddTooltip;

      public Config(int? count = null, ItemSlot.Size size = ItemSlot.Size.Large, bool shouldAddTooltip = true)
      {
        Count = count;
        Size = size;
        ShouldAddTooltip = shouldAddTooltip;
      }
    }

    public static void AddItems(
      ScreenController controller,
      VisualElement parentElement,
      IEnumerable<IItemData> items,
      Config config)
    {
      parentElement.Clear();

      foreach (var item in items)
      {
        var slot = new ItemSlot(config.Size);
        slot.SetItem(controller, item, config.ShouldAddTooltip);
        parentElement.Add(slot);
      }

      for (var index = 0; index < (config.Count ?? 0); ++index)
      {
        parentElement.Add(new ItemSlot(config.Size));
      }
    }
  }
}
