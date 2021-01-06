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
using System.Linq;

#nullable enable

namespace Nighthollow.Stats2
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
      groups[ModifierType.Set].Count(op => op.SetValue) > 0 &&
      groups[ModifierType.Set].Count(op => op.SetValue == false) == 0;
  }
}
