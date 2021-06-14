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

using Nighthollow.Data;
using Nighthollow.Items;
using Nighthollow.Services;
using Nighthollow.Utils;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public sealed class InterfaceItemSlot : VisualElement, IDragTarget<InterfaceItemImage, InterfaceItemSlot>
  {
    public enum Size
    {
      Small,
      Large
    }

    IItemData? _item;

    public IItemData? Item => _item;

    public new sealed class UxmlFactory : UxmlFactory<InterfaceItemSlot, UxmlTraits>
    {
    }

    public InterfaceItemSlot()
    {
    }

    public InterfaceItemSlot(Size size = Size.Large)
    {
      AddToClassList("item-slot");
      AddToClassList(SizeClass(size));
    }

    public void SetItem(ServiceRegistry registry, IItemData item, ItemRenderer.Config config)
    {
      _item = item;
      var image = new InterfaceItemImage(registry, this, item, config.ShouldAddTooltip);
      Add(image);
      if (config.DragManager != null)
      {
        image.MakeDraggable(config.DragManager);
      }
    }

    static string SizeClass(Size size)
    {
      return size switch
      {
        Size.Large => "large",
        Size.Small => "small",
        _ => throw Errors.UnknownEnumValue(size)
      };
    }

    public InterfaceItemSlot This() => this;

    public VisualElement DragTargetElement => this;

    public void OnDraggableElementReceived(InterfaceItemImage element)
    {
      _item = element.Item;
    }

    public void OnDraggableElementRemoved(InterfaceItemImage element)
    {
      _item = null;
    }
  }
}