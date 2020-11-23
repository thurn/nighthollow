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

using Nighthollow.Data;
using Nighthollow.Services;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nighthollow.Interface
{
  public sealed class WorldScreenController : MonoBehaviour
  {
    [SerializeField] UIDocument _document = null!;
    [SerializeField] DataService _dataService = null!;
    public DataService DataService => _dataService;

    WorldScreen _worldScreen = null!;
    VisualElement _advisorBar = null!;
    CardsWindow _cardsWindow = null!;
    AbstractWindow? _currentWindow;
    ItemTooltip _itemTooltip = null!;

    void Start()
    {
      _worldScreen = InterfaceUtils.FindByName<WorldScreen>(_document.rootVisualElement, "WorldScreen");
      _worldScreen.Controller = this;
      _advisorBar = InterfaceUtils.FindByName<VisualElement>(_worldScreen, "AdvisorBar");
      _cardsWindow = InterfaceUtils.FindByName<CardsWindow>(_worldScreen, "CardsWindow");
      _cardsWindow.Controller = this;
      _itemTooltip = InterfaceUtils.FindByName<ItemTooltip>(_worldScreen, "ItemTooltip");
      _itemTooltip.Controller = this;
    }

    public bool ConsumesMousePosition(Vector3 mousePosition) =>
      _currentWindow != null || _advisorBar.ContainsPoint(mousePosition);

    public void ShowItemTooltip(CreatureItemData data, Rect anchor)
    {
      _itemTooltip.Show(data, anchor);
    }

    public void HideTooltip()
    {
      _itemTooltip.Hide();
    }

    public void HideCurrentWindow()
    {
      _currentWindow?.Hide();
      _currentWindow = null;
    }

    public void ShowCardsWindow() => ShowWindow(_cardsWindow);

    void ShowWindow(AbstractWindow window)
    {
      _currentWindow?.Hide();
      _currentWindow = window;
      _currentWindow.Show();
    }
  }
}
