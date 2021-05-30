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
using Nighthollow.Services;

#nullable enable

namespace Nighthollow.Delegates
{
  public interface IDelegateLocator
  {
    public DelegateList GetDelegateList(IGameContext c);
  }

  public interface IEventData
  {
    IEnumerable<Effect> Raise(IGameContext c);
  }

  public abstract class EventData<THandler> : IEventData where THandler : IHandler
  {
    public abstract IEnumerable<Effect> Raise(IGameContext c);

    public abstract IEnumerable<Effect> Invoke(IGameContext c, THandler handler);
  }

  public abstract class CreatureEventData<THandler> : EventData<THandler> where THandler : IHandler
  {
    protected CreatureEventData(CreatureId self)
    {
      Self = self;
    }

    public CreatureId Self { get; }

    public override IEnumerable<Effect> Raise(IGameContext c) => Self.GetDelegateList(c).Invoke(c, this);
  }

  public abstract class GlobalEventData<THandler> : EventData<THandler> where THandler : IHandler
  {
    public override IEnumerable<Effect> Raise(IGameContext c) => DelegateList.Root.Invoke(c, this);
  }

  public abstract class QueryData<THandler, TResult>
  {
    public abstract TResult Invoke(IGameContext c, THandler handler);
  }

  public abstract class IteratedQueryData<THandler, TResult>
  {
    public abstract TResult Invoke(IGameContext c, THandler handler, TResult current);
  }
}