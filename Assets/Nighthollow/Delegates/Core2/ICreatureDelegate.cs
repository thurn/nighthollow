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

using Nighthollow.Data;

namespace Nighthollow.Delegates.Core2
{
  public interface ICreatureDelegate : ICreatureEventsDelegate<CreatureContext>
  {
    /// <summary>
    /// Should return true if the creature could currently use a melee skill.
    /// </summary>
    bool CanUseMeleeSkill(CreatureContext c);

    /// <summary>
    /// Should return true if the creature could currently use a projectile skill.
    /// </summary>
    bool CanUseProjectileSkill(CreatureContext c);

    /// <summary>
    /// Called when a creature wants to decide which skill to use. Should return null if there is no appropriate skill
    /// available.
    /// </summary>
    SkillData? SelectSkill(CreatureContext c);
  }
}