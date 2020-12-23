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
using DG.Tweening;
using Nighthollow.Items;
using Nighthollow.Utils;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

[assembly: UxmlNamespacePrefix("Nighthollow.Interface", "nh")]

namespace Nighthollow.Interface
{
  public interface IElementKey
  {
    public string Name { get; }

    AbstractHideableElement FindByNameIn(VisualElement element);
  }

  public readonly struct ElementKey<T> : IElementKey where T : AbstractHideableElement
  {
    public string Name { get; }

    public ElementKey(string name)
    {
      Name = name;
    }

    public AbstractHideableElement FindByNameIn(VisualElement element) => InterfaceUtils.FindByName<T>(element, Name);
  }

  public sealed class ScreenController : MonoBehaviour
  {
    public static ElementKey<HideableVisualElement> Background =
      new ElementKey<HideableVisualElement>("Background");

    public static ElementKey<AdvisorBar> AdvisorBar =
      new ElementKey<AdvisorBar>("AdvisorBar");

    public static ElementKey<UserStatus> UserStatus =
      new ElementKey<UserStatus>("UserStatus");

    public static ElementKey<BlackoutWindow> BlackoutWindow =
      new ElementKey<BlackoutWindow>("BlackoutWindow");

    public static ElementKey<GameOverMessage> GameOverMessage =
      new ElementKey<GameOverMessage>("GameOverMessage");

    public static ElementKey<RewardsWindow> RewardsWindow =
      new ElementKey<RewardsWindow>("RewardsWindow");

    public static ElementKey<RewardChoiceWindow> RewardChoiceWindow =
      new ElementKey<RewardChoiceWindow>("RewardChoiceWindow");

    [SerializeField] UIDocument _document = null!;
    readonly Dictionary<string, AbstractHideableElement> _elements = new Dictionary<string, AbstractHideableElement>();

    readonly List<IElementKey> _keys = new List<IElementKey>
    {
      Background,
      AdvisorBar,
      UserStatus,
      BlackoutWindow,
      GameOverMessage,
      RewardsWindow,
      RewardChoiceWindow
    };

    DragInfo? _currentlyDragging;
    CardsWindow _cardsWindow = null!;
    AbstractWindow? _currentWindow;
    Dialog _dialog = null!;
    ItemTooltip _itemTooltip = null!;

    VisualElement _screen = null!;
    bool _currentlyWithinDragTarget;

    public UIDocument Document => _document;
    public VisualElement Screen => _screen;
    public bool IsCurrentlyDragging => _currentlyDragging != null;

    public void Initialize()
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

      _screen.RegisterCallback<MouseMoveEvent>(MouseMove);
      _screen.RegisterCallback<MouseUpEvent>(MouseUp);
    }

    public T Get<T>(ElementKey<T> key) where T : AbstractHideableElement => (T) GetElement(key);

    public void Show<T>(ElementKey<T> key) where T : DefaultHideableElement
    {
      Get(key).Show();
    }

    AbstractHideableElement GetElement(IElementKey key)
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
      if (!IsCurrentlyDragging)
      {
        _itemTooltip.Hide();
        _itemTooltip.Show(builder, anchor);
      }
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

    public void StartDrag(DragInfo dragInfo)
    {
      _currentlyDragging = dragInfo;
      var element = dragInfo.Element;
      HideTooltip();
      element.RemoveFromHierarchy();

      element.style.position = new StyleEnum<Position>(Position.Absolute);
      _screen.Add(element);
      element.style.left = new StyleLength(_currentlyDragging.DragStartElementPosition.x);
      element.style.top = new StyleLength(_currentlyDragging.DragStartElementPosition.y);
    }

    void MouseMove(MouseMoveEvent e)
    {
      if (_currentlyDragging != null)
      {
        var diff = e.mousePosition - _currentlyDragging.DragStartMousePosition;
        _currentlyDragging.Element.style.left = new StyleLength(_currentlyDragging.DragStartElementPosition.x + diff.x);
        _currentlyDragging.Element.style.top = new StyleLength(_currentlyDragging.DragStartElementPosition.y + diff.y);

        var withinTarget = _currentlyDragging.DragTarget.worldBound.Contains(e.mousePosition);
        if (!_currentlyWithinDragTarget && withinTarget)
        {
          _currentlyWithinDragTarget = true;
          _currentlyDragging.OnDragEnter();
        }
        else if (_currentlyWithinDragTarget && !withinTarget)
        {
          _currentlyWithinDragTarget = false;
          _currentlyDragging.OnDragExit();
        }
      }
    }

    void MouseUp(MouseUpEvent e)
    {
      if (_currentlyDragging != null)
      {
        var currentlyDragging = _currentlyDragging;
        _currentlyDragging = null;

        var element = currentlyDragging.Element;
        currentlyDragging.DisableElementDragging();

        var droppedOverTarget = currentlyDragging.DragTarget.worldBound.Contains(e.mousePosition);

        var targetParent = currentlyDragging.OriginalParent;
        var targetPosition = currentlyDragging.DragStartElementPosition;
        if (droppedOverTarget)
        {
          targetParent = currentlyDragging.OnDraggableReleased();
          targetPosition = targetParent.worldBound.position +
                           new Vector2(
                             currentlyDragging.OriginalLeft.value.value,
                             currentlyDragging.OriginalTop.value.value);
        }

        var currentPosition = new Vector2(element.style.left.value.value, element.style.top.value.value);
        var distance = Vector2.Distance(currentPosition, targetPosition);
        var animationDuration = distance < 100 ? 0.1f : 0.3f;

        DOTween.Sequence()
          .Insert(0, DOTween.To(
            () => currentPosition.x,
            left => element.style.left = left,
            targetPosition.x,
            animationDuration))
          .Insert(0, DOTween.To(
            () => currentPosition.y,
            left => element.style.top = left,
            targetPosition.y,
            animationDuration))
          .AppendCallback(() =>
          {
            element.RemoveFromHierarchy();
            targetParent.Add(element);
            element.style.position = currentlyDragging.OriginalPosition;
            element.style.left = currentlyDragging.OriginalLeft;
            element.style.top = currentlyDragging.OriginalTop;

            if (droppedOverTarget)
            {
              currentlyDragging.OnDragReceived();
            }

            currentlyDragging.MakeElementDraggable();
          });
      }
    }
  }
}