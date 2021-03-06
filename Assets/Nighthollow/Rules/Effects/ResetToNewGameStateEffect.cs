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
using UnityEngine;

#nullable enable

namespace Nighthollow.Rules.Effects
{
  [MessagePackObject]
  public sealed class ResetToNewGameStateEffect : RuleEffect
  {
    public override Description Describe() => new Description("reset to the 'new game' database state");

    public override ImmutableHashSet<IKey> GetDependencies() => ImmutableHashSet.Create<IKey>(Key.Database);

    public override void Execute(IEffectScope scope, RuleOutput? output)
    {
      var database = scope.Get(Key.Database);
      var gameData = database.Snapshot();
      var school = gameData.UserData.PrimarySchool;
      database.OverwriteSingleton(TableId.UserData, new UserData(school));
      database.ClearTable(TableId.UserModifiers);
      database.ClearTable(TableId.Collection);
      database.ClearTable(TableId.Deck);
      foreach (var card in gameData.ItemLists.Values
        .First(list => list.Tag == StaticItemListData.IdentifierTag.StartingDeck).Creatures)
      {
        // Transform all cards in deck to match starting school
        database.Insert(TableId.Deck,
          card.WithSchool(school)
            .WithInfluenceCost(new ImmutableDictionaryValue<School, int>(
              card.InfluenceCost.Dictionary.ToImmutableDictionary(pair => school, pair => pair.Value))));
      }

      foreach (var pair in gameData.Globals)
      {
        database.Update(TableId.Globals, pair.Key, g => g.WithValue(0));
      }

      Debug.Log($"Reset to the New Game database state using school {school}");
    }
  }
}