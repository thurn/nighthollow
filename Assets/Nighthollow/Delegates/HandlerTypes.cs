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

#nullable enable

namespace Nighthollow.Delegates
{
  public interface IEventData
  {
    IEnumerable<Effect> Raise(DelegateContext c, DelegateList delegateList);
  }

  public abstract class EventData<THandler> : IEventData where THandler : IHandler
  {
    public abstract IEnumerable<Effect> Invoke(DelegateContext c, int delegateIndex, THandler handler);

    public IEnumerable<Effect> Raise(DelegateContext c, DelegateList delegateList) => delegateList.Invoke(c, this);
  }

  public abstract class QueryData<THandler, TResult>
  {
    public abstract TResult Invoke(DelegateContext c, int delegateIndex, THandler handler);
  }

  public abstract class IteratedQueryData<THandler, TResult>
  {
    public abstract TResult Invoke(DelegateContext c, int delegateIndex, THandler handler, TResult current);
  }
}