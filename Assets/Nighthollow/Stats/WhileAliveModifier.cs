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
using Nighthollow.Utils;

namespace Nighthollow.Stats
{
  public class WhileAliveModifier<TValue> : IModifier<TValue> where TValue : IStatValue
  {
    readonly StaticModifier<TValue> _modifier;
    readonly WeakReference<Creature> _scopeCreature;

    public WhileAliveModifier(StaticModifier<TValue> modifier, Creature creature)
    {
      _modifier = modifier;
      _scopeCreature = new WeakReference<Creature>(Errors.CheckNotNull(creature));
    }

    WhileAliveModifier(StaticModifier<TValue> modifier, WeakReference<Creature> scopeCreature)
    {
      _modifier = modifier;
      _scopeCreature = scopeCreature;
    }

    public StaticModifier<TValue> BaseModifier => _modifier;

    public IModifier<T> Clone<T>() where T : IStatValue => Errors.CheckNotNull(this as IModifier<T>);

    public IModifier WithValue<TNew>(TNew value) where TNew : IStatValue =>
      new WhileAliveModifier<TNew>((StaticModifier<TNew>) _modifier.WithValue(value), _scopeCreature);

    public IStatValue GetArgument() => BaseModifier.GetArgument();

    public bool IsDynamic() => true;

    public bool IsValid()
    {
      _scopeCreature.TryGetTarget(out var creature);
      return creature && creature.IsAlive();
    }
  }
}