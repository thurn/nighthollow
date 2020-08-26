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

namespace Nighthollow.Data
{
  [CreateAssetMenu(menuName = "Data/User")]
  public sealed class UserData : ScriptableObject
  {
    [SerializeField] Stat _startingLife;
    public Stat StartingLife => _startingLife;

    [SerializeField] Stat _startingMana;
    public Stat StartingMana => _startingMana;

    [SerializeField] Influence _influence;
    public Influence Influence => _influence;

    [SerializeField] Stat _startingHandSize;
    public Stat StartingHandSize => _startingHandSize;

    [SerializeField] Stat _manaGain;
    public Stat ManaGain => _manaGain;

    [SerializeField] Stat _manaGainIntervalMs;
    public Stat ManaGainIntervalMs => _manaGainIntervalMs;

    [SerializeField] Stat _cardDrawIntervalMs;
    public Stat CardDrawIntervalMs => _cardDrawIntervalMs;
  }
}