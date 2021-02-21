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
using Nighthollow.Delegates2.Core;
using Nighthollow.Services;

#nullable enable

namespace Nighthollow.Delegates2.Effects
{
  public static class EffectEvents
  {
    public static EventEffect<TContext> Effect<TContext>(TContext context, Action<IDelegate, TContext> action)
      where TContext : DelegateContext =>
      new EventEffect<TContext>(context.Delegate, context, action);
  }

  public sealed class EventEffect<TContext> : Effect where TContext : DelegateContext
  {
    public EventEffect(IDelegate delegateInstance, TContext context, Action<IDelegate, TContext> action)
    {
      Delegate = delegateInstance;
      Context = context;
      Action = action;
    }

    public IDelegate Delegate { get; }
    public TContext Context { get; }
    public Action<IDelegate, TContext> Action { get; }

    public override void Execute(GameServiceRegistry registry)
    {
    }

    public override void RaiseEvents()
    {
      Action(Delegate, Context);
    }
  }
}
