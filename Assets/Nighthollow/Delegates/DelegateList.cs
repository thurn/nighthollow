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
using Nighthollow.Services;

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

    public IEnumerable<Effect> Invoke<THandler>(GameContext c, EventData<THandler> eventData)
      where THandler : IHandler =>
      AllHandlers<THandler>().SelectMany(pair => eventData.Invoke(c, pair.Item1, pair.Item2));

    public TResult First<THandler, TResult>(
      GameContext c,
      QueryData<THandler, TResult> queryData,
      TResult notFound) where THandler : IHandler
    {
      foreach (var (index, handler) in AllHandlers<THandler>())
      {
        return queryData.Invoke(c, index, handler);
      }

      return notFound;
    }

    public TResult? FirstNonNull<THandler, TResult>(
      GameContext c,
      QueryData<THandler, TResult?> queryData) where THandler : IHandler where TResult : class
    {
      return AllHandlers<THandler>()
        .Select(pair => queryData.Invoke(c, pair.Item1, pair.Item2))
        .FirstOrDefault(result => result != null);
    }

    public bool Any<THandler>(GameContext c, QueryData<THandler, bool> queryData) where THandler : IHandler =>
      AllHandlers<THandler>().Any(pair => queryData.Invoke(c, pair.Item1, pair.Item2));

    public TResult Iterate<THandler, TResult>(
      GameContext c,
      IteratedQueryData<THandler, TResult> queryData,
      TResult initialValue) where THandler : IHandler =>
      AllHandlers<THandler>().Aggregate(initialValue, (current, pair) =>
        queryData.Invoke(c, pair.Item1, pair.Item2, current));

    IEnumerable<(int, THandler)> AllHandlers<THandler>() where THandler : IHandler
    {
      var index = 1;
      foreach (var handler in _delegates)
      {
        if (handler is THandler h)
        {
          yield return (index++, h);
        }
      }

      if (_parent != null)
      {
        foreach (var (_, handler) in _parent.AllHandlers<THandler>())
        {
          yield return (index++, handler);
        }
      }
    }
  }
}