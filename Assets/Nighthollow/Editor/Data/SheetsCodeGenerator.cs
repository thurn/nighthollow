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
using System.IO;
using System.Linq;
using System.Text;
using Nighthollow.Generated;
using SimpleJSON;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Nighthollow.Editor.Data
{
  public static class SheetsCodeGenerator
  {
    const string SheetId = "1n1tpIsouYgoPiiY5gEJ5lHzpBFpjX1poHMNseUpojFE";
    const string ApiKey = "AIzaSyAh7HQu6r9J55bMAqW60IvX_oZ5RIIwO7g";
    const string LicenseHeader = @"// Copyright © 2020-present Derek Thurn

// Licensed under the Apache License, Version 2.0 (the ""License"");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//    https://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an ""AS IS"" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.";

    static UnityWebRequestAsyncOperation _request;

    [MenuItem("Tools/Run Code Generation %#g")]
    public static void RunCodeGeneration()
    {
      var request = UnityWebRequest.Get(
        $"https://sheets.googleapis.com/v4/spreadsheets/{SheetId}/values:batchGet" +
        $"?ranges=Enums&ranges=Stats&key={ApiKey}");

      _request = request.SendWebRequest();
      EditorApplication.update += EditorUpdate;
    }

    static void EditorUpdate()
    {
      if (!_request.isDone)
      {
        return;
      }

      var request = _request.webRequest;
      if (request.isNetworkError || request.isHttpError)
      {
        Debug.LogError(request.error);
        EditorApplication.update -= EditorUpdate;
        return;
      }

      var parsed = JSON.Parse(request.downloadHandler.text);

      foreach (var range in parsed["valueRanges"].Children)
      {
        if (SheetName(range).Equals("Enums"))
        {
          GenerateEnums(range["values"]);
        }
        else if (SheetName(range).Equals("Stats"))
        {
          GenerateStats(range["values"]);
        }
      }

      EditorApplication.update -= EditorUpdate;
    }

    static string SheetName(JSONNode range) => range["range"].Value.Split('!').First();

    static void GenerateStats(JSONNode rows)
    {
      var input = AsHeaderIdentifiedRows(rows);
      var builder = CreateHeader("\nusing Nighthollow.Stats;\n");
      builder.Append("  public static class Stat\n");
      builder.Append("  {\n");

      foreach (var stat in input)
      {
        string idType;
        switch ((StatType) int.Parse(stat["Type ID"]))
        {
          case StatType.Int:
            idType = "IntStatId";
            break;
          case StatType.Bool:
            idType = "BoolStatId";
            break;
          case StatType.SchoolInts:
            idType = "TaggedIntsStatId<School>";
            break;
          case StatType.DamageTypeInts:
            idType = "TaggedIntsStatId<DamageType>";
            break;
          case StatType.Unknown:
          default:
            throw new ArgumentOutOfRangeException();
        }

        builder.Append($"    public static readonly {idType} {stat["Name"]} = new {idType}({stat["ID"]});\n");
      }

      builder.Append("  }\n");
      builder.Append("}\n");
      File.WriteAllText("Assets/Nighthollow/Generated/Stat.cs", builder.ToString());
    }

    static void GenerateEnums(JSONNode rows)
    {
      var firstRow = rows[0];
      var enumNames = new Dictionary<int, string>();
      var enums = new Dictionary<int, List<(string, string)>>();

      for (var i = 0; i < firstRow.Count; i += 2)
      {
        enumNames[i] = firstRow[i].Value;
        enums[i] = new List<(string, string)>();
      }

      for (var i = 1; i < rows.Count; ++i)
      {
        var row = rows[i];
        for (var j = 0; j < row.Count; j += 2)
        {
          if (!string.IsNullOrWhiteSpace(row[j].Value) && !string.IsNullOrWhiteSpace(row[j + 1]))
          {
            enums[j].Add((row[j].Value, row[j + 1].Value));
          }
        }
      }

      var builder = CreateHeader();

      foreach (var index in enums)
      {
        builder.Append($"  public enum {enumNames[index.Key]}\n");
        builder.Append("  {\n");
        builder.Append($"    Unknown = 0,\n");
        foreach (var (name, integer) in index.Value)
        {
          builder.Append($"    {name} = {integer},\n");
        }

        builder.Append("  }\n\n");
      }

      builder.Append("}");
      File.WriteAllText("Assets/Nighthollow/Generated/Enums.cs", builder.ToString());
    }

    static StringBuilder CreateHeader(string usingDirective = "")
    {
      var builder = new StringBuilder();
      builder.Append(LicenseHeader);
      builder.Append($"\n{usingDirective}");
      builder.Append("\n// Generated Code - Do Not Modify!");
      builder.Append("\nnamespace Nighthollow.Generated\n");
      builder.Append("{\n");
      return builder;
    }

    /// <summary>
    /// Converts a sheets JSON response into a list of dictionaries where each item is a row and the keys are provided
    /// by the first row of the response.
    /// </summary>
    static List<Dictionary<string, string>> AsHeaderIdentifiedRows(JSONNode rows)
    {
      var firstRow = rows[0];
      var keys = new Dictionary<int, string>();
      for (var i = 0; i < firstRow.Count; ++i)
      {
        keys[i] = firstRow[i].Value;
      }

      var result = new List<Dictionary<string, string>>();
      for (var i = 1; i < rows.Count; ++i)
      {
        var row = rows[i];
        var dictionary = new Dictionary<string, string>();
        for (var j = 0; j < row.Count; ++j)
        {
          dictionary[keys[j]] = row[j].Value;
        }
        result.Add(dictionary);
      }

      return result;
    }
  }
}
