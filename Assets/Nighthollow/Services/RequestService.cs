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
using clojure.clr.api;
using clojure.lang;
using Nighthollow.Model;
using UnityEngine;

namespace Nighthollow.Services
{
  public sealed class RequestService : MonoBehaviour
  {
    const string Namespace = "nighthollow.main";
    IFn _onStartNewGame;
    IFn _onCardDrawn;
    IFn _onPlayedCard;
    IFn _onCreatureCollision;
    IFn _onRangedSkillFire;
    IFn _onMeleeSkillImpact;
    IFn _onProjectileImpact;
    IFn _onSkillComplete;
    IFn _onDebugCreateEnemy;

    void Start()
    {
      Arcadia.Util.require(Namespace);
      Clojure.var(Namespace, "on-connect").invoke();
      _onStartNewGame = Clojure.var(Namespace, "on-start-new-game");
      _onCardDrawn = Clojure.var(Namespace, "on-card-drawn");
      _onPlayedCard = Clojure.var(Namespace, "on-played-card");
      _onCreatureCollision = Clojure.var(Namespace, "on-creature-collision");
      _onRangedSkillFire = Clojure.var(Namespace, "on-ranged-skill-fire");
      _onMeleeSkillImpact = Clojure.var(Namespace, "on-melee-skill-impact");
      _onProjectileImpact = Clojure.var(Namespace, "on-projectile-impact");
      _onSkillComplete = Clojure.var(Namespace, "on-skill-complete");
      _onDebugCreateEnemy = Clojure.var(Namespace, "on-debug-create-enemy");
    }

    public void OnStartNewGame()
    {
      _onStartNewGame.invoke();
    }

    public void OnCardDrawn(CardId cardId)
    {
      _onCardDrawn.invoke(cardId.Value);
    }

    public void OnPlayedCard(CardId cardId, RankValue targetRank, FileValue targetFile)
    {
      _onPlayedCard.invoke(cardId.Value, targetRank, targetFile);
    }

    public void OnCreatureCollision(CreatureId source, CreatureId other)
    {
      _onCreatureCollision.invoke(source.Value, other.Value);
    }

    public void OnRangedSkillFire(CreatureId sourceCreature)
    {
      _onRangedSkillFire.invoke(sourceCreature.Value);
    }

    public void OnMeleeSkillImpact(CreatureId sourceCreature, List<int> hitTargetIds)
    {
      _onMeleeSkillImpact.invoke(sourceCreature.Value, hitTargetIds);
    }

    public void OnProjectileImpact(CreatureId sourceCreature, List<int> hitTargetIds)
    {
      _onProjectileImpact.invoke(sourceCreature, hitTargetIds);
    }

    public void OnSkillComplete(CreatureId sourceCreature, bool hasMeleeCollision)
    {
      _onSkillComplete.invoke(sourceCreature.Value, hasMeleeCollision);
    }

    public void OnDebugCreateEnemy()
    {
      _onDebugCreateEnemy.invoke();
    }
  }
}