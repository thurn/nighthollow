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
using MessagePack;
using Nighthollow.Interface;

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
  public sealed partial class CharacterDialogueEffect : RuleEffect
  {
    public override Description Describe() => new Description(
      "display the dialogue",
      nameof(Text),
      "from character",
      nameof(CharacterName),
      "and then invoke rule",
      nameof(OnContinueRuleId));

    public CharacterDialogueEffect(CharacterName characterName, string text, int? onContinueRuleId)
    {
      CharacterName = characterName;
      Text = text;
      OnContinueRuleId = onContinueRuleId;
    }

    [Key(0)] public CharacterName CharacterName { get; }
    [Key(1)] public string Text { get; }
    [RuleId] [Key(2)] public int? OnContinueRuleId { get; }

    public override ImmutableHashSet<IKey> GetDependencies() => ImmutableHashSet.Create<IKey>(
      Key.RulesEngine,
      Key.ScreenController
    );

    public override void Execute(IEffectScope scope, RuleOutput? output)
    {
      Action? action = null;
      if (OnContinueRuleId.HasValue)
      {
        action = () =>
        {
          scope.Get(Key.RulesEngine).InvokeRuleId(OnContinueRuleId.Value, scope.ClearDependencies());
        };
      }

      scope.Get(Key.ScreenController).Get(ScreenController.CharacterDialogue).Show(
        new CharacterDialogue.Args(CharacterName, Text, action), animate: true);
    }
  }
}
