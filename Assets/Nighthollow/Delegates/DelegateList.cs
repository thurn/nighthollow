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
using System.Linq;

#nullable enable

namespace Nighthollow.Delegates
{
  public sealed class DelegateList
  {
    readonly ImmutableList<AbstractDelegate> _delegates;
    readonly DelegateList? _parent;

    public DelegateList(ImmutableList<AbstractDelegate> delegates, DelegateList? parent)
    {
      _delegates = delegates;
      _parent = parent;
    }

    public IEnumerable<Effect> Invoke<THandler>(DelegateContext c, EventData<THandler> eventData)
      where THandler : IHandler =>
      AllHandlers<THandler>().SelectMany(handler => eventData.Invoke(c, handler));

    public TResult First<THandler, TResult>(
      DelegateContext c,
      QueryData<THandler, TResult> queryData,
      TResult notFound) where THandler : IHandler
    {
      foreach (var handler in AllHandlers<THandler>())
      {
        return queryData.Invoke(c, handler);
      }

      return notFound;
    }

    public TResult? FirstNonNull<THandler, TResult>(
      DelegateContext c,
      QueryData<THandler, TResult?> queryData) where THandler : IHandler where TResult : class
    {
      return AllHandlers<THandler>()
        .Select(handler => queryData.Invoke(c, handler))
        .FirstOrDefault(result => result != null);
    }

    public bool Any<THandler>(DelegateContext c, QueryData<THandler, bool> queryData) where THandler : IHandler =>
      AllHandlers<THandler>().Any(handler => queryData.Invoke(c, handler));

    public TResult Iterate<THandler, TResult>(
      DelegateContext c,
      IteratedQueryData<THandler, TResult> queryData,
      TResult initialValue) where THandler : IHandler =>
      AllHandlers<THandler>().Aggregate(initialValue, (current, handler) => queryData.Invoke(c, handler, current));

    IEnumerable<THandler> AllHandlers<THandler>() where THandler : IHandler
    {
      foreach (var handler in _delegates)
      {
        if (handler is THandler h)
        {
          yield return h;
        }
      }

      if (_parent != null)
      {
        foreach (var handler in _parent.AllHandlers<THandler>())
        {
          yield return handler;
        }
      }
    }
  }
}