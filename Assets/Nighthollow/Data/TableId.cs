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
using System.Collections;
using System.Collections.Immutable;

#nullable enable

namespace Nighthollow.Data
{
  public interface ITableId
  {
    Type GetUnderlyingType();

    IDictionary GetInUnchecked(GameData gameData);
  }

  public abstract class TableId<T> : ITableId
  {
    public abstract ImmutableDictionary<int, T> GetIn(GameData gameData);

    public abstract GameData Write(GameData gameData, ImmutableDictionary<int, T> newValue);

    public Type GetUnderlyingType() => typeof(T);

    public IDictionary GetInUnchecked(GameData gameData) => GetIn(gameData);
  }

  public static class TableId
  {
    public static readonly TableId<CreatureTypeData> CreatureTypes = new CreatureTypesTableId();

    public sealed class CreatureTypesTableId : TableId<CreatureTypeData>
    {
      public override ImmutableDictionary<int, CreatureTypeData> GetIn(GameData gameData) =>
        gameData.CreatureTypes;

      public override GameData Write(GameData gameData, ImmutableDictionary<int, CreatureTypeData> newValue) =>
        gameData.WithCreatureTypes(newValue);
    }
  }
}
