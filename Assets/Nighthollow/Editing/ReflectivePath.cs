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
using System.Linq;
using System.Reflection;
using Nighthollow.Data;

#nullable enable

namespace Nighthollow.Editing
{
  public sealed class ReflectivePath
  {
    readonly Database _database;
    readonly ITableId _tableId;
    readonly ImmutableList<ISegment> _path;

    public ReflectivePath(Database database, ITableId tableId)
    {
      _database = database;
      _tableId = tableId;
      _path = ImmutableList<ISegment>.Empty;
    }

    ReflectivePath(Database database, ITableId tableId, ImmutableList<ISegment> segments)
    {
      _database = database;
      _tableId = tableId;
      _path = segments;
    }

    public Database Database => _database;

    public ITableId TableId => _tableId;

    public ReflectivePath EntityId(int entityId) =>
      new ReflectivePath(_database, _tableId, _path.Add(new TableEntitySegement(_tableId!, entityId)));

    public ReflectivePath Property(PropertyInfo propertyInfo) =>
      new ReflectivePath(_database, _tableId, _path.Add(new PropertySegment(propertyInfo)));

    public ReflectivePath ListIndex(Type listType, int listIndex) =>
      new ReflectivePath(_database, _tableId, _path.Add(new ListIndexSegment(listType, listIndex)));

    public IDictionary GetTable() => _tableId.GetInUnchecked(_database.Snapshot());

    public Type GetUnderlyingType() => _path.IsEmpty ? _tableId.GetUnderlyingType() : _path.Last().GetUnderlyingType();

    public object? Read()
    {
      object? result = _database;
      foreach (var segment in _path)
      {
        result = segment.Read(result);
        if (result == null)
        {
          return null;
        }
      }

      return result;
    }

    public void Write(object? newValue)
    {
      _path.First().Write(_path.Skip(1).ToImmutableList(), _database, newValue);
    }

    public void OnEntityUpdated(Action<object?> action)
    {
      ((TableEntitySegement) _path.First()).OnEntityUpdated(_database, _ => action(Read()));
    }

    interface ISegment
    {
      object? Read(object previous);

      object Write(ImmutableList<ISegment> remainingSegments, object parent, object? newValue);

      Type GetUnderlyingType();
    }

    sealed class PropertySegment : ISegment
    {
      readonly PropertyInfo _property;
      readonly bool _nullable;

      public PropertySegment(PropertyInfo property)
      {
        _property = property;
        // ReSharper disable once OperatorIsCanBeUsed
        _nullable = _property.GetType() == typeof(Nullable<>);
      }

      public object? Read(object previous)
      {
        var result = _property.GetValue(previous);
        return _nullable ? _property.GetType().GetProperty("Value")!.GetValue(result) : result;
      }

      public object Write(ImmutableList<ISegment> remainingSegments, object parent, object? newValue)
      {
        var method = parent.GetType().GetMethod("With" + _property.Name, BindingFlags.Instance | BindingFlags.Public)!;
        if (remainingSegments.IsEmpty)
        {
          return method.Invoke(parent, new[] {newValue});
        }
        else
        {
          var parentValue = Read(parent) ?? TypeUtils.InstantiateWithDefaults(GetUnderlyingType());
          return method.Invoke(parent, new[]
          {
            remainingSegments.First().Write(remainingSegments.Skip(1).ToImmutableList(), parentValue, newValue)
          });
        }
      }

      public Type GetUnderlyingType() => _property.PropertyType;
    }

    sealed class TableEntitySegement : ISegment
    {
      readonly ITableId _tableId;
      readonly int _entityId;

      public TableEntitySegement(ITableId tableId, int entityId)
      {
        _entityId = entityId;
        _tableId = tableId;
      }

      public object Read(object previous) => _tableId.GetInUnchecked(((Database) previous).Snapshot())[_entityId];

      public object Write(ImmutableList<ISegment> remainingSegments, object parent, object? newValue) =>
        typeof(TableEntitySegement)
            .GetMethod(nameof(WriteInternal), BindingFlags.NonPublic | BindingFlags.Instance)!
          .MakeGenericMethod(_tableId.GetUnderlyingType())
          .Invoke(this, new[] {remainingSegments, parent, newValue});

      void WriteInternal<T>(ImmutableList<ISegment> remainingSegments, object parent, object newValue)
        where T : class
      {
        T UpdateFn(T? oldValue)
        {
          var segment = remainingSegments.First();
          return (T) segment.Write(remainingSegments.Skip(1).ToImmutableList(), oldValue!, newValue);
        }

        ((Database) parent).Update((TableId<T>) _tableId, _entityId, UpdateFn);
      }

      public Type GetUnderlyingType() => _tableId.GetUnderlyingType();

      public void OnEntityUpdated(Database database, Action<object> action)
      {
        database.GetType()
            .GetMethod(nameof(Database.OnEntityUpdated))!
          .MakeGenericMethod(_tableId.GetUnderlyingType())
          .Invoke(database, new object[] {_tableId, _entityId, action});
      }
    }

    sealed class ListIndexSegment : ISegment
    {
      readonly Type _type;
      readonly int _index;

      public ListIndexSegment(Type type, int index)
      {
        _type = type;
        _index = index;
      }

      public object? Read(object previous) => previous is IList list && list.Count > _index ? list[_index] : null;

      public object Write(ImmutableList<ISegment> remainingSegments, object parent, object? newValue)
      {
        return GetType()
            .GetMethod(nameof(WriteInternal), BindingFlags.Instance | BindingFlags.NonPublic)!
          .MakeGenericMethod(_type)
          .Invoke(this, new[] {remainingSegments, parent, newValue});
      }

      ImmutableList<T> WriteInternal<T>(
        ImmutableList<ISegment> remainingSegments,
        ImmutableList<T> parent,
        object? newValue)
      {
        if (newValue == null)
        {
          return parent.RemoveAt(_index);
        }

        return parent.SetItem(_index,
          remainingSegments.IsEmpty
            ? (T) newValue
            : (T) remainingSegments.First()
              .Write(remainingSegments.Skip(1).ToImmutableList(), parent[_index]!, newValue));
      }

      public Type GetUnderlyingType() => _type;
    }
  }
}