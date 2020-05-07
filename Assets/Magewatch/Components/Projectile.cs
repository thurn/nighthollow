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
using Magewatch.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Magewatch.Components
{
  public sealed class Projectile : MonoBehaviour
  {
    [Header("Config")] [SerializeField] TimedEffect _flashEffect;
    [SerializeField] TimedEffect _hitEffect;
    [SerializeField] float _speed;
    [Header("State")] Transform _target;
    Action<Creature> _onHit;

    public void Initialize(Transform firingPoint, Transform target, Action<Creature> onHit)
    {
      _target = target;
      _onHit = onHit;
      transform.position = firingPoint.position;
      transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);

      var flash = Instantiate(_flashEffect);
      flash.transform.position = transform.position;
      flash.transform.forward = transform.forward;
    }

    void Update()
    {
      transform.position += _speed * Time.deltaTime * transform.forward;
    }

    void OnTriggerEnter(Collider other)
    {
      if (other.transform == _target)
      {
        var hit = Instantiate(_hitEffect);
        hit.transform.position = transform.position;
        hit.transform.forward = -transform.forward;
        // _onHit(ComponentUtils.GetComponent<Creature>(other.gameObject));
        gameObject.SetActive(false);
      }
      else
      {
        Debug.Log($"Projectile::OnTriggerEnter> Skipping {other.name}, not the target");
      }
    }
  }
}