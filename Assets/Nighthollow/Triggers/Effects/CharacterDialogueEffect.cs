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
using MessagePack;
using Nighthollow.Stats;
using Nighthollow.Triggers.Events;

#nullable enable

namespace Nighthollow.Triggers.Effects
{
  public enum CharacterName
  {
    Unknown = 0,
    Ocerak = 1
  }

  [MessagePackObject]
  public sealed partial class CharacterDialogueEffect : IEffect<TriggerEvent>
  {
    public static Description Describe => new Description(
      "display the dialogue",
      nameof(Text),
      "from character",
      nameof(CharacterName),
      "with timeout",
      nameof(Timeout));

    public CharacterDialogueEffect(CharacterName characterName, string text, DurationValue? timeout)
    {
      CharacterName = characterName;
      Text = text;
      Timeout = timeout;
    }

    [Key(0)] public CharacterName CharacterName { get; }
    [Key(1)] public string Text { get; }

    /// <summary>How long to display the message for. If null, a close button will be shown instead.</summary>
    [Key(2)] public DurationValue? Timeout { get; }

    public void Execute(TriggerEvent trigger)
    {
      var portraitName = CharacterName switch
      {
        CharacterName.Ocerak => "ocerak",
        _ => throw new ArgumentOutOfRangeException()
      };
      // trigger.Registry.ScreenController.ShowDialog(portraitName, Text);
    }
  }
}