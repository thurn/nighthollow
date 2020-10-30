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
using Nighthollow.Data;
using Nighthollow.Generated;
using SimpleJSON;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Nighthollow.Editor.Data
{
  public static class SheetsCodeGenerator
  {
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
      var request = SpreadsheetHelper.SpreadsheetRequest(new List<string> {"Enums", "Stats", "Delegates"});
      Debug.Log("Running Code Generation...");
      _request = request.SendWebRequest();
      EditorApplication.update += EditorUpdate;
    }

    static void EditorUpdate()
    {
      if (!_request.isDone)
      {
        return;
      }

      try
      {
        var request = _request.webRequest;
        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.DataProcessingError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
          Debug.LogError(request.error);
          EditorApplication.update -= EditorUpdate;
          return;
        }

        Debug.Log("Got Database Response...");
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
          else if (SheetName(range).Equals("Delegates"))
          {
            GenerateDelegates(range["values"]);
          }
        }
      }
      finally
      {
        EditorApplication.update -= EditorUpdate;
      }
    }

    static string SheetName(JSONNode range) => range["range"].Value.Split('!').First();

    static void GenerateDelegates(JSONNode rows)
    {
      var builder = CreateHeader("\nusing System.Collections.Generic;\n" +
                                 "using Nighthollow.Delegates.Core;\n" +
                                 "using Nighthollow.Delegates.Creatures;\n" +
                                 "using Nighthollow.Delegates.Skills;\n");

      var typeRow = rows[1];
      var typeNames = new Dictionary<int, string>();
      var enums = new Dictionary<string, List<(string, string)>>();

      for (var i = 0; i < typeRow.Count; i += 2)
      {
        typeNames[i] = typeRow[i + 1].Value;
        enums[typeNames[i]] = new List<(string, string)>();
      }

      for (var i = 2; i < rows.Count; ++i)
      {
        var row = rows[i];
        for (var j = 0; j < row.Count; j += 2)
        {
          enums[typeNames[j]].Add((row[j].Value, row[j + 1].Value));
        }
      }

      foreach (var pair in enums)
      {
        builder.Append($"  public enum {pair.Key}DelegateId\n");
        builder.Append("  {\n");
        builder.Append("    Unknown = 0,\n");
        foreach (var (valueName, integer) in pair.Value)
        {
          builder.Append($"    {valueName} = {integer},\n");
        }

        builder.Append("  }\n\n");

        builder.Append($"  public static class {pair.Key}DelegateMap\n");
        builder.Append("  {\n");
        builder.Append(
          $"    static readonly Dictionary<{pair.Key}DelegateId, I{pair.Key}Delegate> Delegates = new\n");
        builder.Append($"        Dictionary<{pair.Key}DelegateId, I{pair.Key}Delegate>\n");
        builder.Append("    {\n");
        foreach (var (valueName, _) in pair.Value)
        {
          builder.Append($"      {{{pair.Key}DelegateId.{valueName}, new {valueName}()}},\n");
        }

        builder.Append("    };\n\n");
        builder.Append($"    public static I{pair.Key}Delegate Get({pair.Key}DelegateId id) => Delegates[id];\n");
        builder.Append("  }\n\n");
      }

      builder.Append("}\n");
      File.WriteAllText("Assets/Nighthollow/Generated/Delegates.cs", builder.ToString());
    }

    static void GenerateStats(JSONNode rows)
    {
      var input = SpreadsheetHelper.AsHeaderIdentifiedRows(rows);

      var builder = CreateHeader("\nusing Nighthollow.Stats;\nusing System;\n");
      builder.Append("  public static class Stat\n");
      builder.Append("  {\n");

      foreach (var stat in input)
      {
        var statType = (StatType) int.Parse(stat["Type"]) switch
        {
          StatType.Int => "IntStat",
          StatType.Percentage => "PercentageStat",
          StatType.Duration => "DurationStat",
          StatType.IntRange => "IntRangeStat",
          StatType.Bool => "BoolStat",
          StatType.SchoolInts => "SchoolIntsStat",
          StatType.DamageTypeInts => "DamageTypeIntsStat",
          StatType.DamageTypeIntRanges => "DamageTypeIntRangesStat",
          _ => throw new ArgumentOutOfRangeException()
        };

        builder.Append($"    public static readonly {statType} {stat["Name"]} = new {statType}({stat["Stat ID"]});\n");
      }

      builder.Append("\n    public static IStat GetStat(int statId)\n");
      builder.Append("    {\n");
      builder.Append("      switch (statId)\n");
      builder.Append("      {\n");
      foreach (var stat in input)
      {
        builder.Append($"        case {stat["Stat ID"]}: return {stat["Name"]};\n");
      }
      builder.Append($"        default: throw new ArgumentOutOfRangeException(statId.ToString());\n");
      builder.Append("      }\n");
      builder.Append("    }\n");

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
  }
}