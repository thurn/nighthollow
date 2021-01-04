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
using MessagePack;

#nullable enable

namespace Nighthollow.Stats2
{
  [MessagePackObject]
  public sealed class NumericOperation<T> : IOperation where T : struct
  {
#pragma warning disable 618 // Using Obsolete
    public static NumericOperation<T> Add(T value) =>
      new NumericOperation<T>(OperationType.Add, value, null, null);

    public static NumericOperation<T> Increase(PercentageValue value) =>
      new NumericOperation<T>(OperationType.Increase, null, value, null);

    public static NumericOperation<T> Overwrite(T value) =>
      new NumericOperation<T>(OperationType.Set, null, null, value);
#pragma warning restore 618

    [Obsolete("This constructor is visible only for use by the serialization system.")]
    public NumericOperation(OperationType type, T? add, PercentageValue? increase, T? ovewrite)
    {
      Type = type;
      AddTo = add;
      IncreaseBy = increase;
      OvewriteWith = ovewrite;
    }

    [Key(0)] public OperationType Type { get; }
    [Key(1)] public T? AddTo { get; }
    [Key(2)] public PercentageValue? IncreaseBy { get; }
    [Key(3)] public T? OvewriteWith { get; }
  }
}