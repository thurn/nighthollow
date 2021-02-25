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
using Nighthollow.Data;

#nullable enable

namespace Nighthollow.Delegates.Handlers
{
  public interface IOnEnemyCreatureAtEndzone : IHandler
  {
    public sealed class Data : EventData<IOnEnemyCreatureAtEndzone>
    {
      public Data(CreatureState self)
      {
        Self = self;
      }

      public override IEnumerable<Effect> Invoke(
        DelegateContext c, int delegateIndex, IOnEnemyCreatureAtEndzone handler) =>
        handler.OnEnemyCreatureAtEndzone(c, delegateIndex, this);

      public CreatureState Self { get; }
    }

    /// <summary>Called when an enemy creature passes all defenders and reaches the end of the board.</summary>
    IEnumerable<Effect> OnEnemyCreatureAtEndzone(DelegateContext context, int delegateIndex, Data data);
  }
}