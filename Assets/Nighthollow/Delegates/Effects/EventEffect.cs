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
using Nighthollow.Delegates.Core;

namespace Nighthollow.Delegates.Effects
{
  public static class Events
  {
    public static EventEffect<ISkillDelegate, SkillContext> Skill(
      SkillContext c, Action<ISkillDelegate, SkillContext> action) =>
      new EventEffect<ISkillDelegate, SkillContext>(c.Delegate, c, action);

    public static EventEffect<ICreatureDelegate, CreatureContext> Creature(
      CreatureContext c, Action<ICreatureDelegate, CreatureContext> action) =>
      new EventEffect<ICreatureDelegate, CreatureContext>(c.Delegate, c, action);

    public static EventEffect<ICreatureDelegate, CreatureContext> Creature(
      SkillContext c, Action<ICreatureDelegate, CreatureContext> action) =>
      new EventEffect<ICreatureDelegate, CreatureContext>(c.Self.Data.Delegate, new CreatureContext(c.Self) ,action);
  }

  public sealed class EventEffect<TDelegate, TContext> : Effect
    where TDelegate : IDelegate where TContext : DelegateContext<TContext>
  {
    public TDelegate Delegate { get; }
    public TContext Context { get; }
    public Action<TDelegate, TContext> Action { get; }

    public EventEffect(TDelegate delegateInstance, TContext context, Action<TDelegate, TContext> action)
    {
      Delegate = delegateInstance;
      Context = context.New();
      Action = action;
    }

    public override void Execute()
    {
    }

    public override void RaiseEvents()
    {
      Action(Delegate, Context);
    }
  }
}