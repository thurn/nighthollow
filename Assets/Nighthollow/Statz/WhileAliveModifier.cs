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
using Nighthollow.Components;

namespace Nighthollow.Statz
{
  public sealed class WhileAliveModifier<TOperation> : IModifier<TOperation> where TOperation : IOperation
  {
    readonly WeakReference<Creature> _scopeCreature;

    public TOperation Operation { get; }

    public bool IsValid()
    {
      _scopeCreature.TryGetTarget(out var creature);
      return creature && creature.IsAlive();
    }

    public WhileAliveModifier(TOperation operation, WeakReference<Creature> scopeCreature)
    {
      Operation = operation;
      _scopeCreature = scopeCreature;
    }
  }
}