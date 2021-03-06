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
using System.Linq;
using Nighthollow.Data;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Stats
{
  public sealed class BoolStat : AbstractStat<BooleanStatModifier, bool>
  {
    public BoolStat(StatId statId) : base(statId)
    {
    }

    public IStatModifier Set(bool value) => BooleanStatModifier.Set(this, value);

    public IStatModifier? SetIfTrue(bool value) => value ? Set(value) : null;

    public override bool ComputeValue(
      IReadOnlyDictionary<ModifierType, IEnumerable<BooleanStatModifier>> groups) =>
      groups
        .GetOrReturnDefault(ModifierType.Set, Enumerable.Empty<BooleanStatModifier>())
        .Count(op => op.SetValue) > 0 &&
      groups
        .GetOrReturnDefault(ModifierType.Set, Enumerable.Empty<BooleanStatModifier>())
        .Count(op => op.SetValue == false) == 0;

    public override IStatModifier BuildModifier(ModifierType type, IValueData value) =>
      type switch
      {
        ModifierType.Set => BooleanStatModifier.Set(this, (bool) value.Get()),
        _ => throw new InvalidOperationException($"Unsupported modifier type: {type}")
      };

    protected override bool TryParseValue(string input, out IValueData result)
    {
      if (bool.TryParse(input, out var boolean))
      {
        result = new BoolValueData(boolean);
        return true;
      }

      result = null!;
      return false;
    }
  }
}
