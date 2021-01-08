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
using Nighthollow.Delegates.Implementations;

#nullable enable

namespace Nighthollow.Delegates.Core
{
  public enum DelegateId
  {
    Unknown = 0,
    DefaultCreatureDelegate = 1,
    DefaultSkillDelegate = 2,
    MultipleProjectilesDelegate = 3,
    ProjectileArcDelegate = 4,
    AdjacentFileProjectilesDelegate = 5,
    ChainingProjectilesDelegate = 6,
    ManaGenerationDelegate = 7,
    InfluenceAddedDelegate = 8,
    KnockbackOnHitDelegate = 9,
    SummonMinionsDelegate = 10,
    AddedDamageOnRepeatedHitsDelegate = 11,
    ApplyTargetedAffixesOnHitDelegate = 12,
    ChainToRandomTargetDelegate = 13,
    ChanceToShockDelegate = 14,
    ApplyToAdjacentAlliesOnUseDelegate = 15
  }

  public static class DelegateMap
  {
    static readonly Dictionary<DelegateId, IDelegate> Delegates = new Dictionary<DelegateId, IDelegate>
    {
      {DelegateId.DefaultCreatureDelegate, new DefaultCreatureDelegate()},
      {DelegateId.DefaultSkillDelegate, new DefaultSkillDelegate()},
      {DelegateId.MultipleProjectilesDelegate, new MultipleProjectilesDelegate()},
      {DelegateId.ProjectileArcDelegate, new ProjectileArcDelegate()},
      {DelegateId.AdjacentFileProjectilesDelegate, new AdjacentFileProjectilesDelegate()},
      {DelegateId.ChainingProjectilesDelegate, new ChainingProjectilesDelegate()},
      {DelegateId.ManaGenerationDelegate, new ManaGenerationDelegate()},
      {DelegateId.InfluenceAddedDelegate, new InfluenceAddedDelegate()},
      {DelegateId.KnockbackOnHitDelegate, new KnockbackOnHitDelegate()},
      {DelegateId.SummonMinionsDelegate, new SummonMinionsDelegate()},
      {DelegateId.AddedDamageOnRepeatedHitsDelegate, new AddedDamageOnRepeatedHitsDelegate()},
      {DelegateId.ApplyTargetedAffixesOnHitDelegate, new ApplyTargetedAffixesOnHitDelegate()},
      {DelegateId.ChainToRandomTargetDelegate, new ChainToRandomTargetDelegate()},
      {DelegateId.ChanceToShockDelegate, new ChanceToShockDelegate()},
      {DelegateId.ApplyToAdjacentAlliesOnUseDelegate, new ApplyToAdjacentAlliesOnUseDelegate()}
    };

    public static IDelegate Get(DelegateId id) => Delegates[id];
  }
}
