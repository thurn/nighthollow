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

using Nighthollow.Services;

#nullable enable

namespace Nighthollow.Rules.Effects
{
  public sealed class ApplyDamageEffect : IEffect
  {
    public ApplyDamageEffect(CreatureId appliedBy, CreatureId target, int amount)
    {
      AppliedBy = appliedBy;
      Target = target;
      Amount = amount;
    }

    public Description Describe => new Description("apply damage");
    public CreatureId AppliedBy { get; }
    public CreatureId Target { get; }
    public int Amount { get; }

    public Injector AddDependencies(EffectInjector injector) => injector
      .AddDependency(Key.CreatureController);

    public void Execute(EffectInjector injector)
    {
      injector.Get(Key.CreatureController).AddDamage(AppliedBy, Target, Amount);
    }
  }
}