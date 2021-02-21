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

using System.Collections.Generic;
using System.Collections.Immutable;
using Nighthollow.Delegates.Effects;

#nullable enable

namespace Nighthollow.Delegates
{
  public sealed class DelegateList
  {
    readonly ImmutableList<AbstractDelegate> _delegates;

    public DelegateList(ImmutableList<AbstractDelegate> delegates)
    {
      _delegates = delegates;
    }

    public IEnumerable<Effect> Invoke<THandler>(DelegateContext c, EventData<THandler> eventData)
      where THandler : IHandler
    {
      foreach (var handler in _delegates)
      {
        if (handler is THandler h)
        {
          foreach (var effect in eventData.Invoke(c, h))
          {
            yield return effect;
          }
        }
      }
    }

    public TResult QueryFirst<THandler, TResult>(
      DelegateContext c,
      QueryData<THandler, TResult> queryData,
      TResult notFound) where THandler : IHandler
    {
      foreach (var handler in _delegates)
      {
        if (handler is THandler h)
        {
          return queryData.Invoke(c, h);
        }
      }

      return notFound;
    }

    public bool Any<THandler>(DelegateContext c, QueryData<THandler, bool> queryData) where THandler : IHandler
    {
      foreach (var handler in _delegates)
      {
        if (handler is THandler h)
        {
          if (queryData.Invoke(c, h))
          {
            return true;
          }
        }
      }

      return false;
    }

    public TResult Iterate<THandler, TResult>(
      DelegateContext c,
      IteratedQueryData<THandler, TResult> queryData,
      TResult initialValue) where THandler : IHandler
    {
      var result = initialValue;
      foreach (var handler in _delegates)
      {
        if (handler is THandler h)
        {
          result = queryData.Invoke(c, h, result);
        }
      }

      return result;
    }
  }
}