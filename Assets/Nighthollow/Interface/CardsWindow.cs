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

using System.Linq;
using Nighthollow.Items;
using Nighthollow.Services;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public sealed class CardsWindow : AbstractWindow
  {
    VisualElement _collection = null!;
    VisualElement _mainDeck = null!;
    VisualElement _manaDeck = null!;

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

    public ScreenController Controller { get; set; } = null!;

    void OnGeometryChange(GeometryChangedEvent evt)
    {
      this.Q("CloseButton").RegisterCallback<ClickEvent>(e => { Controller.HideCurrentWindow(); });

      _collection = this.Q("Collection");
      _mainDeck = this.Q("MainDeck");
      _manaDeck = this.Q("ManaDeck");

      UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    protected override void Render()
    {
      ItemRenderer.AddItems(
        Controller,
        _collection,
        Database.Instance.UserData.Collection,
        new ItemRenderer.Config(count: 20));
      ItemRenderer.AddItems(
        Controller,
        _mainDeck,
        Database.Instance.UserData.Deck.Where(i => !i.BaseType.IsManaCreature).ToList(),
        new ItemRenderer.Config(count: 9));
      ItemRenderer.AddItems(
        Controller,
        _manaDeck,
        Database.Instance.UserData.Deck.Where(i => i.BaseType.IsManaCreature).ToList(),
        new ItemRenderer.Config(count: 6, ItemRenderer.Size.Small));
    }
  }
}
