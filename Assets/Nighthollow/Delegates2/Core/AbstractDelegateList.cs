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
using System.Collections.Generic;
using System.Linq;
using Nighthollow.Services;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Delegates2.Core
{
  public abstract class AbstractDelegateList
  {
    readonly IReadOnlyList<IDelegate> _delegates;
    readonly GameServiceRegistry _registry;

    protected AbstractDelegateList(IReadOnlyList<IDelegate> delegates, GameServiceRegistry registry)
    {
      Errors.CheckArgument(delegates.Count > 0, "Expected > 0 delegates");
      _delegates = delegates;
      _registry = registry;
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
      where TContext : DelegateContext
    {
      var context = delegateContext;

      var i = 0;
      foreach (var @delegate in AllDelegates(context))
      {
        context.DelegateIndex = i;
        var result = function(@delegate, context);
        if (result != null)
        {
          yield return result;
        }

        i++;
      }
    }

    protected TResult FirstOrDefault<TContext, TResult>(
      TContext delegateContext,
      Func<IDelegate, TContext, TResult> function)
      where TContext : DelegateContext => IterateDelegates(delegateContext, function).FirstOrDefault();

    protected bool AnyReturnedTrue<TContext>(TContext delegateContext, Func<IDelegate, TContext, bool?> function)
      where TContext : DelegateContext
    {
      return IterateDelegates(delegateContext, function).Any(v => v == true);
    }

    protected void ExecuteEvent<TContext>(
      TContext context,
      Action<IDelegate, TContext> action)
      where TContext : DelegateContext
    {
      var i = 0;
      foreach (var @delegate in AllDelegates(context))
      {
        context.DelegateIndex = i;
        action(@delegate, context);
        i++;
      }

      var results = context.Results.ClearAndReturnResults();

      foreach (var effect in results)
      {
        effect.Execute(_registry);
      }

      foreach (var effect in results)
      {
        effect.RaiseEvents();
      }
    }

    protected TResult AggregateDelegates<TContext, TResult>(
      TContext context,
      TResult initialValue,
      Func<IDelegate, TContext, TResult, TResult?> function)
      where TContext : DelegateContext where TResult : class
    {
      var i = 0;
      var value = initialValue;

      foreach (var @delegate in AllDelegates(context))
      {
        context.DelegateIndex = i;
        var result = function(@delegate, context, value);
        if (result != null)
        {
          value = result;
        }

        i++;
      }

      return value;
    }
  }
}
