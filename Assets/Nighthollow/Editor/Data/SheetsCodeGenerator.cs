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
using System.IO;
using System.Text;
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
// limitations under the License.

// Generated Code - Do Not Modify!";

    static UnityWebRequestAsyncOperation _request;

    [MenuItem("Tools/Run Code Generation %#g")]
    public static void RunCodeGeneration()
    {
      var request = UnityWebRequest.Get(
        $"https://sheets.googleapis.com/v4/spreadsheets/{SheetId}/values:batchGet?ranges=Enums&key={ApiKey}");

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
      var rows = parsed["valueRanges"][0]["values"];

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
            enums[j].Add((row[j].Value, row[j+1].Value));
          }
        }
      }

      var builder = new StringBuilder();
      builder.Append(LicenseHeader);
      builder.Append("\nnamespace Nighthollow.Generated\n");
      builder.Append("{\n");

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

      EditorApplication.update -= EditorUpdate;
    }
  }
}
