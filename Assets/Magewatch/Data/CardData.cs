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
using System.Collections.Generic;

namespace Magewatch.Data
{
  [Serializable]
  public sealed class CardData
  {
    public int CardId;
    public Asset Prefab;
    public string Name;
    public bool NoCost;
    public int ManaCost;
    public List<Influence> InfluenceCost;
    public PlayerName Owner;
    public Asset Image;
    public string Text;
    public bool IsRevealed;
    public bool CanBePlayed;
    public bool Untargeted;
    public CreatureData CreatureData;
    public AttachmentData AttachmentData;
  }
}