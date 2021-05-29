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
using System.Collections.Immutable;
using System.Linq;
using Nighthollow.Services;
using UnityEngine;

#nullable enable

namespace Nighthollow.Rules.Events
{
  public sealed class HotkeyPressedEvent : IEvent
  {
    public enum KeyName
    {
      Unknown = 0,
      Space = KeyCode.Space,
      Exclaim = KeyCode.Exclaim,
      DoubleQuote = KeyCode.DoubleQuote,
      Hash = KeyCode.Hash,
      Dollar = KeyCode.Dollar,
      Percent = KeyCode.Percent,
      Ampersand = KeyCode.Ampersand,
      Quote = KeyCode.Quote,
      LeftParen = KeyCode.LeftParen,
      RightParen = KeyCode.RightParen,
      Asterisk = KeyCode.Asterisk,
      Plus = KeyCode.Plus,
      Comma = KeyCode.Comma,
      Minus = KeyCode.Minus,
      Period = KeyCode.Period,
      Slash = KeyCode.Slash,
      Alpha0 = KeyCode.Alpha0,
      Alpha1 = KeyCode.Alpha1,
      Alpha2 = KeyCode.Alpha2,
      Alpha3 = KeyCode.Alpha3,
      Alpha4 = KeyCode.Alpha4,
      Alpha5 = KeyCode.Alpha5,
      Alpha6 = KeyCode.Alpha6,
      Alpha7 = KeyCode.Alpha7,
      Alpha8 = KeyCode.Alpha8,
      Alpha9 = KeyCode.Alpha9,
      Colon = KeyCode.Colon,
      Semicolon = KeyCode.Semicolon,
      Less = KeyCode.Less,
      Equals = KeyCode.Equals,
      Greater = KeyCode.Greater,
      Question = KeyCode.Question,
      At = KeyCode.At,
      LeftBracket = KeyCode.LeftBracket,
      Backslash = KeyCode.Backslash,
      RightBracket = KeyCode.RightBracket,
      Caret = KeyCode.Caret,
      Underscore = KeyCode.Underscore,
      BackQuote = KeyCode.BackQuote,
      A = KeyCode.A,
      B = KeyCode.B,
      C = KeyCode.C,
      D = KeyCode.D,
      E = KeyCode.E,
      F = KeyCode.F,
      G = KeyCode.G,
      H = KeyCode.H,
      I = KeyCode.I,
      J = KeyCode.J,
      K = KeyCode.K,
      L = KeyCode.L,
      M = KeyCode.M,
      N = KeyCode.N,
      O = KeyCode.O,
      P = KeyCode.P,
      Q = KeyCode.Q,
      R = KeyCode.R,
      S = KeyCode.S,
      T = KeyCode.T,
      U = KeyCode.U,
      V = KeyCode.V,
      W = KeyCode.W,
      X = KeyCode.X,
      Y = KeyCode.Y,
      Z = KeyCode.Z,
      LeftCurlyBracket = KeyCode.LeftCurlyBracket,
      Pipe = KeyCode.Pipe,
      RightCurlyBracket = KeyCode.RightCurlyBracket,
      Tilde = KeyCode.Tilde,
      F1 = KeyCode.F1,
      F2 = KeyCode.F2,
      F3 = KeyCode.F3,
      F4 = KeyCode.F4,
      F5 = KeyCode.F5,
      F6 = KeyCode.F6,
      F7 = KeyCode.F7,
      F8 = KeyCode.F8,
      F9 = KeyCode.F9,
      F10 = KeyCode.F10,
      F11 = KeyCode.F11,
      F12 = KeyCode.F12,
      F13 = KeyCode.F13,
      F14 = KeyCode.F14,
      F15 = KeyCode.F15
    }

    public static HotkeyPressedEvent? ShouldFire()
    {
      if (!ModifiersDown())
      {
        return null;
      }

      return (
        from KeyName key in Enum.GetValues(typeof(KeyName))
        where Input.GetKeyDown((KeyCode) key)
        select new HotkeyPressedEvent(key)
      ).FirstOrDefault();
    }

    static bool ModifiersDown() => CtrlDown() && (ShiftDown() || CmdDown());

    static bool CtrlDown() => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

    static bool CmdDown() => Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand);

    static bool ShiftDown() => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

    public static readonly Spec Specification = new Spec();

    public sealed class Spec : EventSpec
    {
      public override EventName Name => EventName.HotkeyPressed;

      public override ServiceRegistryName? ParentRegistry => null;

      public override Description Describe() => new Description("a hotkey is pressed");

      public override ImmutableHashSet<IKey> Bindings() => ImmutableHashSet.Create<IKey>(Key.Hotkey);
    }

    public HotkeyPressedEvent(KeyName hotkey)
    {
      Hotkey = hotkey;
    }

    public EventSpec GetSpec() => Specification;

    public KeyName Hotkey { get; }

    public Scope AddBindings(Scope.Builder builder) => builder.AddValueBinding(Key.Hotkey, Hotkey).Build();
  }
}