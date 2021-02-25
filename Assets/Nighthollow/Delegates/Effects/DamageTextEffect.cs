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


using Nighthollow.Components;
using Nighthollow.Services;

#nullable enable

namespace Nighthollow.Delegates.Effects
{
  public sealed class DamageTextEffect : Effect
  {
    public DamageTextEffect(Creature2 target, int damageAmount)
    {
      Target = target;
      DamageAmount = damageAmount;
    }

    public Creature2 Target { get; }
    public int DamageAmount { get; }

    public override void Execute(GameServiceRegistry registry)
    {
      Root.Instance.DamageTextService.ShowDamageText(Target, DamageAmount);
    }
  }
}
