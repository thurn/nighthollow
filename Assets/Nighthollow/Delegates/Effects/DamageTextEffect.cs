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

using Nighthollow.Components;
using Nighthollow.Services;

namespace Nighthollow.Delegates.Effects
{
  public sealed class DamageTextEffect : Effect
  {
    public Creature Target { get; }
    public int DamageAmount { get; }

    public DamageTextEffect(Creature target, int damageAmount)
    {
      Target = target;
      DamageAmount = damageAmount;
    }

    public override void Execute()
    {
      Root.Instance.DamageTextService.ShowDamageText(Target, DamageAmount);
    }
  }
}