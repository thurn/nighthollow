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

using System.Collections.Immutable;
using Nighthollow.Data;

#nullable enable

namespace Nighthollow.Delegates.Handlers
{
  public interface IOnKilledEnemy : IHandler
  {
    public sealed class Data : EventData<IOnKilledEnemy>
    {
      public Data(CreatureState self, int enemyId)
      {
        Self = self;
        EnemyId = enemyId;
      }

      public override ImmutableList<Effect> Invoke(DelegateContext c, IOnKilledEnemy handler) =>
        handler.OnKilledEnemy(c, this);

      public CreatureState Self { get; }
      public int EnemyId { get; }
    }

    /// <summary>Called when a creature kills an enemy creature.</summary>
    ImmutableList<Effect> OnKilledEnemy(DelegateContext c, Data d);
  }
}