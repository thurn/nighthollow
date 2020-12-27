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

using Nighthollow.Model;
using Nighthollow.Services;
using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public sealed class ItemSlot : VisualElement, IDragTarget<ItemImage, ItemSlot>
  {
    public enum Size
    {
      Small,
      Large
    }

    IItemData? _item;

    public IItemData? Item => _item;
    Image? _nullStateImage;

    public new sealed class UxmlFactory : UxmlFactory<ItemSlot, UxmlTraits>
    {
    }

    public ItemSlot()
    {
    }

    public ItemSlot(Size size = Size.Large)
    {
      AddToClassList("item-slot");
      AddToClassList(SizeClass(size));
    }

    public void SetNullStateImage(string address)
    {
      if (_nullStateImage == null)
      {
        _nullStateImage = new Image {tintColor = new Color(0.5f, 0.5f, 0.5f)};
        Add(_nullStateImage);
      }

      _nullStateImage.sprite = Database.Instance.Assets.GetImage(address);
    }

    public void ClearNullStateImage()
    {
      if (_nullStateImage != null)
      {
        _nullStateImage.RemoveFromHierarchy();
        _nullStateImage = null;
      }
    }

    public void SetItem(ScreenController controller, IItemData item, bool shouldAddTooltip = true)
    {
      _item = item;
      Add(new ItemImage(controller, this, item, shouldAddTooltip));
      ClearNullStateImage();
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

    public ItemSlot This() => this;

    public VisualElement DragTargetElement => this;

    public void OnDraggableElementReceived(ItemImage element)
    {
      _item = element.Item;
    }

    public void OnDraggableElementRemoved(ItemImage element)
    {
      _item = null;
    }
  }
}
