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

using System;
using Nighthollow.Items;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public sealed class Tooltip : HideableElement<TooltipBuilder>
  {
    Label _title = null!;
    VisualElement _closeButton = null!;
    Action? _onHide;
    Vector2 _anchorPoint;
    int _xOffset;

    public new sealed class UxmlFactory : UxmlFactory<Tooltip, UxmlTraits>
    {
    }

    protected override void Initialize()
    {
      _title = Find<Label>("TooltipTitle");
      _closeButton = FindElement("TooltipCloseButton");
      _closeButton.RegisterCallback<ClickEvent>(_ => Hide());
      RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    protected override void OnShow(TooltipBuilder builder)
    {
      _anchorPoint = builder.AnchorPoint;
      _xOffset = builder.XOffset;

      style.visibility = new StyleEnum<Visibility>(Visibility.Visible);
      _title.text = builder.Name;

      _closeButton.style.visibility = new StyleEnum<Visibility>(
        builder.CloseButton ? Visibility.Visible : Visibility.Hidden);

      _onHide = builder.OnHide;

      var content = this.Q(className: "tooltip-content");
      if (content != null)
      {
        Remove(content);
      }

      Add(builder.BuildContent());

      // We need to call SetPosition both here and in OnGeometryChanged because we need to use the updated
      // world bounds if they're available, but OnGeometryChanged doesn't get invoked if the content is the same as
      // the previous tooltip.
      SetPosition();
    }

    void OnGeometryChanged(GeometryChangedEvent evt)
    {
      SetPosition();
    }

    void SetPosition()
    {
      style.left = new StyleLength(_anchorPoint.x > Screen.width / 2f
        ? _anchorPoint.x - worldBound.width - _xOffset
        : _anchorPoint.x + _xOffset);
      style.top = new StyleLength(Mathf.Clamp(
        _anchorPoint.y - worldBound.height / 2f,
        min: 32,
        Screen.height - worldBound.height - 32));
    }

    protected override void OnHide()
    {
      _onHide?.Invoke();
    }
  }
}