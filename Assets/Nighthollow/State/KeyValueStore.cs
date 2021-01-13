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
using JetBrains.Annotations;

#nullable enable

namespace Nighthollow.State
{
  public interface IKeyValueStoreOwner
  {
    public void ExecuteMutatation(IMutation mutation);
  }

  public sealed class KeyValueStore
  {
    readonly ImmutableDictionary<int, object> _values;

    public KeyValueStore() : this(ImmutableDictionary<int, object>.Empty)
    {
    }

    KeyValueStore(ImmutableDictionary<int, object> values)
    {
      _values = values;
    }

    public bool TryGet<T>(Key<T> key, out T result) where T : notnull
    {
      if (_values.ContainsKey(key.Id))
      {
        result = (T) _values[key.Id];
        return true;
      }
      else
      {
        result = default!;
        return false;
      }
    }

    public T Get<T>(Key<T> key, T withDefault) where T : notnull =>
      _values.ContainsKey(key.Id) ? (T) _values[key.Id] : withDefault;

    public T Get<T>(KeyWithDefault<T> key) where T : notnull =>
      _values.ContainsKey(key.Id) ? (T) _values[key.Id] : key.DefaultValue;

    [MustUseReturnValue]
    public KeyValueStore Mutate<T>(Mutation<T> mutation) where T : notnull =>
      new KeyValueStore(_values.SetItem(mutation.Key.Id, mutation.Apply(Get(mutation.Key, mutation.NotFoundValue()))));

    [MustUseReturnValue]
    public KeyValueStore Set<T>(Key<T> key, T value) where T : notnull =>
      new KeyValueStore(_values.SetItem(key.Id, value));

    [MustUseReturnValue]
    public KeyValueStore Increment(Key<int> key) => Mutate(new IncrementIntegerMutation(key));

    [MustUseReturnValue]
    public KeyValueStore Append<T>(Key<ImmutableList<T>> key, T value) where T : notnull =>
      Mutate(new AppendToListMutation<T>(key, value));

    public bool Has<T>(Key<T> key) where T : notnull => _values.ContainsKey(key.Id);
  }
}
