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
using System.Linq;
using Nighthollow.Stats;

namespace Nighthollow.Model
{
  public sealed class EnemyData : AbstractGameEntity
  {
    public IReadOnlyCollection<CreatureData> Enemies { get; }
    public override StatTable Stats { get; }

    public EnemyData(
      IReadOnlyCollection<CreatureData> enemies,
      StatTable stats)
    {
      Enemies = enemies;
      Stats = stats;
    }

    public EnemyData Clone() => new EnemyData(Enemies.Select(e => e.Clone()).ToList(), Stats.Clone());
  }
}