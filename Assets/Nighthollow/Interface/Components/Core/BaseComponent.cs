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
using System.Collections.Generic;
using System.Collections.Immutable;
using Nighthollow.Utils;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

#nullable enable

namespace Nighthollow.Interface.Components.Core
{
  public abstract record BaseComponent
  {
    protected GlobalKey? GlobalKey;

    public string? LocalKey { get; init; }
    public int MarginLeft { get; init; }
    public int MarginTop { get; init; }
    public int MarginRight { get; init; }
    public int MarginBottom { get; init; }
    public Position? FlexPosition { get; init; }
    public Length? Left { get; init; }
    public Length? Top { get; init; }
    public Length? Right { get; init; }
    public Length? Bottom { get; init; }
    public ImmutableHashSet<string> ClassNames { get; init; } = ImmutableHashSet<string>.Empty;
    public Length? MaxWidth { get; init; }
    public Length? MaxHeight { get; init; }

    public Length? InsetLeftRight
    {
      init
      {
        Left = value;
        Right = value;
      }
    }

    public Length? InsetTopBottom
    {
      init
      {
        Top = value;
        Bottom = value;
      }
    }

    public Length? InsetAll
    {
      init
      {
        InsetLeftRight = value;
        InsetTopBottom = value;
      }
    }

    public int MarginLeftRight
    {
      init
      {
        MarginLeft = value;
        MarginRight = value;
      }
    }

    public int MarginTopBottom
    {
      init
      {
        MarginTop = value;
        MarginBottom = value;
      }
    }

    public int MarginAll
    {
      init
      {
        MarginTopBottom = value;
        MarginLeftRight = value;
      }
    }

    public abstract IMountComponent Reduce(GlobalKey globalKey);

    protected static IEnumerable<BaseComponent> List(params BaseComponent?[] children) => children.WhereNotNull();

    protected IState<T> UseState<T>(T initialValue)
    {
      if (GlobalKey == null)
      {
        throw new NullReferenceException($"Error: Invoked {nameof(UseState)}() outside of OnRender()/OnMount()");
      }

      return GlobalKey.UseState(GetType(), initialValue);
    }

    protected T? UseResource<T>(string? address) where T : Object
    {
      if (address == null)
      {
        return null;
      }

      if (GlobalKey == null)
      {
        throw new NullReferenceException($"Error: Invoked {nameof(UseResource)}() outside of OnRender()/OnMount()");
      }

      return GlobalKey.UseResource<T>(address);
    }

    protected IMakeDraggable UseDraggable()
    {
      if (GlobalKey == null)
      {
        throw new NullReferenceException($"Error: Invoked {nameof(UseDraggable)}() outside of OnRender()/OnMount()");
      }

      return GlobalKey.UseDraggable();
    }

    protected BaseComponent MergeCommonProps(BaseComponent child) => child with
    {
      MarginLeft = MarginLeft + child.MarginLeft,
      MarginTop = MarginTop + child.MarginTop,
      MarginRight = MarginRight + child.MarginRight,
      MarginBottom = MarginBottom + child.MarginBottom,
      Left = AddLengths(Left, child.Left),
      Top = AddLengths(Top, child.Top),
      Bottom = AddLengths(Bottom, child.Bottom),
      Right = AddLengths(Right, child.Right),
      FlexPosition = child.FlexPosition ?? FlexPosition,
      ClassNames = ClassNames.Union(child.ClassNames),
      MaxWidth = child.MaxWidth ?? MaxWidth,
      MaxHeight = child.MaxHeight ?? MaxHeight
    };

    static Length? AddLengths(Length? x, Length? y) => (x, y) switch
    {
      (null, null) => null,
      ({ } a, null) => a,
      (null, { } b) => b,
      ({ } a, { } b) when a.unit == b.unit => new Length(a.value + b.value, a.unit),
      _ => throw new InvalidOperationException($"Attempting to add Lengths with non-matching units {x} + {y}")
    };
  }
}