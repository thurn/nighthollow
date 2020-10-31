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

namespace Nighthollow.Delegates.Core
{
  public interface ICreatureDelegate : ICreatureOrSkillDelegate<CreatureContext>
  {
    /// <summary>
    /// Should check if the creature could currently hit with a melee skill. Will return true if any delegate returns
    /// a true value.
    /// </summary>
    bool MeleeCouldHit(CreatureContext c);

    /// <summary>
    /// Called to check if a projectile fired by this creature would currently hit a target. Will return true if
    /// any delegate returns a true value.
    /// </summary>
    bool ProjectileCouldHit(CreatureContext c);

    /// <summary>
    /// Called when a creature wants to decide which skill to use. Should return null if there is no appropriate skill
    /// available.
    /// </summary>
    SkillData? SelectSkill(CreatureContext c);
  }
}