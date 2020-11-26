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
using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Nighthollow.Interface
{
  public interface IElementKey
  {
    public string Name { get; }

    HideableElement FindByNameIn(VisualElement element);
  }

  public readonly struct ElementKey<T> : IElementKey where T : HideableElement
  {
    public string Name { get; }

    public ElementKey(string name)
    {
      Name = name;
    }

    public HideableElement FindByNameIn(VisualElement element) => InterfaceUtils.FindByName<T>(element, Name);
  }

  public sealed class ScreenController : MonoBehaviour
  {
    [SerializeField] UIDocument _document = null!;

    public UIDocument Document => _document;

    VisualElement _screen = null!;
    public VisualElement Screen => _screen;

    public static ElementKey<AdvisorBar> AdvisorBar = new ElementKey<AdvisorBar>("AdvisorBar");
    public static ElementKey<UserStatus> UserStatus = new ElementKey<UserStatus>("UserStatus");

    readonly List<IElementKey> _keys = new List<IElementKey>
    {
      AdvisorBar,
      UserStatus
    };

    readonly Dictionary<string, HideableElement> _elements = new Dictionary<string, HideableElement>();

    CardsWindow _cardsWindow = null!;
    AbstractWindow? _currentWindow;
    ItemTooltip _itemTooltip = null!;
    Dialog _dialog = null!;

    public void Initialize(bool showAdvisorBar)
    {
      _screen = InterfaceUtils.FindByName<VisualElement>(_document.rootVisualElement, "Screen");

      foreach (var key in _keys)
      {
        var element = key.FindByNameIn(_screen);
        element.Controller = this;
        _elements[key.Name] = element;
      }

      _cardsWindow = InterfaceUtils.FindByName<CardsWindow>(_screen, "CardsWindow");
      _cardsWindow.Controller = this;
      _itemTooltip = InterfaceUtils.FindByName<ItemTooltip>(_screen, "ItemTooltip");
      _itemTooltip.Controller = this;
      _dialog = InterfaceUtils.FindByName<Dialog>(_screen, "Dialog");
      _dialog.Initialize();

      if (showAdvisorBar)
      {
        Show(AdvisorBar);
      }
    }

    public T Get<T>(ElementKey<T> key) where T : HideableElement
    {
      Errors.CheckState(_elements.ContainsKey(key.Name), $"Element not found: {key.Name}");
      return (T) _elements[key.Name];
    }

    public void Show(IElementKey key)
    {
      Errors.CheckState(_elements.ContainsKey(key.Name), $"Element not found: {key.Name}");
      _elements[key.Name].Show();
    }

    public void Hide(IElementKey key)
    {
      Errors.CheckState(_elements.ContainsKey(key.Name), $"Element not found: {key.Name}");
      _elements[key.Name].Hide();
    }

    public bool ConsumesMousePosition(Vector3 mousePosition)
    {
      return _currentWindow != null ||
             _elements.Values
               .Any(element => element.Visible && InterfaceUtils.ContainsScreenPoint(_itemTooltip, mousePosition)) ||
             _dialog.Visible ||
             (_itemTooltip.Visible && InterfaceUtils.ContainsScreenPoint(_itemTooltip, mousePosition));
    }

    public void ShowTooltip(TooltipBuilder builder, Vector2 anchor)
    {
      _itemTooltip.Hide();
      _itemTooltip.Show(builder, anchor);
    }

    public void HideTooltip()
    {
      _itemTooltip.Hide();
    }

    public void ShowDialog(string portraitName, string text) => _dialog.Show(portraitName, text);

    public void HideDialog() => _dialog.Hide();

    public void ShowCardsWindow() => ShowWindow(_cardsWindow);

    public void HideCurrentWindow()
    {
      _currentWindow?.Hide();
      _currentWindow = null;
    }

    void ShowWindow(AbstractWindow window)
    {
      _currentWindow?.Hide();
      _currentWindow = window;
      _currentWindow.Show();
    }
  }
}
