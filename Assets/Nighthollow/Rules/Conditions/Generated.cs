// Generated Code - Do not Edit!

using System.Collections.Immutable;
using Nighthollow.Delegates;
using Nighthollow.Services;
using Nighthollow.Stats;
using Nighthollow.State;
using Nighthollow.Rules;
using Nighthollow.Rules.Effects;
using Nighthollow.Rules.Events;
using Nighthollow.World.Data;

#nullable enable

namespace Nighthollow.Rules.Conditions
{

  public sealed partial class HotkeyEqualsCondition
  {
    public HotkeyEqualsCondition WithHotkey(HotkeyPressedEvent.KeyName hotkey) =>
      Equals(hotkey, Hotkey)
        ? this
        : new HotkeyEqualsCondition(
          hotkey);

  }
}
