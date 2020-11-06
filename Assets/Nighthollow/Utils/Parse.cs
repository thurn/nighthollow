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

using System;
using System.Collections.Generic;

namespace Nighthollow.Utils
{
  public static class Parse
  {
    public static bool Boolean(IReadOnlyDictionary<string, string> row, string key)
    {
      if (row.ContainsKey(key) && bool.TryParse(row[key], out var result))
      {
        return result;
      }

      return false;
    }

    public static string? String(IReadOnlyDictionary<string, string> row, string key)
    {
      return row.ContainsKey(key) ? row[key] : null;
    }

    public static string StringRequired(IReadOnlyDictionary<string, string> row, string key)
    {
      if (!row.ContainsKey(key))
      {
        throw new InvalidOperationException($"Key '{key}' not found in {row}");
      }

      return row[key];
    }

    public static int? Int(IReadOnlyDictionary<string, string> row, string key)
    {
      return row.ContainsKey(key) ? (int?) int.Parse(row[key].Replace(",", "")) : null;
    }

    public static int IntRequired(IReadOnlyDictionary<string, string> row, string key)
    {
      if (row.ContainsKey(key) && int.TryParse(row[key].Replace(",", ""), out var result))
      {
        return result;
      }

      throw new InvalidOperationException($"Unable to parse integer for key '{key}' in {row}");
    }
  }
}