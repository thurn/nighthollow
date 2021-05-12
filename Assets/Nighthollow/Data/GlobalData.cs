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
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Data
{
  /**
   * A simple system for managing named global variables. Only variables which can be represented by a single integer
   * are supported.
   */
  [MessagePackObject]
  public sealed partial class GlobalData
  {
    [SerializationConstructor]
    public GlobalData(string name, int value, string? comment)
    {
      Name = name;
      Value = value;
      Comment = comment;
    }

    public GlobalData(string name, bool value, string? comment) : this(name, value ? 1 : 0, comment)
    {
    }

    public GlobalData(string name, PercentageValue value, string? comment) : this(name, value.BasisPoints, comment)
    {
    }

    public GlobalData(string name, DurationValue value, string? comment) : this(name, value.TimeMilliseconds, comment)
    {
    }

    [Key(0)] public string Name { get; }
    [Key(1)] public int Value { get; }
    [Key(2)] public string? Comment { get; }

    public GlobalData WithBool(bool b) => WithValue(b ? 1 : 0);
    public bool AsBool() => Value == 1;

    public GlobalData WithPercentage(PercentageValue v) => WithValue(v.BasisPoints);
    public PercentageValue AsPercentage() => new PercentageValue(Value);

    public GlobalData WithDuration(DurationValue d) => WithValue(d.TimeMilliseconds);
    public DurationValue AsDuration() => new DurationValue(Value);
  }
}