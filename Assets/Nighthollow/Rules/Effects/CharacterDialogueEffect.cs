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

using MessagePack;

#nullable enable

namespace Nighthollow.Rules.Effects
{
  public enum CharacterName
  {
    Unknown = 0,
    Ocerak = 1,
    You = 2
  }

  [MessagePackObject]
  public sealed class CharacterDialogueEffect : IEffect
  {
    public CharacterDialogueEffect(CharacterName characterName, string text, int? onContinueRuleId)
    {
      CharacterName = characterName;
      Text = text;
      OnContinueRuleId = onContinueRuleId;
    }

    public Description Describe => new Description(
      "display the dialogue",
      nameof(Text),
      "from character",
      nameof(CharacterName),
      "and then invoke rule",
      nameof(OnContinueRuleId));

    [Key(0)] public CharacterName CharacterName { get; }
    [Key(1)] public string Text { get; }
    /*[RuleId]*/ [Key(2)] public int? OnContinueRuleId { get; }

    public Injector AddDependencies(EffectInjector injector) => injector
      .AddDependency(Key.TriggerService)
      .AddDependency(Key.ScreenController);

    public void Execute(EffectInjector injector)
    {
      // Action? action = null;
      // if (OnContinueTriggerId.HasValue)
      // {
      //   action = () =>
      //   {
      //     injector.Get(Key.TriggerService).InvokeTriggerId(trigger.Registry, OnContinueTriggerId.Value);
      //   };
      // }
      //
      // injector.Get(Key.ScreenController).Get(ScreenController.CharacterDialogue).Show(
      //   new CharacterDialogue.Args(CharacterName, Text, action), animate: true);
    }
  }
}