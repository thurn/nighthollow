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

using UnityEngine.UIElements;

namespace Nighthollow.Interface
{
  public sealed class CardsWindow : VisualElement
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
      RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }


    void OnGeometryChange(GeometryChangedEvent evt)
    {
      var self = this.Q("CardsWindow");
      this.Q("CloseButton").RegisterCallback<ClickEvent>(e =>
      {
        self.style.opacity = 0;
      });

      _collection = this.Q("Collection");
      _mainDeck = this.Q("MainDeck");
      _manaDeck = this.Q("ManaDeck");
      UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }
  }
}
