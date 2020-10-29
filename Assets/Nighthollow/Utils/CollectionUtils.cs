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

#nullable enable

using System.Collections.Generic;
using System.Linq;

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

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : struct =>
      source.Where(t => t != null).Select(t => t.GetValueOrDefault());
  }
}