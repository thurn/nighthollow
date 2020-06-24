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

using System.Collections;
using System.Linq;
using Nighthollow.Model;
using UnityEngine.SceneManagement;

// ReSharper disable UnusedMember.Global

namespace Nighthollow.Services
{
  public static class Commands
  {
    public static void ResetState()
    {
      SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    public static void DrawCards(IEnumerable cards)
    {
      Root.Instance.User.Hand.DrawCards(cards.Cast<CardData>());
    }

    public static void UpdateUser(UserData userData)
    {
      Root.Instance.User.UpdateUserData(userData);
    }

    public static void UpdateCard(CardData cardData)
    {
      if (Root.Instance.User.Hand.HasCard(cardData.CardId))
      {
        Root.Instance.User.Hand.GetCard(cardData.CardId).UpdateCardData(cardData);
      }
    }

    public static void UpdateCreature(CreatureData creatureData)
    {
      if (Root.Instance.CreatureService.HasCreature(creatureData.CreatureId))
      {
        Root.Instance.CreatureService.GetCreature(creatureData.CreatureId).UpdateCreatureData(creatureData);
      }
    }

    public static void CreateEnemy(CreatureData creatureData, FileValue fileValue)
    {
      Root.Instance.CreatureService.CreateEnemyCreature(creatureData, fileValue);
    }

    public static void UseSkill(CreatureId creatureId, SkillAnimationNumber animation, SkillType skillType)
    {
      Root.Instance.CreatureService.GetCreature(creatureId).UseSkill(animation, skillType);
    }

    public static void FireProjectile(ProjectileData projectileData)
    {
      Root.Instance.CreatureService.FireProjectile(projectileData);
    }

    public static void PlayDeathAnimation(CreatureId creatureId)
    {
      Root.Instance.CreatureService.GetCreature(creatureId).PlayDeathAnimation();
    }
  }
}