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
using Nighthollow.Interface;
using Nighthollow.Services;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Items
{
  public sealed class ItemImage : VisualElement
  {
    readonly bool _addTooltip;
    readonly IItemData _item;
    readonly ScreenController _screenController;

    public new sealed class UxmlFactory : UxmlFactory<HideableVisualElement, UxmlTraits>
    {
    }

    public ItemImage(ScreenController screenController, IItemData item, bool addTooltip = true)
    {
      _screenController = screenController;
      _item = item;
      _addTooltip = addTooltip;

      RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    public void AddTooltip()
    {
      RegisterCallback<MouseOverEvent>(OnHover);
      RegisterCallback<MouseOutEvent>(e => { _screenController.HideTooltip(); });
    }

    void OnHover(MouseOverEvent e)
    {
      var tooltipBuilder =
        _item.Switch(creature =>
            CreatureItemTooltip.Create(
              Database.Instance.UserData.Stats,
              creature),
          resource => new TooltipBuilder(resource.Name));
      tooltipBuilder.XOffset = 128;

      _screenController.ShowTooltip(tooltipBuilder, new Vector2(worldBound.x, worldBound.y));
    }

    void OnGeometryChange(GeometryChangedEvent evt)
    {
      var image = new Image();
      image.style.backgroundImage = new StyleBackground(Database.Instance.Assets.GetImage(_item.ImageAddress));
      Add(image);

      if (_addTooltip)
      {
        AddTooltip();
      }

      UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }
  }
}
