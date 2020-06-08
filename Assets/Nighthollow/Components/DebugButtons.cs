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

    public void Draw()
    {
      var card = Wizard();
      Root.Instance.AssetService.FetchAssets(card, () =>
      {
        Root.Instance.GetPlayer(PlayerName.User).Hand.DrawOrUpdateCard(card);
      });
    }

    CardData Wizard() => new CardData
    {
      CardId = new CardId(_idCounter++),
      Prefab = Prefab("Content/Card"),
      StandardCost = Cost(50, School.Flame, 1),
      Image = Sprite("CreatureImages/Wizard"),
      CanBePlayed = true,
      CreatureData = new CreatureData
      {
        CreatureId = new CreatureId(_idCounter++),
        Prefab = Prefab("Creatures/Wizard"),
        Owner = PlayerName.User,
      }
    };

    Asset Prefab(string address) => new Asset
    {
      Address = address,
      AssetType = AssetType.Prefab
    };

    Asset Sprite(string address) => new Asset
    {
      Address = address,
      AssetType = AssetType.Sprite
    };

    StandardCost Cost(int mana, School school, int influence) => new StandardCost
    {
      ManaCost = 50,
      InfluenceCost = new List<Influence>
      {
        new Influence
        {
          School = school,
          Value = influence
        }
      }
    };
  }
}