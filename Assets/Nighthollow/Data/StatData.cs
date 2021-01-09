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

using MessagePack;
using Nighthollow.Stats2;

#nullable enable

namespace Nighthollow.Data
{
  public enum StatType
  {
    Unknown = 0,
    Int = 1,
    Bool = 2,
    IntRange = 3,
    Duration = 4,
    Percentage = 5,
    TaggedValues = 6
  }

  public enum StatTagType
  {
    Unknown = 0,
    DamageType = 1,
    School = 2
  }

  [MessagePackObject]
  public sealed partial class StatData
  {
    public StatData(
      string name,
      StatType statType,
      string? descriptionTemplate = null,
      IValueData? defaultValue = null,
      StatTagType? tagType = null,
      StatType? statValueType = null,
      string? comment = null)
    {
      Name = name;
      StatType = statType;
      DescriptionTemplate = descriptionTemplate;
      DefaultValue = defaultValue;
      TagType = tagType;
      StatValueType = statValueType;
      Comment = comment;
    }

    [Key(0)] public string Name { get; }
    [Key(1)] public StatType StatType { get; }
    [Key(2)] public string? DescriptionTemplate { get; }
    [Key(3)] public IValueData? DefaultValue { get; }
    [Key(4)] public StatTagType? TagType { get; }
    [Key(5)] public StatType? StatValueType { get; }
    [Key(6)] public string? Comment { get; }

    public IValueData? Parse(string input)
    {
      switch (StatType)
      {
        case StatType.Int:
          if (int.TryParse(input, out var i))
          {
            return new IntValueData(i);
          }

          break;
        case StatType.Bool:
          if (bool.TryParse(input, out var b))
          {
            return new BoolValueData(b);
          }

          break;
        case StatType.Duration:
          if (DurationValue.TryParse(input, out var d))
          {
            return d;
          }

          break;

        case StatType.Percentage:
          if (PercentageValue.TryParse(input, out var p))
          {
            return p;
          }

          break;

        case StatType.IntRange:
          if (IntRangeValue.TryParse(input, out var ir))
          {
            return ir;
          }

          break;
      }

      return null;
    }
  }
}
