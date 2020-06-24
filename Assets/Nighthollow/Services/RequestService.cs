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
    static Coroutine _ticker;
    IFn _onStartNewGame;
    IFn _onEndGame;
    IFn _onTick;
    IFn _onCardDrawn;
    IFn _onPlayedCard;
    IFn _onCreatureCollision;
    IFn _onRangedSkillFire;
    IFn _onMeleeSkillImpact;
    IFn _onProjectileImpact;
    IFn _onSkillCompleteNoHit;
    IFn _onSkillCompleteWithHit;
    IFn _onDebugCreateEnemy;

    void Start()
    {
      Arcadia.Util.require(Namespace);
      Clojure.var(Namespace, "on-connect").invoke();
      _onStartNewGame = Clojure.var(Namespace, "on-start-new-game");
      _onEndGame = Clojure.var(Namespace, "on-end-game");
      _onTick = Clojure.var(Namespace, "on-tick");
      _onCardDrawn = Clojure.var(Namespace, "on-card-drawn");
      _onPlayedCard = Clojure.var(Namespace, "on-played-card");
      _onCreatureCollision = Clojure.var(Namespace, "on-creature-collision");
      _onRangedSkillFire = Clojure.var(Namespace, "on-ranged-skill-fire");
      _onMeleeSkillImpact = Clojure.var(Namespace, "on-melee-skill-impact");
      _onProjectileImpact = Clojure.var(Namespace, "on-projectile-impact");
      _onSkillCompleteNoHit = Clojure.var(Namespace, "on-skill-complete-no-hit");
      _onSkillCompleteWithHit = Clojure.var(Namespace, "on-skill-complete-with-hit");
      _onDebugCreateEnemy = Clojure.var(Namespace, "on-debug-create-enemy");

      if (_ticker != null)
      {
        StopCoroutine(_ticker);
      }
      _ticker = StartCoroutine(RunTicker());
    }

    IEnumerator<YieldInstruction> RunTicker()
    {
      while (true)
      {
        yield return new WaitForSeconds(1);
        _onTick.invoke();
      }
      // ReSharper disable once IteratorNeverReturns
    }

    public void OnStartNewGame()
    {
      _onStartNewGame.invoke();
    }

    public void OnEndGame()
    {
      _onEndGame.invoke();
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
      _onProjectileImpact.invoke(sourceCreature.Value, hitTargetIds);
    }

    public void OnSkillCompleteWithHit(CreatureId sourceCreature, CreatureId closestEnemy, int closestEnemyDistance)
    {
      _onSkillCompleteWithHit.invoke(sourceCreature.Value, closestEnemy.Value, closestEnemyDistance);
    }

    public void OnSkillCompleteNoHit(CreatureId sourceCreature)
    {
      _onSkillCompleteNoHit.invoke(sourceCreature.Value);
    }

    public void OnDebugCreateEnemy()
    {
      _onDebugCreateEnemy.invoke();
    }
  }
}