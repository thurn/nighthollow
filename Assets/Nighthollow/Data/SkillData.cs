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

using Nighthollow.Delegates.Core;
using Nighthollow.Stats2;

#nullable enable

namespace Nighthollow.Data
{
  public sealed partial class SkillData : StatEntity
  {
    public SkillData(
      IDelegate @delegate,
      StatTable stats,
      SkillTypeData baseType,
      SkillItemData itemData)
    {
      Delegate = @delegate;
      Stats = stats;
      BaseType = baseType;
      ItemData = itemData;
    }

    [Field] public IDelegate Delegate { get; }
    [Field] public override StatTable Stats { get; }
    [Field] public SkillTypeData BaseType { get; }
    [Field] public SkillItemData ItemData { get; }
  }
}
