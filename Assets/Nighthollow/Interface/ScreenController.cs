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


using System.Collections.Generic;
using System.Linq;
using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

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

    public HideableElement FindByNameIn(VisualElement element)
    {
      return InterfaceUtils.FindByName<T>(element, Name);
    }
  }

  public sealed class ScreenController : MonoBehaviour
  {
    public static ElementKey<HideableVisualElement> Background = new ElementKey<HideableVisualElement>("Background");
    public static ElementKey<AdvisorBar> AdvisorBar = new ElementKey<AdvisorBar>("AdvisorBar");
    public static ElementKey<UserStatus> UserStatus = new ElementKey<UserStatus>("UserStatus");
    public static ElementKey<BlackoutWindow> BlackoutWindow = new ElementKey<BlackoutWindow>("BlackoutWindow");
    public static ElementKey<GameOverMessage> GameOverMessage = new ElementKey<GameOverMessage>("GameOverMessage");
    [SerializeField] UIDocument _document = null!;

    readonly Dictionary<string, HideableElement> _elements = new Dictionary<string, HideableElement>();

    readonly List<IElementKey> _keys = new List<IElementKey>
    {
      Background,
      AdvisorBar,
      UserStatus,
      BlackoutWindow,
      GameOverMessage
    };

    CardsWindow _cardsWindow = null!;
    AbstractWindow? _currentWindow;
    Dialog _dialog = null!;
    ItemTooltip _itemTooltip = null!;

    VisualElement _screen = null!;

    public UIDocument Document => _document;
    public VisualElement Screen => _screen;

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

      if (showAdvisorBar) Get(AdvisorBar).Show();
    }

    public T Get<T>(ElementKey<T> key) where T : HideableElement
    {
      return (T) GetElement(key);
    }

    public void Show<T>(ElementKey<T> key) where T : DefaultHideableElement
    {
      Get(key).Show();
    }

    HideableElement GetElement(IElementKey key)
    {
      Errors.CheckState(_elements.ContainsKey(key.Name), $"Element not found: {key.Name}");
      return _elements[key.Name];
    }

    public bool ConsumesMousePosition(Vector3 mousePosition)
    {
      return _currentWindow != null ||
             _elements.Values
               .Any(element => element.Visible && InterfaceUtils.ContainsScreenPoint(element, mousePosition)) ||
             _dialog.Visible ||
             _itemTooltip.Visible && InterfaceUtils.ContainsScreenPoint(_itemTooltip, mousePosition);
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

    public void ShowDialog(string portraitName, string text, bool hideCloseButton = false)
    {
      _dialog.Show(portraitName, text, hideCloseButton);
    }

    public void HideDialog()
    {
      _dialog.Hide();
    }

    public void ShowCardsWindow()
    {
      ShowWindow(_cardsWindow);
    }

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
