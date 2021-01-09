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
using Nighthollow.Data;

#nullable enable

namespace Nighthollow.Stats2
{
  public abstract class NumericStat<T> : AbstractStat<NumericStatModifier<T>, T> where T : struct
  {
    protected NumericStat(StatId statId) : base(statId)
    {
    }

    public IStatModifier Add(T value) => NumericStatModifier<T>.Add(this, value);

    public IStatModifier Increase(PercentageValue value) => NumericStatModifier<T>.Increase(this, value);

    public IStatModifier Set(T value) => NumericStatModifier<T>.Set(this, value);

    public IStatModifier? Set(T? nullable) =>
      nullable.HasValue ? NumericStatModifier<T>.Set(this, nullable.Value) : null;

    public override IStatModifier BuildModifier(ModifierType type, IValueData value) =>
      type switch
      {
        ModifierType.Add => Add((T) value.Get()),
        ModifierType.Increase => Increase((PercentageValue) value.Get()),
        ModifierType.Set => Set((T) value.Get()),
        _ => throw new InvalidOperationException($"Unsupported modifier type: {type}")
      };
  }
}