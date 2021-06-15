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

using System.Collections.Immutable;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface.Components.Core
{
  public interface IMountComponent
  {
    public string? LocalKey { get; }

    string Type { get; }

    VisualElement CreateMountContent();

    void Mount(GlobalKey hooks, VisualElement element);

    void ReduceChildren(GlobalKey hooks);

    ImmutableList<IMountComponent> GetReducedChildren();
  }

  public abstract record MountComponent<TElement> : BaseComponent, IMountComponent where TElement : VisualElement
  {
    protected abstract TElement OnCreateMountContent();

    /// <summary>
    /// Updates the provided VisualElement to match the current component.
    /// </summary>
    /// <remarks>
    /// NOTE: because elements are reused across contexts, any kind of conditional logic inside OnMount is very
    /// unsafe. For example, if you write code like:
    /// <code>
    /// if (BackgroundColor != null)
    /// {
    ///   element.style.backgroundColor = BackgroundColor;
    /// }
    /// </code>
    /// then the previously-set background color will not be removed correctly!
    /// </remarks>
    /// <param name="element"></param>
    protected abstract void OnMount(TElement element);

    public abstract string Type { get; }

    public override IMountComponent Reduce(GlobalKey globalKey) => this;

    public VisualElement CreateMountContent() => OnCreateMountContent();

    public void Mount(GlobalKey globalKey, VisualElement element)
    {
      GlobalKey = globalKey;
      ApplyCommonProps(element);
      OnMount((TElement) element);
      GlobalKey = null;
    }

    public virtual void ReduceChildren(GlobalKey globalKey)
    {
    }

    public virtual ImmutableList<IMountComponent> GetReducedChildren() => ImmutableList<IMountComponent>.Empty;

    void ApplyCommonProps(VisualElement element)
    {
      element.style.position = FlexPosition ?? Position.Relative;
      element.style.left = Left ?? new StyleLength(StyleKeyword.Null);
      element.style.top = Top ?? new StyleLength(StyleKeyword.Null);
      element.style.bottom = Bottom ?? new StyleLength(StyleKeyword.Null);
      element.style.right = Right ?? new StyleLength(StyleKeyword.Null);
      element.style.marginLeft = MarginLeft;
      element.style.marginTop = MarginTop;
      element.style.marginRight = MarginRight;
      element.style.marginBottom = MarginBottom;
    }
  }
}