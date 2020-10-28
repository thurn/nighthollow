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

#nullable enable

namespace Nighthollow.Statz
{
  public readonly struct DurationValue : IStatValue
  {
    readonly int _timeMilliseconds;

    public DurationValue(int timeMilliseconds)
    {
      _timeMilliseconds = timeMilliseconds;
    }

    public float AsSeconds() => _timeMilliseconds / 1000f;

    public int AsMilliseconds() => _timeMilliseconds;
  }

  public sealed class DurationStat : AbstractStat<NumericOperation<DurationValue>, DurationValue>
  {
    public DurationStat(int id) : base(id)
    {
    }

    public override DurationValue DefaultValue() => new DurationValue(0);

    public override DurationValue ComputeValue(IReadOnlyList<NumericOperation<DurationValue>> operations) =>
      new DurationValue(IntStat.Compute(operations, duration => new IntValue(duration.AsMilliseconds())).Int);
  }
}