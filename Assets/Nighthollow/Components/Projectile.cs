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

using Nighthollow.Delegates.SkillDelegates;
using Nighthollow.Generated;
using Nighthollow.Model;
using Nighthollow.Services;
using Nighthollow.Utils;
using UnityEngine;

namespace Nighthollow.Components
{
  public sealed class Projectile : MonoBehaviour
  {
    [Header("Config")]
    [SerializeField] TimedEffect _flashEffect;
    [SerializeField] TimedEffect _hitEffect;

    [Header("State")]
    [SerializeField] Creature _firedBy;
    [SerializeField] SkillData _skillData;
    [SerializeField] Collider2D _collider;

    public SkillData Data => _skillData;

    public void Initialize(
      Creature firedBy,
      SkillData skillData,
      Vector2 firingPosition,
      Vector2? direction = null)
    {
      _firedBy = firedBy;
      transform.position = firingPosition;
      transform.forward = direction ?? Constants.ForwardDirectionForPlayer(firedBy.Owner);
      gameObject.layer = Constants.LayerForProjectiles(firedBy.Owner);

      _collider = GetComponent<Collider2D>();

      var flash = Root.Instance.ObjectPoolService.Create(_flashEffect.gameObject, transform.position);
      flash.transform.forward = transform.forward;
      _skillData = skillData;
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

    public void SetReferences(TimedEffect flashEffect, TimedEffect hitEffect)
    {
      _flashEffect = flashEffect;
      _hitEffect = hitEffect;
    }

    void Update()
    {
      transform.position += (_skillData.GetInt(Stat.Speed)/ 1000f) * Time.deltaTime * transform.forward;

      if (Mathf.Abs(transform.position.x) > 25)
      {
        gameObject.SetActive(false);
      }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
      var hit = Root.Instance.ObjectPoolService.Create(_hitEffect.gameObject, transform.position);
      hit.transform.forward = -transform.forward;
      _skillData.Delegate.OnImpact(new SkillContext(_firedBy, _skillData));
      gameObject.SetActive(false);
    }
  }
}
