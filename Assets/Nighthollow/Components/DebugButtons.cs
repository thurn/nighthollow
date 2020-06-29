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

using Nighthollow.Model;
using Nighthollow.Services;
using UnityEngine;

// ReSharper disable UnusedMember.Global

namespace Nighthollow.Components
{
  public sealed class DebugButtons : MonoBehaviour
  {
    int _idCounter;

    public void HideButtons()
    {
      gameObject.SetActive(false);
    }

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

    public void RunPerftest()
    {
      Commands.DisableAssertions();
      Root.Instance.RequestService.OnRunPerftest();
    }

    public void ResetGame()
    {
      Root.Instance.RequestService.OnEndGame();
      Commands.ResetState();
    }

    public void Enemy()
    {
      Root.Instance.RequestService.OnDebugCreateEnemy();
    }

    public void FireProjectile()
    {
      var projectile = new ProjectileData(
        new AssetData("Projectiles/Projectile 1", AssetType.Prefab),
        new CreatureId(2),
        PlayerName.User,
        10000,
        1000);
      Root.Instance.CreatureService.FireProjectile(projectile);
    }
  }
}