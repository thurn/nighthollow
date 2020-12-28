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

#nullable enable

namespace Nighthollow.Data
{
  public interface IQuery<T>
  {
    QueryKey QueryKey { get; }

    T Read(GameData parent);

    GameData Write(GameData parent, T child);
  }

  public interface IQueryListener
  {
    QueryKey QueryKey { get; }

    void Invoke(GameData gameData);
  }

  public sealed class QueryListener<T> : IQueryListener
  {
    readonly IQuery<T> _query;
    readonly Action<T> _listener;

    public QueryListener(IQuery<T> query, Action<T> listener)
    {
      _query = query;
      _listener = listener;
    }

    public QueryKey QueryKey => _query.QueryKey;

    public void Invoke(GameData gameData)
    {
      _listener(_query.Read(gameData));
    }
  }

  public interface IQueryWriter
  {
    QueryKey QueryKey { get; }

    GameData Write(GameData gameData);
  }

  public sealed class QueryWriter<T> : IQueryWriter
  {
    readonly IQuery<T> _query;
    readonly T _newValue;

    public QueryWriter(IQuery<T> query, T newValue)
    {
      _query = query;
      _newValue = newValue;
    }

    public QueryKey QueryKey => _query.QueryKey;

    public GameData Write(GameData gameData) => _query.Write(gameData, _newValue);
  }

  public enum QueryType
  {
    CreatureTypeData
  }

  public readonly struct QueryKey
  {
    public QueryKey(QueryType queryType, int entityId)
    {
      QueryType = queryType;
      EntityId = entityId;
    }

    public readonly QueryType QueryType;

    public readonly int EntityId;

    public bool Equals(QueryKey other) => QueryType == other.QueryType && EntityId == other.EntityId;

    public override bool Equals(object? obj) => obj is QueryKey other && Equals(other);

    public override int GetHashCode()
    {
      unchecked
      {
        return ((int) QueryType * 397) ^ EntityId;
      }
    }

    public static bool operator ==(QueryKey left, QueryKey right) => left.Equals(right);

    public static bool operator !=(QueryKey left, QueryKey right) => !left.Equals(right);
  }

  public abstract class EntityIdQuery<T> : IQuery<T>
  {
    protected EntityIdQuery(int entityId, QueryType queryType)
    {
      EntityId = entityId;
      QueryType = queryType;
    }

    protected int EntityId { get; }

    protected QueryType QueryType { get; }

    public QueryKey QueryKey => new QueryKey(QueryType, EntityId);

    public abstract T Read(GameData parent);

    public abstract GameData Write(GameData parent, T child);
  }

  public class CreatureTypeQuery : EntityIdQuery<CreatureTypeData>
  {
    public CreatureTypeQuery(int creatureId) : base(creatureId, QueryType.CreatureTypeData)
    {
    }

    public override CreatureTypeData Read(GameData parent) => parent.CreatureTypes[EntityId];

    public override GameData Write(GameData parent, CreatureTypeData child) => new GameData(
      parent.CreatureTypes.ToImmutableDictionary().Add(EntityId, child),
      parent.AffixTypes,
      parent.SkillTypes);
  }
}
