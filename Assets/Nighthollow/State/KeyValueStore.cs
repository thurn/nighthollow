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
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace Nighthollow.State
{
  public sealed class KeyValueStore
  {
    readonly Dictionary<int, object> _values;

    public KeyValueStore() : this(new Dictionary<int, object>())
    {
    }

    KeyValueStore(Dictionary<int, object> values)
    {
      _values = values;
    }

    public T Get<T>(Key<T> key)
    {
      return _values.ContainsKey(key.Id) ? (T) _values[key.Id] : key.DefaultValue;
    }

    public KeyValueStore Set<T>(Key<T> key, T value)
    {
      _values[key.Id] = value ?? throw new NullReferenceException($"Null value provided for Set() {key}");
      return this;
    }

    public bool Has<T>(Key<T> key)
    {
      return _values.ContainsKey(key.Id);
    }

    public KeyValueStore Mutate<T>(Mutation<T> mutation)
    {
      _values[mutation.Key.Id] = mutation.Apply(Get(mutation.Key)) ??
                                 throw new NullReferenceException($"Null Apply() value returned for {mutation}");
      return this;
    }

    public KeyValueStore Increment(Key<int> key)
    {
      return Set(key, Get(key) + 1);
    }

    public KeyValueStore Append<T>(Key<IReadOnlyList<T>> key, T value)
    {
      return Set(key, Get(key).Append(value).ToList());
    }

    public KeyValueStore Copy()
    {
      return new KeyValueStore(_values.ToDictionary(pair => pair.Key, pair => pair.Value));
    }

    public void OverwriteWithValues(KeyValueStore? other)
    {
      if (other != null)
        foreach (var pair in other._values)
          _values[pair.Key] = pair.Value;
    }
  }
}
