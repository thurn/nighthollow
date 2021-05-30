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
using System.Linq;
using MessagePack;
using Nighthollow.Data;
using Nighthollow.Editing.Scenarios;
using UnityEngine;

#nullable enable

namespace Nighthollow.Rules.Effects
{
  [MessagePackObject]
  public sealed class ClearScenariosEffect : RuleEffect
  {
    public override Description Describe() => new Description("clear repeating scenarios");

    public override ImmutableHashSet<IKey> GetDependencies() => ImmutableHashSet.Create<IKey>(Key.Database);

    public override void Execute(IEffectScope scope, RuleOutput? output)
    {
      var database = scope.Get(Key.Database);

      foreach (var pair in database.Snapshot().Scenarios.Where(pair => pair.Value?.Repeating == true))
      {
        database.Update(TableId.Scenarios, pair.Key, t => t.WithRepeating(false));
      }

      ScenarioData.ClearPreference();

      Debug.Log("Scenarios Cleared");
    }
  }
}