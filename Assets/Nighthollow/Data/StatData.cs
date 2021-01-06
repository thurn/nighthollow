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
using Nighthollow.Generated;

#nullable enable

namespace Nighthollow.Data
{
  [MessagePackObject]
  public sealed partial class StatData
  {
    public StatData(
      string name,
      StatType statType,
      IValueData defaultValue,
      string description,
      string comment)
    {
      Name = name;
      StatType = statType;
      DefaultValue = defaultValue;
      Description = description;
      Comment = comment;
    }

    [Key(0)] public string Name { get; }
    [Key(1)] public StatType StatType { get; }
    [Key(2)] public IValueData DefaultValue { get; }
    [Key(3)] public string Description { get; }
    [Key(4)] public string Comment { get; }
  }
}
