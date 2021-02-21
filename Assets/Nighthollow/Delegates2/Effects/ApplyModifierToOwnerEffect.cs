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
using Nighthollow.Components;
using Nighthollow.Data;
using Nighthollow.Services;
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Delegates2.Effects
{
  public sealed class ApplyModifierToOwnerEffect : Effect
  {
    public ApplyModifierToOwnerEffect(Creature self, IStatModifier modifier)
    {
      Self = self;
      Modifier = modifier;
    }

    public Creature Self { get; }
    public IStatModifier Modifier { get; }

    public override void Execute(GameServiceRegistry registry)
    {
      switch (Self.Owner)
      {
        case PlayerName.User:
          Root.Instance.User.InsertModifier(Modifier);
          break;
        case PlayerName.Enemy:
          Root.Instance.Enemy.InsertModifier(Modifier);
          break;
        case PlayerName.Unknown:
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
}
