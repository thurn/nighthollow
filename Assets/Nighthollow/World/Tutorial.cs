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

#nullable enable

using System.Collections.Generic;
using Nighthollow.Interface;
using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Nighthollow.World
{
  public sealed class Tutorial : MonoBehaviour
  {
#pragma warning disable 0649
    [SerializeField] Dialog _dialogPrefab = null!;
    [SerializeField] bool _showIntroduction;
    [SerializeField] WorldMap _worldMap = null!;
    [SerializeField] Tile _fightIcon = null!;
#pragma warning restore 0649

    static readonly Color BorderColor = new Color(0.498f, 0f, 0f);
    static readonly Vector2Int StartingHex = new Vector2Int(-12, 7);
    static readonly Vector2Int AttackHex = new Vector2Int(-11, 7);

    const string IntroText =
      "The sleeper awakes... we have been preparing for your return for many years, my lord. We will once again bring the Eternal Night to the world of the living!";

    void Start()
    {
      _worldMap.Initialize();

      if (_showIntroduction)
      {
        ShowDialog(IntroText);
      }
    }

    void ShowDialog(string text)
    {
      ComponentUtils.Instantiate(_dialogPrefab).Initialize(text, () =>
      {
        _worldMap.ShowIcon(AttackHex, _fightIcon);
      });
    }
  }
}
