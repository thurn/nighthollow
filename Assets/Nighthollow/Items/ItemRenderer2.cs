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
using System.Collections.Immutable;
using System.Linq;
using Nighthollow.Interface;
using Nighthollow.Data;
using Nighthollow.Services;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Items
{
  public static class ItemRenderer2
  {
    public sealed class Config
    {
      /// <summary>
      /// Fixed number of slots to render -- additional empty slots will be added if insufficient items are provided,
      /// and excess items will not be rendered.
      /// </summary>
      public int Count { get; }

      public InterfaceItemSlot.Size Size { get; }
      public bool ShouldAddTooltip { get; }

      /// <summary>
      /// If provided, the item will be draggable using this DragManager
      /// </summary>
      public IDragManager<InterfaceItemImage, InterfaceItemSlot>? DragManager { get; }

      public Config(
        int count,
        InterfaceItemSlot.Size size = InterfaceItemSlot.Size.Large,
        bool shouldAddTooltip = true,
        IDragManager<InterfaceItemImage, InterfaceItemSlot>? dragManager = null)
      {
        Count = count;
        Size = size;
        ShouldAddTooltip = shouldAddTooltip;
        DragManager = dragManager;
      }
    }

    public static ImmutableList<InterfaceItemSlot> AddItems(
      ServiceRegistry registry,
      VisualElement parentElement,
      IEnumerable<IItemData> items,
      Config config)
    {
      var result = ImmutableList.CreateBuilder<InterfaceItemSlot>();
      parentElement.Clear();
      var itemList = items.ToList();
      foreach (var item in itemList.Take(config.Count))
      {
        var slot = new InterfaceItemSlot(config.Size);
        slot.SetItem(registry, item, config);
        parentElement.Add(slot);
        result.Add(slot);
      }

      // Add extra empty slots to get to Count
      for (var index = 0; index < config.Count - itemList.Count; ++index)
      {
        var slot = new InterfaceItemSlot(config.Size);
        parentElement.Add(slot);
        result.Add(slot);
      }

      return result.ToImmutable();
    }
  }
}