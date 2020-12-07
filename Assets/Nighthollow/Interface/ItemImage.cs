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
using Nighthollow.Services;
using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public sealed class ItemImage : VisualElement
  {
    readonly bool _addTooltip;
    readonly CreatureItemData _item;
    readonly ScreenController _screenController;

    public new sealed class UxmlFactory : UxmlFactory<HideableVisualElement, UxmlTraits>
    {
    }

    public ItemImage(ScreenController screenController, CreatureItemData item, bool addTooltip = true)
    {
      _screenController = screenController;
      _item = item;
      _addTooltip = addTooltip;

      RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    public void AddTooltip()
    {
      RegisterCallback<MouseOverEvent>(e =>
      {
        var tooltipBuilder = CreatureItemTooltip.Create(
          Database.Instance.UserData.Stats,
          _item);
        tooltipBuilder.XOffset = 128;

        _screenController.ShowTooltip(tooltipBuilder, new Vector2(worldBound.x, worldBound.y));
      });
      RegisterCallback<MouseOutEvent>(e => { _screenController.HideTooltip(); });
    }

    void OnGeometryChange(GeometryChangedEvent evt)
    {
      var image = new Image();
      image.style.backgroundImage = new StyleBackground(Database.Instance.Assets.GetImage(
        Errors.CheckNotNull(_item.BaseType.ImageAddress)));
      Add(image);

      if (_addTooltip)
      {
        AddTooltip();
      }

      UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }
  }
}
