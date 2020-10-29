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

#nullable enable

using System;
using System.Collections.Generic;

namespace Nighthollow.Stats
{
  public static class TaggedNumericOperation
  {
    public static TaggedNumericOperation<TTag, TValue> Add<TTag, TValue>(TTag tag, TValue value)
      where TTag : Enum where TValue : struct, IStatValue =>
      new TaggedNumericOperation<TTag, TValue>(new Dictionary<TTag, NumericOperation<TValue>>
      {
        {tag, NumericOperation.Add(value)}
      });

    public static TaggedNumericOperation<TTag, TValue> Increase<TTag, TValue>(TTag tag, PercentageValue value)
      where TTag : Enum where TValue : struct, IStatValue =>
      new TaggedNumericOperation<TTag, TValue>(new Dictionary<TTag, NumericOperation<TValue>>
      {
        {tag, NumericOperation.Increase<TValue>(value)}
      });
  }

  public sealed class TaggedNumericOperation<TTag, TValue> : IOperation
    where TTag : Enum where TValue : struct, IStatValue
  {
    public IReadOnlyDictionary<TTag, NumericOperation<TValue>> Operations { get; }

    public TaggedNumericOperation(IReadOnlyDictionary<TTag, NumericOperation<TValue>> operations)
    {
      Operations = operations;
    }
  }
}