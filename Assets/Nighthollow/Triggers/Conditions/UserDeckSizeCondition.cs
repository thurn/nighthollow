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

namespace Nighthollow.Triggers.Conditions
{
  /// <summary>
  /// Integer condition based on the number of cards in the user's deck.
  /// </summary>
  [MessagePackObject]
  public sealed class UserDeckSizeCondition : IntegerCondition<TriggerEvent>
  {
    public UserDeckSizeCondition(int target, IntegerOperator op) : base(target, op)
    {
    }

    public override ImmutableHashSet<IKey> Dependencies => ImmutableHashSet.Create<IKey>(
      Key.GameData
    );

    public override int GetSource(IScope scope) => scope.Get(Key.GameData).Deck.Count;

    protected override IntegerCondition<TriggerEvent> Clone(int target, IntegerOperator op) =>
      new UserDeckSizeCondition(target, op);

    public static Description Describe => new Description("the user's deck size", nameof(Operator), nameof(Target));
  }
}