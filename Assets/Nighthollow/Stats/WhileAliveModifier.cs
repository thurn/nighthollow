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
  public sealed class WhileAliveModifier : IModifier
  {
    readonly StaticModifier _modifier;
    readonly WeakReference<Creature> _scopeCreature;

    public WhileAliveModifier(StaticModifier modifier, Creature creature)
    {
      _modifier = modifier;
      _scopeCreature = new WeakReference<Creature>(Errors.CheckNotNull(creature));
    }

    WhileAliveModifier(StaticModifier modifier, WeakReference<Creature> scopeCreature)
    {
      _modifier = modifier;
      _scopeCreature = scopeCreature;
    }

    public StaticModifier BaseModifier => _modifier;

    public IModifier Clone() => this;

    public IModifier WithValue(IStatValue value) =>
      new WhileAliveModifier((StaticModifier) _modifier.WithValue(value), _scopeCreature);

    public IStatValue GetArgument() => BaseModifier.GetArgument();

    public bool IsDynamic() => true;

    public bool IsValid()
    {
      _scopeCreature.TryGetTarget(out var creature);
      return creature && creature.IsAlive();
    }
  }
}