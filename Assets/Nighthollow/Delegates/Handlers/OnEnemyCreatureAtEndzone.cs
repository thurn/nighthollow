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

namespace Nighthollow.Delegates.Handlers
{
  public interface IOnEnemyCreatureAtEndzone : IHandler
  {
    public sealed class Data : CreatureEventData<IOnEnemyCreatureAtEndzone>
    {
      public Data(CreatureId self) : base(self)
      {
      }

      public override IEnumerable<Effect> Invoke(
        IGameContext c, IOnEnemyCreatureAtEndzone handler) =>
        handler.OnEnemyCreatureAtEndzone(c, this);
    }

    /// <summary>Called when an enemy creature passes all defenders and reaches the end of the board.</summary>
    IEnumerable<Effect> OnEnemyCreatureAtEndzone(IGameContext context, Data data);
  }
}