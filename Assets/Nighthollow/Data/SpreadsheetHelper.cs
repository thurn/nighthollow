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

using System.Collections.Generic;
using SimpleJSON;

namespace Nighthollow.Data
{
  public static class SpreadsheetHelper
  {
    /// <summary>
    /// Converts a sheets JSON response into a list of dictionaries where each item is a row and the keys are provided
    /// by the first row of the response.
    /// </summary>
    public static List<Dictionary<string, string>> AsHeaderIdentifiedRows(JSONNode rows)
    {
      var firstRow = rows[0];
      var columnNames = new Dictionary<int, string>();
      for (var i = 0; i < firstRow.Count; ++i)
      {
        var header = firstRow[i].Value;
        if (header.Equals("ID"))
        {
          columnNames[i] = columnNames[i - 1];
          columnNames.Remove(i - 1);
        }
        else
        {
          columnNames[i] = header;
        }
      }

      var result = new List<Dictionary<string, string>>();
      for (var i = 1; i < rows.Count; ++i)
      {
        var row = rows[i];
        var dictionary = new Dictionary<string, string>();
        for (var j = 0; j < row.Count; ++j)
        {
          if (!columnNames.ContainsKey(j))
          {
            continue;
          }

          dictionary[columnNames[j]] = row[j].Value;
        }

        result.Add(dictionary);
      }

      return result;
    }
  }
}