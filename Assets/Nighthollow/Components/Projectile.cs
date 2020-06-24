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
using System.Linq;
using Nighthollow.Model;
using Nighthollow.Services;
using Nighthollow.Utils;
using UnityEngine;

namespace Nighthollow.Components
{
  public sealed class Projectile : MonoBehaviour
  {
    [Header("Config")] [SerializeField] TimedEffect _flashEffect;
    [SerializeField] TimedEffect _hitEffect;
    ProjectileData _projectileData;

    static readonly Collider2D[] ColliderArray = Enumerable.Repeat<Collider2D>(null, 128).ToArray();

    public void Initialize(ProjectileData projectileData, Transform firingPoint)
    {
      transform.position = new Vector3(firingPoint.position.x, firingPoint.position.y, 0);
      transform.forward = Constants.ForwardDirectionForPlayer(projectileData.Owner);
      gameObject.layer = Constants.LayerForPlayerProjectiles(projectileData.Owner);

      var flash = Root.Instance.ObjectPoolService.Create(_flashEffect.gameObject, transform.position);
      flash.transform.forward = transform.forward;
      _projectileData = projectileData;
    }

    void OnValidate()
    {
      foreach (var ps in GetComponentsInChildren<ParticleSystem>())
      {
        // We give projectiles a lifetime of 3 seconds, which is approximately enough
        // time to get across the screen at a speed of 10,000.
        var main = ps.main;
        main.startLifetime = 3.0f;
        ps.GetComponent<Renderer>().sortingOrder = 500;
      }
    }

    void Update()
    {
      transform.position += (_projectileData.Speed / 1000f) * Time.deltaTime * transform.forward;
      if (Mathf.Abs(transform.position.x) > 25)
      {
        gameObject.SetActive(false);
      }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
      var hit = Root.Instance.ObjectPoolService.Create(_hitEffect.gameObject, transform.position);
      hit.transform.forward = -transform.forward;

      var hitCount = Physics2D.OverlapBoxNonAlloc(
        transform.position,
        new Vector2(_projectileData.HitboxSize / 1000f, _projectileData.HitboxSize / 1000f),
        angle: 0,
        ColliderArray,
        Constants.LayerMaskForPlayerCreatures(_projectileData.Owner.GetOpponent()));
      var hits = new List<int>(hitCount);

      for (var i = 0; i < hitCount; ++i)
      {
        hits.Add(ComponentUtils.GetComponent<Creature>(ColliderArray[i]).CreatureId.Value);
      }

      Root.Instance.RequestService.OnProjectileImpact(_projectileData.FiredBy, hits);

      gameObject.SetActive(false);
    }
  }
}