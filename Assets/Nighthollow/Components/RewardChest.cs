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
using UnityEngine;

#nullable enable

namespace Nighthollow.Components
{
  public sealed class RewardChest : MonoBehaviour
  {
    static readonly int OpenTrigger = Animator.StringToHash("Open");

    [SerializeField] TimedEffect _onOpenEffect = null!;
    Animator _animator = null!;

    void Start()
    {
      _animator = GetComponent<Animator>();
    }

    void OnMouseDown()
    {
      StartCoroutine(Play());
    }

    IEnumerator<YieldInstruction> Play()
    {
      _animator.SetTrigger(OpenTrigger);
      yield return new WaitForSeconds(seconds: 0.2f);
      var effect = Instantiate(_onOpenEffect.gameObject);
      effect.transform.position = transform.position;
    }
  }
}
