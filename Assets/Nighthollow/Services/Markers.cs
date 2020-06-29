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

using Unity.Profiling;

namespace Nighthollow.Services
{
  public sealed class Markers
  {
    public static readonly ProfilerMarker Unknown = new ProfilerMarker("Unknown");

    public static readonly ProfilerMarker Dispatch = new ProfilerMarker("Dispatch");
    public static readonly ProfilerMarker ApplyEffects = new ProfilerMarker("ApplyEffects");
    public static readonly ProfilerMarker ApplyEventCommands = new ProfilerMarker("ApplyEventCommands");
    public static readonly ProfilerMarker EventCommand = new ProfilerMarker("EventCommand");
    public static readonly ProfilerMarker UpdateGameObjects = new ProfilerMarker("UpdateGameObjects");
    public static readonly ProfilerMarker DrawCards = new ProfilerMarker("DrawCards");
    public static readonly ProfilerMarker SetCanPlayCard = new ProfilerMarker("SetCanPlayCard");
    public static readonly ProfilerMarker PlayCreature = new ProfilerMarker("PlayCreature");
    public static readonly ProfilerMarker CreateEnemy = new ProfilerMarker("CreateEnemy");
    public static readonly ProfilerMarker CreateRandomEnemy = new ProfilerMarker("CreateRandomEnemy");
    public static readonly ProfilerMarker UseSkill = new ProfilerMarker("UseSkill");
    public static readonly ProfilerMarker ClearCurrentSkill = new ProfilerMarker("ClearCurrentSkill");
    public static readonly ProfilerMarker MutateCreature = new ProfilerMarker("MutateCreature");
    public static readonly ProfilerMarker MutateUser = new ProfilerMarker("MutateUser");
    public static readonly ProfilerMarker FireProjectile = new ProfilerMarker("FireProjectile");
    public static readonly ProfilerMarker ApplyStartDrawingCards = new ProfilerMarker("ApplyStartDrawingCards");
    public static readonly ProfilerMarker ApplyCreateEnemy = new ProfilerMarker("ApplyCreateEnemy");
    public static readonly ProfilerMarker ApplyUseSkill = new ProfilerMarker("ApplyUseSkill");
    public static readonly ProfilerMarker ApplyFireProjectile = new ProfilerMarker("ApplyFireProjectile");
    public static readonly ProfilerMarker ApplyDeathAnimation = new ProfilerMarker("ApplyDeathAnimation");
    public static readonly ProfilerMarker DefaultMarker = new ProfilerMarker("DefaultMarker");

    public static readonly ProfilerMarker GameStart = new ProfilerMarker("GameStart");
    public static readonly ProfilerMarker StartDrawingCards = new ProfilerMarker("StartDrawingCards");
    public static readonly ProfilerMarker CardDrawn = new ProfilerMarker("CardDrawn");
    public static readonly ProfilerMarker CardPlayed = new ProfilerMarker("CardPlayed");
    public static readonly ProfilerMarker CreaturePlayed = new ProfilerMarker("CreaturePlayed");
    public static readonly ProfilerMarker EnemyAppeared = new ProfilerMarker("EnemyAppeared");
    public static readonly ProfilerMarker CardMutated = new ProfilerMarker("CardMutated");
    public static readonly ProfilerMarker CreatureCollision = new ProfilerMarker("CreatureCollision");
    public static readonly ProfilerMarker RangedSkillFire = new ProfilerMarker("RangedSkillFire");
    public static readonly ProfilerMarker MeleeSkillImpact = new ProfilerMarker("MeleeSkillImpact");
    public static readonly ProfilerMarker ProjectileImpact = new ProfilerMarker("ProjectileImpact");
    public static readonly ProfilerMarker SkillComplete = new ProfilerMarker("SkillComplete");
    public static readonly ProfilerMarker CreatureKilled = new ProfilerMarker("CreatureKilled");
    public static readonly ProfilerMarker CreatureMutated = new ProfilerMarker("CreatureMutated");
    public static readonly ProfilerMarker UserMutated = new ProfilerMarker("UserMutated");
    public static readonly ProfilerMarker Tick = new ProfilerMarker("Tick");

    public static readonly ProfilerMarker FireProjectileInternal = new ProfilerMarker("FireProjectileInternal");
    public static readonly ProfilerMarker UseSkillInternal = new ProfilerMarker("UseSkillInternal");



  }
}