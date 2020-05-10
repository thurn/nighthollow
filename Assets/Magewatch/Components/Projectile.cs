// Copyright The Magewatch Project

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//    https://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Magewatch.Data;
using Magewatch.Services;
using Magewatch.Utils;
using UnityEngine;

namespace Magewatch.Components
{
  public sealed class Projectile : MonoBehaviour
  {
    [Header("Config")] [SerializeField] TimedEffect _flashEffect;
    [SerializeField] TimedEffect _hitEffect;
    [SerializeField] float _speed;

    [Header("State")] Collider2D _target;
    Action _onHit;

    public static void Instantiate(FireProjectileEffect fireProjectile, Transform firingPoint, Collider2D target,
      Action onHit)
    {
      var projectile = Root.Instance.ObjectPoolService.Instantiate(fireProjectile.Prefab, firingPoint.position);
      ComponentUtils.GetComponent<Projectile>(projectile).Initialize(firingPoint, target, onHit);
    }

    void Initialize(Transform firingPoint, Collider2D target, Action onHit)
    {
      _target = target;
      _onHit = onHit;
      transform.position = new Vector3(firingPoint.position.x, firingPoint.position.y, 0);
      transform.rotation = Quaternion.LookRotation(target.bounds.center - transform.position, Vector3.up);

      var flash = Root.Instance.ObjectPoolService.Instantiate(_flashEffect.gameObject, transform.position);
      flash.transform.forward = transform.forward;
    }

    void OnValidate()
    {
      foreach (var ps in GetComponentsInChildren<ParticleSystem>())
      {
        ps.GetComponent<Renderer>().sortingOrder = 500;
      }
    }

    void Update()
    {
      transform.position += _speed * Time.deltaTime * transform.forward;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
      if (other == _target)
      {
        var hit = Root.Instance.ObjectPoolService.Instantiate(_hitEffect.gameObject, transform.position);
        hit.transform.forward = -transform.forward;
        _onHit?.Invoke();
        gameObject.SetActive(false);
      }
      else
      {
        Debug.Log($"Projectile::OnTriggerEnter> Skipping {other.name}, not the target");
      }
    }
  }
}