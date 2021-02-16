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

using Nighthollow.Data;

#nullable enable

namespace Nighthollow.Stats
{
  public sealed class BooleanStatModifier : AbstractStatModifier
  {
    public static BooleanStatModifier Set(AbstractStat<BooleanStatModifier, bool> stat, bool value) =>
      new BooleanStatModifier(stat.StatId, value);

    BooleanStatModifier(StatId statId, bool setValue) : base(statId, ModifierType.Set)
    {
      SetValue = setValue;
    }

    public bool SetValue { get; }

    public override string Describe(string template, IValueData? highValue)
    {
      var split = template.Split('/');
      return SetValue ? split[0].Trim() : split[1].Trim();
    }
  }
}
