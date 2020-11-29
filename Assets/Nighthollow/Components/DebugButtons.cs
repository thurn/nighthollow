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

using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable

namespace Nighthollow.Components
{
  public sealed class DebugButtons : MonoBehaviour
  {
    public void HideButtons()
    {
      gameObject.SetActive(value: false);
    }

    public void Slow()
    {
      Time.timeScale = 0.05f;
    }

    public void Fast()
    {
      Time.timeScale = 1.0f;
    }

    public void StartGame()
    {
      Debug.Log("Start Game");
    }

    public void ResetGame()
    {
      SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void Draft()
    {
    }

    public void DeckBuilder()
    {
    }
  }
}
