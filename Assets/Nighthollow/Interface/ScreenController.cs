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
using Nighthollow.Editing;
using Nighthollow.Items;
using Nighthollow.Services;
using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public interface IElementKey
  {
    public string Name { get; }

    AbstractHideableElement FindByNameIn(VisualElement element);
  }

  public readonly struct ElementKey<T> : IElementKey where T : AbstractHideableElement, new()
  {
    readonly bool _createAtRuntime;
    public string Name { get; }

    public ElementKey(string name, bool createAtRuntime = false)
    {
      Name = name;
      _createAtRuntime = createAtRuntime;
    }

    public AbstractHideableElement FindByNameIn(VisualElement element)
    {
      if (_createAtRuntime)
      {
        var result = new T();
        element.Add(result);
        return result;
      }
      else
      {
        return InterfaceUtils.FindByName<T>(element, Name);
      }
    }
  }

  public sealed class ScreenController
  {
    public static ElementKey<HideableVisualElement> Background =
      new ElementKey<HideableVisualElement>("Background");

    public static ElementKey<AdvisorBar> AdvisorBar =
      new ElementKey<AdvisorBar>("AdvisorBar");

    public static ElementKey<CharacterDialogue> CharacterDialogue =
      new ElementKey<CharacterDialogue>("CharacterDialogue");

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

    public static ElementKey<GameDataEditor> GameDataEditor =
      new ElementKey<GameDataEditor>("GameDataEditor", createAtRuntime: true);

    public static ElementKey<SchoolSelectionScreen> SchoolSelectionScreen =
      new ElementKey<SchoolSelectionScreen>("SchoolSelectionScreen");

    public static ElementKey<BlackoutWindow> LoadingBlackout =
      new ElementKey<BlackoutWindow>("LoadingBlackout");

    public static ElementKey<CardsWindow> CardsWindow =
      new ElementKey<CardsWindow>("CardsWindow");

    readonly Dictionary<string, AbstractHideableElement> _elements = new Dictionary<string, AbstractHideableElement>();

    readonly List<IElementKey> _keys = new List<IElementKey>
    {
      Background,
      AdvisorBar,
      CharacterDialogue,
      UserStatus,
      BlackoutWindow,
      GameOverMessage,
      RewardsWindow,
      RewardChoiceWindow,
      GameDataEditor,
      SchoolSelectionScreen,
      LoadingBlackout,
      CardsWindow
    };

    readonly ServiceRegistry _registry;
    readonly VisualElement _screen;
    public VisualElement Screen => _screen;
    readonly ItemTooltip _itemTooltip;

    DragInfo? _currentlyDragging;
    int? _currentlyWithinDragTarget;
    public bool IsCurrentlyDragging => _currentlyDragging != null;

    public ScreenController(VisualElement rootVisualElement, ServiceRegistry registry)
    {
      _screen = InterfaceUtils.FindByName<VisualElement>(rootVisualElement, "Screen");
      _registry = registry;

      foreach (var key in _keys)
      {
        var element = key.FindByNameIn(_screen);
        element.Registry = _registry;
        element.Controller = this;
        _elements[key.Name] = element;
      }

      _itemTooltip = InterfaceUtils.FindByName<ItemTooltip>(_screen, "ItemTooltip");
      _itemTooltip.Controller = this;

      _screen.RegisterCallback<MouseMoveEvent>(MouseMove);
      _screen.RegisterCallback<MouseUpEvent>(MouseUp);

      Get(LoadingBlackout).Hide();
    }

    public void OnUpdate()
    {
      if (Input.GetKeyDown(KeyCode.E) && CtrlOrCmdDown() && ShiftDown())
      {
        var editor = Get(GameDataEditor);
        if (editor.Visible)
        {
          editor.Hide();
        }
        else
        {
          editor.Show(new GameDataEditor.Args(_registry.Database), true);
        }
      }

      if (Input.GetKeyDown(KeyCode.R) && CtrlOrCmdDown())
      {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
      }

      if (Input.anyKeyDown)
      {
        InterfaceUtils.FindByName<VisualElement>(Screen, "HelperTextContainer").Clear();
      }
    }

    static bool CtrlOrCmdDown() => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) ||
                                   Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand);

    static bool ShiftDown() => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

    public T Get<T>(ElementKey<T> key) where T : AbstractHideableElement, new() => (T) GetElement(key);

    public void Show<T>(ElementKey<T> key) where T : DefaultHideableElement, new()
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
      return _elements.Values
               .Any(element => element.Visible && InterfaceUtils.ContainsScreenPoint(element, mousePosition)) ||
             _itemTooltip.Visible && InterfaceUtils.ContainsScreenPoint(_itemTooltip, mousePosition);
    }

    public bool HasExclusiveFocus() => _elements.Values.Any(e => e.Visible && e.ExclusiveFocus);

    public void HideExclusiveElements()
    {
      foreach (var element in _elements.Values.Where(e => e.ExclusiveFocus))
      {
        element.Hide();
      }
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

    public void StartDrag(DragInfo dragInfo)
    {
      _currentlyDragging = dragInfo;
      var element = dragInfo.Element;
      HideTooltip();
      element.RemoveFromHierarchy();

      element.style.position = new StyleEnum<Position>(Position.Absolute);
      _screen.Add(element);
      element.style.left = _currentlyDragging.DragStartElementPosition.x;
      element.style.top = _currentlyDragging.DragStartElementPosition.y;
      element.style.width = element.contentRect.width;
      element.style.height = element.contentRect.height;
    }

    void MouseMove(MouseMoveEvent e)
    {
      if (_currentlyDragging != null)
      {
        var diff = e.mousePosition - _currentlyDragging.DragStartMousePosition;
        _currentlyDragging.Element.style.left = new StyleLength(_currentlyDragging.DragStartElementPosition.x + diff.x);
        _currentlyDragging.Element.style.top = new StyleLength(_currentlyDragging.DragStartElementPosition.y + diff.y);

        var withinDragTarget = FindWithinDragTarget(_currentlyDragging, e);
        if (_currentlyWithinDragTarget == null && withinDragTarget != null)
        {
          _currentlyWithinDragTarget = withinDragTarget;
          _currentlyDragging.OnDragEnter(withinDragTarget.Value);
        }
        else if (_currentlyWithinDragTarget != null && withinDragTarget == null)
        {
          _currentlyDragging.OnDragExit(_currentlyWithinDragTarget.Value);
          _currentlyWithinDragTarget = null;
        }
      }
    }

    static int? FindWithinDragTarget(DragInfo currentlyDragging, IMouseEvent e)
    {
      for (var i = 0; i < currentlyDragging.TargetElements.Count; ++i)
      {
        var target = currentlyDragging.TargetElements[i];
        if (target.worldBound.Contains(e.mousePosition))
        {
          return i;
        }
      }

      return null;
    }

    void MouseUp(MouseUpEvent e)
    {
      if (_currentlyDragging != null)
      {
        var currentlyDragging = _currentlyDragging;
        _currentlyDragging = null;
        _currentlyWithinDragTarget = null;

        var element = currentlyDragging.Element;
        currentlyDragging.EnableDragging = false;

        var droppedOverTargetIndex = FindWithinDragTarget(currentlyDragging, e);

        var targetParent = currentlyDragging.OriginalParent;
        if (droppedOverTargetIndex != null)
        {
          targetParent = currentlyDragging.GetTarget(droppedOverTargetIndex.Value);
        }

        element.RemoveFromHierarchy();
        targetParent.Add(element);
        element.style.position = currentlyDragging.OriginalPosition;
        element.style.left = currentlyDragging.OriginalLeft;
        element.style.top = currentlyDragging.OriginalTop;
        element.style.width = currentlyDragging.OriginalWidth;
        element.style.height = currentlyDragging.OriginalHeight;

        if (droppedOverTargetIndex != null)
        {
          currentlyDragging.OnDropped(droppedOverTargetIndex.Value);
        }

        currentlyDragging.EnableDragging = true;
      }
    }
  }
}