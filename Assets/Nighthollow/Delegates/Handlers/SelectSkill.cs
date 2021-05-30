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
using Nighthollow.Services;

#nullable enable

namespace Nighthollow.Delegates.Handlers
{
  public interface ISelectSkill : IHandler
  {
    public sealed class Data : QueryData<ISelectSkill, SkillData?>
    {
      public Data(CreatureId self)
      {
        Self = self;
      }

      public override SkillData? Invoke(IGameContext c, ISelectSkill handler) =>
        handler.SelectSkill(c, this);

      public CreatureId Self { get; }
    }

    /// <summary>
    /// Called when a creature wants to decide which skill to use. The first delegate to return a non-null value is
    /// used.
    /// </summary>
    SkillData? SelectSkill(IGameContext context, Data data);
  }
}