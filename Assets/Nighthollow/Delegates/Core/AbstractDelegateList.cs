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
using Nighthollow.Utils;

namespace Nighthollow.Delegates.Core
{
  public abstract class AbstractDelegateList<TContext, TDelegate>
    where TContext : DelegateContext<TContext> where TDelegate : IDelegate
  {
    readonly IReadOnlyList<TDelegate> _delegates;

    protected AbstractDelegateList(IReadOnlyList<TDelegate> delegates)
    {
      Errors.CheckArgument(delegates.Count > 0, "Expected > 0 delegates");
      _delegates = delegates;
    }

    protected TResult GetFirstImplemented<TResult>(
      TContext delegateContext,
      Func<TDelegate, TContext, TResult> function)
    {
      var context = delegateContext.New();

      for (var i = 0; i < _delegates.Count; ++i)
      {
        context.Implemented = true;
        context.DelegateIndex = i;
        var result = function(_delegates[i], context);
        if (context.Implemented)
        {
          return result;
        }
      }

      throw new InvalidOperationException("No implementation found for callback.");
    }

    protected void ExecuteEvent(
      TContext delegateContext,
      Action<TDelegate, TContext> action)
    {
      var context = delegateContext.New();

      for (var i = 0; i < _delegates.Count; ++i)
      {
        context.DelegateIndex = i;
        action(_delegates[i], context);
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

    protected bool AnyReturnedTrue(TContext delegateContext, Func<TDelegate, TContext, bool> function)
    {
      var context = delegateContext.New();

      for (var i = 0; i < _delegates.Count; ++i)
      {
        context.Implemented = true;
        context.DelegateIndex = i;
        var result = function(_delegates[i], context);
        if (context.Implemented && result)
        {
          return true;
        }
      }

      return false;
    }
  }
}