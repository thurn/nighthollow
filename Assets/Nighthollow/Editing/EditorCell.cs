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
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Editing
{
  public abstract class EditorCell : VisualElement
  {
    protected EditorCell()
    {
      AddToClassList("editor-cell");
    }

    public virtual void Select()
    {
      AddToClassList("selected");
    }

    public virtual void Unselect()
    {
      RemoveFromClassList("selected");
    }

    public virtual void Activate()
    {
    }

    public virtual void Deactivate()
    {
    }

    public virtual void OnParentKeyDown(KeyDownEvent keyDownEvent)
    {
    }
  }

  public abstract class EditorCellDelegate
  {
    public virtual void Initialize(TextField field, IEditor parent)
    {
    }

    public abstract string? RenderPreview(object? value);

    public abstract void OnActivate(TextField field, Rect worldBound);

    public virtual void OnParentKeyDown(KeyDownEvent evt)
    {
    }

    public virtual void OnDeactivate(TextField field)
    {
    }
  }
}