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
using System.Collections.Generic;
using UnityEngine;

namespace Nighthollow.Data
{
  [Serializable]
  public class CardWithWeight
  {
    public CardData Card;
    public int Weight;
  }

  [CreateAssetMenu(menuName = "Data/Deck")]
  public class DeckData : ScriptableObject
  {
    public List<CardWithWeight> Cards;
  }
}