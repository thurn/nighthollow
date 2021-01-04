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

namespace Nighthollow.Stats2
{
  [Union(0, typeof(NumericOperation<int>))]
  [Union(1, typeof(NumericOperation<DurationValue>))]
  [Union(2, typeof(NumericOperation<PercentageValue>))]
  [Union(3, typeof(NumericOperation<IntRangeValue>))]
  [Union(4, typeof(BooleanOperation))]
  [Union(5, typeof(TaggedNumericOperation<DamageType, int>))]
  [Union(6, typeof(TaggedNumericOperation<DamageType, PercentageValue>))]
  [Union(7, typeof(TaggedNumericOperation<DamageType, IntRangeValue>))]
  [Union(8, typeof(TaggedNumericOperation<School, int>))]
  public interface IOperation
  {
    public StatId StatId { get; }

    OperationType Type { get; }
  }

  // Hack: MessagePack can't find types that are only ever used in a union declaration, so we need to have a concrete
  // list of all types we expect to appear as well.
  [MessagePackObject]
  public sealed class SerializationHack
  {
    [Key(0)] public NumericOperation<int> Zero = null!;
    [Key(1)] public NumericOperation<DurationValue> One = null!;
    [Key(2)] public NumericOperation<PercentageValue> Two = null!;
    [Key(3)] public NumericOperation<IntRangeValue> Three = null!;
    [Key(4)] public BooleanOperation Four = null!;
    [Key(5)] public TaggedNumericOperation<DamageType, int> Five = null!;
    [Key(6)] public TaggedNumericOperation<DamageType, PercentageValue> Six = null!;
    [Key(7)] public TaggedNumericOperation<DamageType, IntRangeValue> Seven = null!;
    [Key(8)] public TaggedNumericOperation<School, int> Eight = null!;
  }
}