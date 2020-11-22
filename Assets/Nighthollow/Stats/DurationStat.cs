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
using Nighthollow.Generated;
using UnityEngine;

#nullable enable

namespace Nighthollow.Stats
{
  public readonly struct DurationValue
  {
    readonly int _timeMilliseconds;

    public DurationValue(int timeMilliseconds)
    {
      _timeMilliseconds = timeMilliseconds;
    }

    public override string ToString() => $"{_timeMilliseconds / 1000f}s";

    public float AsSeconds() => _timeMilliseconds / 1000f;

    public int AsMilliseconds() => _timeMilliseconds;
  }

  public sealed class DurationStat : NumericStat<DurationValue>
  {
    public DurationStat(StatId id) : base(id)
    {
    }

    public override DurationValue ComputeValue(IReadOnlyList<NumericOperation<DurationValue>> operations) =>
      new DurationValue(IntStat.Compute(operations, duration => duration.AsMilliseconds()));

    protected override DurationValue ParseStatValue(string value) => ParseDuration(value);

    public static DurationValue ParseDuration(string value) =>
      new DurationValue(Mathf.RoundToInt(float.Parse(value.Replace("s", "")) * 1000f));
  }
}
