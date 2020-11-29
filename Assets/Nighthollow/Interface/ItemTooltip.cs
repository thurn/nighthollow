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


using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public sealed class ItemTooltip : VisualElement
  {
    Vector2? _anchor;
    VisualElement _closeButton = null!;
    bool _initialized;
    Label _title = null!;
    int _xOffset;

    public ItemTooltip()
    {
      if (Application.isPlaying) RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    public ScreenController Controller { get; set; } = null!;

    public bool Visible { get; private set; }

    public void Show(TooltipBuilder builder, Vector2 anchorPoint)
    {
      style.visibility = new StyleEnum<Visibility>(Visibility.Visible);
      _anchor = anchorPoint;
      _title.text = builder.Name;
      _xOffset = builder.XOffset;
      Visible = true;

      _closeButton.style.visibility = new StyleEnum<Visibility>(
        builder.CloseButton ? Visibility.Visible : Visibility.Hidden);

      var onClose = builder.OnClose;
      if (onClose != null) _closeButton.RegisterCallback<ClickEvent>(e => { onClose(); });

      var content = this.Q(name: null, "tooltip-content");
      if (content != null) Remove(content);

      Add(builder.BuildContent());

      SetPosition();
    }

    public void Hide()
    {
      Visible = false;
      style.visibility = new StyleEnum<Visibility>(Visibility.Hidden);
      _closeButton.style.visibility = new StyleEnum<Visibility>(Visibility.Hidden);
      _anchor = null;
    }

    void OnGeometryChange(GeometryChangedEvent evt)
    {
      if (!_initialized)
      {
        _title = InterfaceUtils.FindByName<Label>(this, "Title");
        _closeButton = this.Q("CloseButton");
        _initialized = true;
      }

      SetPosition();
    }

    void SetPosition()
    {
      if (_anchor != null)
      {
        style.left = new StyleLength(_anchor.Value.x > Screen.width / 2f
          ? _anchor.Value.x - worldBound.width - _xOffset
          : _anchor.Value.x + _xOffset);
        style.top = new StyleLength(Mathf.Clamp(
          _anchor.Value.y - worldBound.height / 2f,
          min: 32,
          Screen.height - worldBound.height - 32));
      }
    }

    public new sealed class UxmlFactory : UxmlFactory<ItemTooltip, UxmlTraits>
    {
    }

    public new sealed class UxmlTraits : VisualElement.UxmlTraits
    {
    }
  }
}
