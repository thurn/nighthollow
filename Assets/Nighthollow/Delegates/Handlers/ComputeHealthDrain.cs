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

using Nighthollow.Data;

#nullable enable

namespace Nighthollow.Delegates.Handlers
{
  public interface IComputeHealthDrain : IHandler
  {
    public sealed class Data : QueryData<IComputeHealthDrain, int>
    {
      public Data(CreatureState self, SkillData skill, CreatureState target, int totalDamage)
      {
        Self = self;
        Skill = skill;
        Target = target;
        TotalDamage = totalDamage;
      }

      public override int Invoke(DelegateContext c, IComputeHealthDrain handler) =>
        handler.ComputeHealthDrain(c, this);

      public CreatureState Self { get; }
      public SkillData Skill { get; }
      public CreatureState Target { get; }
      public int TotalDamage { get; }
    }

    /// <summary>Computes health drain to apply for a hit on a target</summary>
    int ComputeHealthDrain(DelegateContext c, Data d);
  }
}