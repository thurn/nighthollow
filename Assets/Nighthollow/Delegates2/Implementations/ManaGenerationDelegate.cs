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


using Nighthollow.Delegates.Effects;
using Nighthollow.Delegates2.Core;
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Delegates2.Implementations
{
  public sealed class ManaGenerationDelegate : AbstractDelegate
  {
    public override string Describe(IStatDescriptionProvider provider) =>
      $"+{provider.Get(Stat.AddedManaGain)} Mana Generated";

    public override void OnActivate(CreatureContext c)
    {
      c.Results.Add(new ApplyModifierToOwnerEffect(c.Self,
        Stat.ManaGain.Add(c.Get(Stat.AddedManaGain)).WithLifetime(new WhileAliveLifetime(c.Self))));
    }
  }
}
