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
using MessagePack;
using Nighthollow.Rules.Events;

#nullable enable

namespace Nighthollow.Rules.Conditions
{
  [MessagePackObject]
  public sealed partial class HotkeyEqualsCondition : RuleCondition
  {
    public override Description Describe() => new Description("that hotkey is", nameof(Hotkey));

    public HotkeyEqualsCondition(HotkeyPressedEvent.KeyName hotkey)
    {
      Hotkey = hotkey;
    }

    [Key(0)] public HotkeyPressedEvent.KeyName Hotkey { get; }

    public override ImmutableHashSet<IKey> GetDependencies() => ImmutableHashSet.Create<IKey>(Key.Hotkey);

    public override bool Satisfied(IScope scope) => scope.Get(Key.Hotkey) == Hotkey;
  }
}