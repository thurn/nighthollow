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
  public interface IStatModifier
  {
    IStat Stat { get; }

    IOperation Operation { get; }
  }

  public sealed class StatModifier<TOperation, TValue> : IStatModifier where TOperation : IOperation
  {
    public StatModifier(AbstractStat<TOperation, TValue> stat, TOperation operation)
    {
      ModifierStat = stat;
      ModifierOperation = operation;
    }

    public AbstractStat<TOperation, TValue> ModifierStat { get; }

    public TOperation ModifierOperation { get; }

    public IStat Stat => ModifierStat;

    public IOperation Operation => ModifierOperation;
  }
}