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

namespace Nighthollow.Services
{
  public interface IStartCoroutine
  {
    Coroutine StartCoroutine(IEnumerator<YieldInstruction> routine);
  }

  public abstract class AbstractInitializer : MonoBehaviour, IStartCoroutine
  {
    void OnEnable()
    {
      // If you load into a scene from an editor window (e.g. by invoking a scenario), timeScale could be 0.
      Time.timeScale = 1f;
    }

    public Coroutine StartCoroutine(IEnumerator<YieldInstruction> routine) => base.StartCoroutine(routine);
  }
}