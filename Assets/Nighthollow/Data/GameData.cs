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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using MessagePack;
using UnityEngine;

#nullable enable

namespace Nighthollow.Data
{
  public abstract class Key
  {
    public static readonly Key<CreatureTypeData> CreatureTypes = new Key<CreatureTypeData>("CreatureTypes");

    protected Key(string identifier)
    {
      Identifier = identifier;
    }

    public string Identifier { get; }

#if UNITY_EDITOR
    public string SaveFilePath() => Path.Combine(Application.dataPath, "Data", $"{Identifier}.bin");
#else
    public string SaveFilePath() => Path.Combine(Application.persistentDataPath, $"{Identifier}.bin");
#endif
    
    public abstract Task<GameData> DeserializeAsync(GameData gameData);

    public abstract Task SerializeAsync(GameData gameData);

    public abstract Type GetUnderlyingType();

    public abstract IDictionary LookUpIn(GameData gameData);
  }

  public sealed class Key<T> : Key where T : class
  {
    public Key(string identifier) : base(identifier)
    {
    }

    public override async Task<GameData> DeserializeAsync(GameData gameData)
    {
      using var stream = File.OpenRead(SaveFilePath());
      var result = await MessagePackSerializer.DeserializeAsync<IReadOnlyDictionary<int, T>>(stream);
      return gameData.SetItem(this, result.ToImmutableDictionary());
    }

    public override async Task SerializeAsync(GameData gameData)
    {
      using var stream = File.OpenWrite(SaveFilePath());
      await MessagePackSerializer.SerializeAsync<IReadOnlyDictionary<int, T>>(stream, gameData.Get(this));
    }

    public override Type GetUnderlyingType() => typeof(T);

    public override IDictionary LookUpIn(GameData gameData) => gameData.Get(this);
  }

  public sealed class GameData
  {
    readonly ImmutableDictionary<Key, IDictionary> _tables;

    public GameData()
    {
      _tables = ImmutableDictionary<Key, IDictionary>.Empty;
    }

    GameData(ImmutableDictionary<Key, IDictionary>? value = null)
    {
      _tables = value ?? ImmutableDictionary<Key, IDictionary>.Empty;
    }

    public bool ContainsKey<T>(Key<T> key) where T : class => _tables.ContainsKey(key);

    public ImmutableDictionary<int, T> Get<T>(Key<T> key) where T : class =>
      (ImmutableDictionary<int, T>) _tables[key];

    public GameData SetItem<T>(Key<T> key, ImmutableDictionary<int, T> value) where T : class =>
      new GameData(_tables.SetItem(key, value));

    public IEnumerable<Key> Keys => _tables.Keys;
  }
}
