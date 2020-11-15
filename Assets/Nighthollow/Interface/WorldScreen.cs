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

using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nighthollow.Interface
{
  public sealed class WorldScreen : VisualElement
  {
    public static WorldScreen FindInDocument(UIDocument document) =>
      Errors.CheckNotNull((WorldScreen) document.rootVisualElement.Q("WorldScreen"));

    VisualElement _advisorBar = null!;

    public new sealed class UxmlFactory : UxmlFactory<WorldScreen, UxmlTraits>
    {
    }

    public new sealed class UxmlTraits : VisualElement.UxmlTraits
    {
    }

    public WorldScreen()
    {
      RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    public bool ContainsMousePosition(Vector3 mousePosition) => _advisorBar.ContainsPoint(mousePosition);

    void OnGeometryChange(GeometryChangedEvent evt)
    {
      _advisorBar = this.Q("AdvisorBar");
      _advisorBar.Q("CardsButton").RegisterCallback<ClickEvent>(e => Debug.Log("Cards Button"));
      UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }
  }
}
