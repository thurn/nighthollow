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

namespace Nighthollow.Utils
{
  public static class CollectionUtils
  {
    public static TValue GetValueOrDefault<TKey, TValue>(
      this IReadOnlyDictionary<TKey, TValue> dictionary,
      TKey key,
      TValue defaultValue) =>
      dictionary.TryGetValue(key, out var value) ? value : defaultValue;

    public static TValue GetOrCreateDefault<TKey, TValue>(
      this IDictionary<TKey, TValue> dictionary,
      TKey key,
      TValue defaultValue)
    {
      if (dictionary.TryGetValue(key, out var value))
      {
        return value;
      }

      dictionary[key] = defaultValue;
      return defaultValue;
    }

    public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary)
    {
      return dictionary.ToDictionary(p => p.Key, p => p.Value);
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : struct
    {
      return source.Where(t => t != null).Select(t => t.GetValueOrDefault());
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : class
    {
      return source.Where(t => t != null).Select(t => t!);
    }

    /// <summary>Returns the infinite sequence (1, -1, -2, -2, ....)</summary>
    public static IEnumerable<int> AlternatingIntegers()
    {
      var i = 0;
      while (true)
      {
        i++;
        yield return i;
        yield return -i;
      }

      // ReSharper disable once IteratorNeverReturns
    }

    public static IEnumerable<T> AllNonDefaultEnumValues<T>(Type type) where T : struct, Enum
    {
      return Enum.GetValues(type).Cast<T>().Where(v => !v.Equals(obj: default));
    }
  }
}
