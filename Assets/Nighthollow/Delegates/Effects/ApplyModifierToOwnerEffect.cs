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

#nullable enable

using System;
using Nighthollow.Components;
using Nighthollow.Delegates.Core;
using Nighthollow.Generated;
using Nighthollow.Services;
using Nighthollow.Stats;

namespace Nighthollow.Delegates.Effects
{
  public sealed class ApplyModifierToOwnerEffect : Effect
  {
    public Creature Self { get; }
    public IStatModifier Modifier { get; }

    public ApplyModifierToOwnerEffect(Creature self, IStatModifier modifier)
    {
      Self = self;
      Modifier = modifier;
    }

    public override void Execute()
    {
      switch (Self.Owner)
      {
        case PlayerName.User:
          Modifier.ApplyTo(Root.Instance.User.Data.Stats);
          break;
        case PlayerName.Enemy:
          Modifier.ApplyTo(Root.Instance.Enemy.Data.Stats);
          break;
        case PlayerName.Unknown:
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
}