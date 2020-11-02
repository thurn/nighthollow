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
using System.Linq;
using Nighthollow.Utils;

namespace Nighthollow.Delegates.Core
{
  public abstract class AbstractDelegateList
  {
    readonly IReadOnlyList<IDelegate> _delegates;

    protected AbstractDelegateList(IReadOnlyList<IDelegate> delegates)
    {
      Errors.CheckArgument(delegates.Count > 0, "Expected > 0 delegates");
      _delegates = delegates;
    }

    protected abstract AbstractDelegateList? GetChild(DelegateContext context);

    IEnumerable<IDelegate> AllDelegates(DelegateContext context)
    {
      var child = GetChild(context);
      return child == null ? _delegates : _delegates.Concat(child._delegates);
    }

    IEnumerable<TResult> IterateDelegates<TContext, TResult>(
      TContext delegateContext,
      Func<IDelegate, TContext, TResult> function)
      where TContext : DelegateContext, IDelegateContext<TContext>
    {
      var context = delegateContext.Clone();

      var i = 0;
      foreach (var @delegate in AllDelegates(context))
      {
        context.Implemented = true;
        context.DelegateIndex = i;
        var result = function(@delegate, context);
        if (context.Implemented)
        {
          yield return result;
        }

        i++;
      }
    }

    protected TResult GetFirstImplemented<TContext, TResult>(
      TContext delegateContext,
      Func<IDelegate, TContext, TResult> function)
      where TContext : DelegateContext, IDelegateContext<TContext> =>
      IterateDelegates(delegateContext, function).First();

    protected bool AnyReturnedTrue<TContext>(TContext delegateContext, Func<IDelegate, TContext, bool> function)
      where TContext : DelegateContext, IDelegateContext<TContext> =>
      IterateDelegates(delegateContext, function).Any(v => v);

    protected void ExecuteEvent<TContext>(
      TContext delegateContext,
      Action<IDelegate, TContext> action)
      where TContext : DelegateContext, IDelegateContext<TContext>
    {
      var context = delegateContext.Clone();

      var i = 0;
      foreach (var @delegate in AllDelegates(context))
      {
        context.DelegateIndex = i;
        action(@delegate, context);
        i++;
      }

      foreach (var effect in context.Results.Values)
      {
        effect.Execute();
      }

      foreach (var effect in context.Results.Values)
      {
        effect.RaiseEvents();
      }
    }

    protected TResult AggregateDelegates<TContext, TResult>(
      TContext delegateContext,
      TResult initialValue,
      Func<IDelegate, TContext, TResult, TResult> function)
      where TContext : DelegateContext, IDelegateContext<TContext>
    {
      var context = delegateContext.Clone();
      var i = 0;
      var value = initialValue;

      foreach (var @delegate in AllDelegates(context))
      {
        context.Implemented = true;
        context.DelegateIndex = i;
        var result = function(@delegate, context, value);
        if (context.Implemented)
        {
          value = result;
        }

        i++;
      }

      return value;
    }
  }
}