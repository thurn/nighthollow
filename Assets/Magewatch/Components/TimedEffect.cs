// Copyright 2020 © Derek Thurn

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
using UnityEngine;

namespace Magewatch.Components
{
  public sealed class TimedEffect : MonoBehaviour
  {
    [Header("Config")] [SerializeField] float _duration;

    void OnEnable()
    {
      StartCoroutine(DisableAsync(_duration));
    }

    void OnValidate()
    {
      _duration = 0.0f;

      foreach (var system in GetComponentsInChildren<ParticleSystem>())
      {
        var main = system.main;
        _duration = Mathf.Max(_duration, main.duration + main.startLifetime.constantMax);
      }

      foreach (var audioSource in GetComponentsInChildren<AudioSource>())
      {
        if (audioSource.clip != null)
        {
          _duration = Mathf.Max(_duration, audioSource.clip.length);
        }
      }

      foreach (var ps in GetComponentsInChildren<ParticleSystem>())
      {
        ps.GetComponent<Renderer>().sortingOrder = 500;
      }
    }

    IEnumerator<YieldInstruction> DisableAsync(float duration)
    {
      // Add a little extra time for safety :)
      yield return new WaitForSeconds(duration + 0.5f);
      gameObject.SetActive(false);
    }
  }
}