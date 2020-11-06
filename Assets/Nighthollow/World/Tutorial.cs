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

using Nighthollow.Interface;
using Nighthollow.Utils;
using UnityEngine;

namespace Nighthollow.World
{
  public sealed class Tutorial : MonoBehaviour
  {
#pragma warning disable 0649
    [SerializeField] Dialog _dialogPrefab;
#pragma warning restore 0649

    static readonly string IntroText =
      "The sleeper awakes... we have been preparing for your return for many years, my lord. We will once again bring the Eternal Night to the world of the living!";

    void Start()
    {
      ShowDialog(IntroText);
    }

    void ShowDialog(string text)
    {
      ComponentUtils.Instantiate(_dialogPrefab).Initialize(text);
    }
  }
}