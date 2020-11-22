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

#nullable enable

using System.Collections.Generic;
using System.Linq;
using Nighthollow.Data;
using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nighthollow.Interface
{
  public sealed class CardsWindow : AbstractWindow
  {
    VisualElement _collection = null!;
    VisualElement _mainDeck = null!;
    VisualElement _manaDeck = null!;

    public WorldScreenController Controller { get; set; } = null!;

    public new sealed class UxmlFactory : UxmlFactory<CardsWindow, UxmlTraits>
    {
    }

    public new sealed class UxmlTraits : VisualElement.UxmlTraits
    {
    }

    public CardsWindow()
    {
      if (Application.isPlaying)
      {
        RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
      }
    }

    void OnGeometryChange(GeometryChangedEvent evt)
    {
      var self = this.Q("CardsWindow");
      this.Q("CloseButton").RegisterCallback<ClickEvent>(e => { self.style.opacity = 0; });

      _collection = this.Q("Collection");
      _mainDeck = this.Q("MainDeck");
      _manaDeck = this.Q("ManaDeck");

      UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    protected override void Render()
    {
      AddItems(_collection,
        Controller.DataService.UserDataService.Collection,
        20,
        "large");
      AddItems(_mainDeck,
        Controller.DataService.UserDataService.Deck.Where(i => !i.BaseType.IsManaCreature).ToList(),
        9,
        "large");
      AddItems(_manaDeck,
        Controller.DataService.UserDataService.Deck.Where(i => i.BaseType.IsManaCreature).ToList(),
        6,
        "small");
    }

    void AddItems(VisualElement parentElement, IReadOnlyList<CreatureItemData> items, int count, string size)
    {
      parentElement.Clear();
      var index = 0;
      for (; index < items.Count; ++index)
      {
        parentElement.Add(RenderItem(items[index], size));
      }

      for (; index < count; ++index)
      {
        parentElement.Add(EmptyItem(size));
      }
    }

    VisualElement EmptyItem(string size)
    {
      var result = new VisualElement();
      result.AddToClassList("card");
      result.AddToClassList(size);
      return result;
    }

    VisualElement RenderItem(CreatureItemData card, string size)
    {
      var result = new VisualElement();
      result.AddToClassList("card");
      result.AddToClassList(size);
      var image = new Image();
      image.style.backgroundImage = new StyleBackground(Controller.DataService.AssetService.GetImage(
        Errors.CheckNotNull(card.BaseType.ImageAddress)));
      result.Add(image);
      return result;
    }
  }
}
