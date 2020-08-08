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
using Nighthollow.Components;
using Nighthollow.Data;
using UnityEngine;
using Bolt;

namespace Nighthollow.Services
{
  public sealed class EventService : MonoBehaviour
  {
    public void OnStartGame()
    {
      CustomEvent.Trigger(Root.Instance.User.gameObject, "StartNewGame");
    }

    public void OnEndGame()
    {
      CustomEvent.Trigger(Root.Instance.User.gameObject, "EndGame");
    }

    public void OnCardDrawn(Card card)
    {
      CustomEvent.Trigger(Root.Instance.User.gameObject, "CardDrawn", card);
      CustomEvent.Trigger(card.gameObject, "CardDrawn");
    }

    public void OnPlayedCard(Card card, RankValue targetRank, FileValue targetFile)
    {
      CustomEvent.Trigger(Root.Instance.User.gameObject, "PlayedCard", targetRank, targetFile);
      CustomEvent.Trigger(card.gameObject, "PlayedCard", targetRank, targetFile);
    }

    public void OnCreatureCollision(Creature source, Creature other)
    {
      CustomEvent.Trigger(source.gameObject, "CreatureCollision", other);
    }

    public void OnRangedSkillFire(Creature sourceCreature)
    {
      CustomEvent.Trigger(sourceCreature.gameObject, "RangedSkillFire");
    }

    public void OnMeleeSkillImpact(Creature sourceCreature, List<Creature> hitTargets)
    {
      CustomEvent.Trigger(sourceCreature.gameObject, "MeleeSkillImpact", hitTargets);
    }

    public void OnProjectileImpact(Creature sourceCreature, List<Creature> hitTargets)
    {
      CustomEvent.Trigger(sourceCreature.gameObject, "ProjectileImpact", hitTargets);
    }

    public void OnSkillCompleteWithHit(Creature sourceCreature, Creature closestEnemy, int closestEnemyDistance)
    {
      CustomEvent.Trigger(sourceCreature.gameObject, "SkillCompleteWithHit", closestEnemy, closestEnemyDistance);
    }

    public void OnSkillCompleteNoHit(Creature sourceCreature)
    {
      CustomEvent.Trigger(sourceCreature.gameObject, "SkillCompleteNoHit");
    }
  }
}