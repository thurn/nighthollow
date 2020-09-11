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

using Nighthollow.Data;
using Nighthollow.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nighthollow.Components
{
  public sealed class RewardChoice : MonoBehaviour
  {
    [SerializeField] Image _image;
    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] CardItemData _card;
    [SerializeField] RewardSelector _rewardSelector;

    public void Initialize(CardItemData card, RewardSelector rewardSelector)
    {
      _image.sprite = card.BaseCard.Image;
      _text.text = DescriptiveTextHelper.TextForCardItem(card);
      _card = card;
      _rewardSelector = rewardSelector;
    }

    public void OnSelected()
    {
      Root.Instance.User.Inventory.AddCard(_card);
      Destroy(_rewardSelector.gameObject);
    }
  }
}