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


using System;
using Nighthollow.Data;
using Nighthollow.Services;
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Delegates.Effects
{
  public sealed class ApplyModifierToOwnerEffect : Effect
  {
    public ApplyModifierToOwnerEffect(CreatureId self, IStatModifier modifier)
    {
      Self = self;
      Modifier = modifier;
    }

    public CreatureId Self { get; }
    public IStatModifier Modifier { get; }

    public override void Execute(BattleServiceRegistry registry)
    {
      switch (registry.Creatures[Self].Owner)
      {
        case PlayerName.User:
          registry.UserController.InsertStatModifier(Modifier);
          break;
        case PlayerName.Enemy:
          registry.EnemyController.InsertStatModifier(Modifier);
          break;
        case PlayerName.Unknown:
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
}
