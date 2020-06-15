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
using Nighthollow.Model;
using Nighthollow.Services;
using UnityEngine;

// ReSharper disable UnusedMember.Global

namespace Nighthollow.Components
{
  public sealed class DebugButtons : MonoBehaviour
  {
    int _idCounter;

    public void Slow()
    {
      Time.timeScale = 0.1f;
    }

    public void Fast()
    {
      Time.timeScale = 1.0f;
    }

    public void StartGame()
    {
      Root.Instance.RequestService.OnStartNewGame();
    }

    public void ResetGame()
    {
      Commands.ResetState();
    }

    public void Draw()
    {
      var card = Wizard();
      Root.Instance.User.Hand.DrawCards(new List<CardData> { card, card, card });
    }

    public void Enemy()
    {
      Root.Instance.CreatureService.CreateEnemyCreature(Viking(), FileValue.File3);
    }

    CardData Wizard() => new CardData(
      cardId: new CardId(_idCounter++),
      prefab: Prefab("Content/Card"),
      canPlay: true,
      cost: Cost(100, School.Flame, 1),
      image: Sprite("CreatureImages/Wizard"),
      creatureData: new CreatureData
      (
        creatureId: new CreatureId(_idCounter++),
        prefab: Prefab("Creatures/Player/Wizard"),
        owner: PlayerName.User,
        speed: 0,
        attachments: new List<AttachmentData>()
      )
    );

    CreatureData Viking() => new CreatureData
    (
      creatureId: new CreatureId(_idCounter++),
      prefab: Prefab("Creatures/Enemy/Viking"),
      owner: PlayerName.Enemy,
      speed: 2000,
      attachments: new List<AttachmentData>()
    );

    AssetData Prefab(string address) => new AssetData(address, AssetType.Prefab);

    AssetData Sprite(string address) => new AssetData(address, AssetType.Sprite);

    Cost Cost(int mana, School school, int influence) => new Cost(
      mana,
      new List<Influence>
      {
        new Influence(school, influence)
      });
  }
}