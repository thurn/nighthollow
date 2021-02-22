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
using UnityEngine;

#nullable enable

namespace Nighthollow.Delegates.Handlers
{
  public interface IGetCollider : IHandler
  {
    public sealed class Data : QueryData<IGetCollider, Collider2D>
    {
      public Data(CreatureState self, SkillData skill)
      {
        Self = self;
        Skill = skill;
      }

      public override Collider2D Invoke(DelegateContext c, IGetCollider handler) =>
        handler.GetCollider(c, this);

      public CreatureState Self { get; }
      public SkillData Skill { get; }
    }

    /// <summary>Returns the collider to use for hit-testing this skill's impact.</summary>
    Collider2D GetCollider(DelegateContext c, Data d);
  }
}