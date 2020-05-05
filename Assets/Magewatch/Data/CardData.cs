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
using UnityEngine;

namespace Magewatch.Data
{
  [Serializable]
  public sealed class CardData
  {
    public int CardId;
    public Asset<GameObject> Prefab;
    public string Name;
    public int ManaCost;
    public Influence Influence;
    public PlayerName Owner;
    public Asset<Sprite> Image;
    public string Text;
    public bool IsRevealed;
    public bool CanBePlayed;
    public CreatureData CreatureData;
  }
}