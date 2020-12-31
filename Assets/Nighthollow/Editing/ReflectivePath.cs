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
    readonly bool _isReadOnly;

    public ReflectivePath(Database database, ITableId tableId)
    {
      _database = database;
      _tableId = tableId;
      _path = ImmutableList<ISegment>.Empty;
    }

    ReflectivePath(Database database, ITableId tableId, ImmutableList<ISegment> segments, bool isReadOnly = false)
    {
      _database = database;
      _tableId = tableId;
      _path = segments;
      _isReadOnly = isReadOnly;
    }

    public bool IsReadOnly => _isReadOnly;

    public ReflectivePath EntityId(int entityId) =>
      new ReflectivePath(_database, _tableId, _path.Add(new TableEntitySegement(_tableId!, entityId)));

    public ReflectivePath Property(PropertyInfo propertyInfo) =>
      new ReflectivePath(_database, _tableId, _path.Add(new PropertySegment(propertyInfo)));

    public ReflectivePath Constant(object constant) =>
      new ReflectivePath(_database, _tableId, _path.Add(new ConstantSegment(constant)), isReadOnly: true);

    public IDictionary GetTable() => _tableId.GetInUnchecked(_database.Snapshot());

    public Type GetUnderlyingType() => _path.IsEmpty ? _tableId.GetUnderlyingType() : _path.Last().GetUnderlyingType();

    public object? Read()
    {
      object? result = _database;
      foreach (var segment in _path)
      {
        result = segment.Read(result!);
      }

      return result;
    }

    public void Write(object? newValue)
    {
      _path.First().Write(_path.Skip(1).ToImmutableList(), _database, newValue);
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

      public PropertySegment(PropertyInfo property)
      {
        _property = property;
      }

      public object? Read(object previous) => _property.GetValue(previous);

      public object Write(ImmutableList<ISegment> remainingSegments, object parent, object? newValue)
      {
        var method = parent.GetType().GetMethod("With" + _property.Name, BindingFlags.Instance | BindingFlags.Public)!;
        if (remainingSegments.IsEmpty)
        {
          return method.Invoke(parent, new[] {newValue});
        }
        else
        {
          return method.Invoke(parent, new[]
          {
            remainingSegments.First().Write(remainingSegments.Skip(1).ToImmutableList(), Read(parent)!, newValue)
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
            .GetMethod(nameof(WriteInternal), BindingFlags.Public | BindingFlags.Instance)!
          .MakeGenericMethod(_tableId.GetUnderlyingType())
          .Invoke(this, new[] {remainingSegments, parent, newValue});

      public void WriteInternal<T>(ImmutableList<ISegment> remainingSegments, object parent, object newValue)
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
    }

    sealed class ConstantSegment : ISegment
    {
      readonly object _value;

      public ConstantSegment(object value)
      {
        _value = value;
      }

      public object Read(object previous) => _value;

      public object Write(ImmutableList<ISegment> remainingSegments, object parent, object? newValue) =>
        throw new InvalidOperationException($"Cannot write to constant {_value}");

      public Type GetUnderlyingType() => _value.GetType();
    }
  }
}