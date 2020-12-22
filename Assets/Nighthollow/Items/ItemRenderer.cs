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
using Nighthollow.Data;
using Nighthollow.Interface;
using Nighthollow.Utils;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Items
{
  public static class ItemRenderer
  {
    public enum Size
    {
      Large,
      Small
    }

    public readonly struct Config
    {
      public readonly int? Count;
      public readonly Size Size;
      public readonly bool ShouldAddTooltip;

      public Config(int? count = null, Size size = Size.Large, bool shouldAddTooltip = true)
      {
        Count = count;
        Size = size;
        ShouldAddTooltip = shouldAddTooltip;
      }
    }

    public static void AddItems(
      ScreenController controller,
      VisualElement parentElement,
      IReadOnlyList<IItemData> items,
      Config config)
    {
      parentElement.Clear();
      var index = 0;
      for (; index < items.Count; ++index)
      {
        parentElement.Add(RenderItem(controller, items[index], config));
      }

      for (; index < (config.Count ?? 0); ++index)
      {
        parentElement.Add(EmptyItem(config));
      }
    }

    public static VisualElement EmptyItem(Config config)
    {
      var result = new VisualElement();
      result.AddToClassList("card");
      result.AddToClassList(SizeClass(config));
      return result;
    }

    public static VisualElement RenderItem(ScreenController controller, IItemData card, Config config)
    {
      var result = new VisualElement();
      result.AddToClassList("card");
      result.AddToClassList(SizeClass(config));
      result.Add(new ItemImage(controller, card, config.ShouldAddTooltip));
      return result;
    }

    static string SizeClass(Config config)
    {
      return config.Size switch
      {
        Size.Large => "large",
        Size.Small => "small",
        _ => throw Errors.UnknownEnumValue(config.Size)
      };
    }
  }
}
