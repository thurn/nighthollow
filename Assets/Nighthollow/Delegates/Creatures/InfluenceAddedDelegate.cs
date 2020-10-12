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

#nullable enable

using Nighthollow.Delegates.Core;
using Nighthollow.Generated;
using Nighthollow.Stats;

namespace Nighthollow.Delegates.Creatures
{
  public sealed class InfluenceAddedDelegate : CreatureDelegate
  {
    public override void OnActivate(CreatureContext c, Results<CreatureEffect> results)
    {
      // results.Add(new ApplyModifierToOwner(
      //   Operator.Add,
      //   Stat.Influence,
      //   new WhileAliveModifier<TaggedStatValue<School, IntValue>>(
      //     new StaticModifier<TaggedStatValue<School, IntValue>>(
      //       new TaggedStatValue<School, IntValue>(
      //         c.Self.Data.School,
      //         new IntValue(1))), c.Self)));
    }
  }
}