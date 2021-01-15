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
using System.IO;
using System.Text;
using Nighthollow.Data;
using Nighthollow.Services;
using Nighthollow.Stats;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

#nullable enable

namespace Nighthollow.Editor.Data
{
  public static class StatsCodeGenerator
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

    [MenuItem("Tools/Run Code Generation &#^g")]
    public static void RunCodeGeneration()
    {
      Debug.Log($"Running generation:");
      var root = new GameObject("Temp");
      var dataService = root.AddComponent<DataService>();
      dataService.Initialize(synchronous: true).MoveNext();
      dataService.OnReady(Generate);
      
      Object.DestroyImmediate(root);
    }

    static void Generate(FetchResult fetchResult)
    {
      var stats = fetchResult.Database.Snapshot().StatData;
      var builder = CreateHeader("\nusing System;\nusing Nighthollow.Data;\n");

      builder.Append("  public enum StatId\n");
      builder.Append("  {\n");
      builder.Append($"    Unknown = 0,\n");
      foreach (var pair in stats)
      {
        builder.Append($"    {pair.Value.Name} = {pair.Key},\n");
      }

      builder.Append("  }\n");

      builder.Append("\n  public static class Stat\n");
      builder.Append("  {\n");

      foreach (var stat in stats.Values)
      {
        var (statType, arg) = StatType(stat);

        builder.Append($"    public static readonly {statType} {stat.Name} =\n");
        builder.Append($"        new {statType}(StatId.{stat.Name}{arg});\n");
      }

      builder.Append("\n    public static IStat GetStat(StatId statId)\n");
      builder.Append("    {\n");
      builder.Append("      switch (statId)\n");
      builder.Append("      {\n");
      foreach (var stat in stats.Values)
      {
        builder.Append($"        case StatId.{stat.Name}: return {stat.Name};\n");
      }

      builder.Append($"        default: throw new ArgumentOutOfRangeException(statId.ToString());\n");
      builder.Append("      }\n");
      builder.Append("    }\n");

      builder.Append("  }\n");
      builder.Append("}\n");

      File.WriteAllText("Assets/Nighthollow/Stats2/Generated.cs", builder.ToString());
    }

    static (string, string) StatType(StatData statData)
    {
      var baseType = statData.StatType switch
      {
        Nighthollow.Data.StatType.Int => nameof(IntStat),
        Nighthollow.Data.StatType.Bool => nameof(BoolStat),
        Nighthollow.Data.StatType.IntRange => nameof(IntRangeStat),
        Nighthollow.Data.StatType.Duration => nameof(DurationStat),
        Nighthollow.Data.StatType.Percentage => nameof(PercentageStat),
        Nighthollow.Data.StatType.TaggedValues => "TaggedValuesStat",
        _ => throw new ArgumentOutOfRangeException()
      };

      if (statData.StatType == Nighthollow.Data.StatType.TaggedValues)
      {
        var (valueType, childType) = statData.StatValueType switch
        {
          Nighthollow.Data.StatType.Int => ("int", nameof(IntStat)),
          Nighthollow.Data.StatType.Bool => ("bool", nameof(BoolStat)),
          Nighthollow.Data.StatType.IntRange => (nameof(IntRangeValue), nameof(IntRangeStat)),
          Nighthollow.Data.StatType.Duration => (nameof(DurationValue), nameof(DurationStat)),
          Nighthollow.Data.StatType.Percentage => (nameof(PercentageValue), nameof(PercentageStat)),
          _ => throw new InvalidOperationException("Unsupported tagged value type")
        };

        return ($"{baseType}<{statData.TagType}, {valueType}>", $", new {childType}(StatId.{statData.Name})");
      }

      return (baseType, "");
    }

    static StringBuilder CreateHeader(string usingDirective = "")
    {
      var builder = new StringBuilder();
      builder.Append(LicenseHeader);
      builder.Append($"\n{usingDirective}");
      builder.Append("\n// Generated Code - Do Not Modify!");
      builder.Append("\nnamespace Nighthollow.Stats2\n");
      builder.Append("{\n");
      return builder;
    }
  }
}
