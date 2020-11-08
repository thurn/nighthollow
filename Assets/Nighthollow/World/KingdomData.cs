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

using System;
using UnityEngine;
using UnityEngine.Tilemaps;

#nullable enable

namespace Nighthollow.World
{
  [Serializable]
  public sealed class KingdomData
  {
#pragma warning disable 0649
    [SerializeField] string _name = null!;
    public string Name => _name;

    [SerializeField] Vector2Int _startingLocation = default;
    public Vector2Int StartingLocation => _startingLocation;

    [SerializeField] Color _borderColor;
    public Color BorderColor => _borderColor;

    [SerializeField] Tile _icon = null!;
    public Tile Icon => _icon;
#pragma warning restore 0649
  }
}
