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
using Nighthollow.Interface;
using Nighthollow.Triggers.Events;

#nullable enable

namespace Nighthollow.Triggers.Effects
{
  public enum CharacterName
  {
    Unknown = 0,
    Ocerak = 1,
    You = 2
  }

  [MessagePackObject]
  public sealed partial class CharacterDialogueEffect : IEffect<TriggerEvent>
  {
    public static Description Describe => new Description(
      "display the dialogue",
      nameof(Text),
      "from character",
      nameof(CharacterName),
      "and then invoke trigger",
      nameof(OnContinueTriggerId));

    public CharacterDialogueEffect(CharacterName characterName, string text, int? onContinueTriggerId)
    {
      CharacterName = characterName;
      Text = text;
      OnContinueTriggerId = onContinueTriggerId;
    }

    [Key(0)] public CharacterName CharacterName { get; }
    [Key(1)] public string Text { get; }
    [TriggerId] [Key(2)] public int? OnContinueTriggerId { get; }

    public void Execute(TriggerEvent trigger)
    {
      Action? action = null;
      if (OnContinueTriggerId.HasValue)
      {
        action = () =>
        {
          trigger.Registry.TriggerService.InvokeTriggerId(new TriggerInvokedEvent(trigger.Registry),
            OnContinueTriggerId.Value);
        };
      }

      trigger.Registry.ScreenController.Get(ScreenController.CharacterDialogue).Show(
        new CharacterDialogue.Args(CharacterName, Text, action), animate: true);
    }
  }
}