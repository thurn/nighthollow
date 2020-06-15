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
using UnityEngine;

namespace Nighthollow.Services
{
  public sealed class CommandService : MonoBehaviour
  {
    public void ResetState()
    {
      Root.Instance.User.Hand.DestroyAllCards();
      Root.Instance.CreatureService.DestroyAllCreatures();
    }

    public void DrawCards(IEnumerable cards)
    {
      Root.Instance.User.Hand.DrawCards(cards.Cast<CardData>());
    }

    public void UpdateCard(CardData cardData)
    {
      Root.Instance.User.Hand.GetCard(cardData.CardId).UpdateCardData(cardData);
    }

    public void UpdateCreature(CreatureData creatureData)
    {
      Root.Instance.CreatureService.GetCreature(creatureData.CreatureId).UpdateCreatureData(creatureData);
    }
  }
}