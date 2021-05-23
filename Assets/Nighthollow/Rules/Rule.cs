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

#nullable enable

namespace Nighthollow.Rules
{
  /// <summary>
  /// A rule contains code to run in response to an event. When the event corresponding to <see cref="TriggerName"/>
  /// occurs and all of the conditions in <see cref="Conditions"/> pass, the rule returns the effects provided in
  /// <see cref="Effects"/>. </summary>
  [MessagePackObject]
  public sealed class Rule
  {
    public Rule(
      TriggerName triggerName,
      ImmutableList<ICondition> conditions,
      ImmutableList<IEffect> effects,
      bool oneTime,
      bool disabled)
    {
      TriggerName = triggerName;
      Conditions = conditions;
      Effects = effects;
      OneTime = oneTime;
      Disabled = disabled;
    }

    /// <summary>Identifies the <see cref="ITrigger{THandler,TResult}"/> which causes this rule to run.</summary>
    [Key(0)] public TriggerName TriggerName { get; }

    /// <summary>Conditional execution for the rule -- rules only rule if ALL conditions are satisfied.</summary>
    [Key(1)] public ImmutableList<ICondition> Conditions { get; }

    /// <summary>Effects which occur when the rule runs.</summary>
    [Key(2)] public ImmutableList<IEffect> Effects { get; }

    /// <summary>If true, the rule will disable itself after it runs.</summary>
    [Key(3)] public bool OneTime { get; }

    /// <summary>If true, the rule will not run.</summary>
    [Key(4)] public bool Disabled { get; }
  }
}