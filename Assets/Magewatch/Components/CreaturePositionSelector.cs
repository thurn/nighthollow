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

using Magewatch.Services;
using Magewatch.Utils;
using UnityEngine;

namespace Magewatch.Components
{
  public sealed class CreaturePositionSelector : MonoBehaviour
  {
    [SerializeField] Card _card;
    [SerializeField] Creature _creature;

    public void Initialize(Card card, Creature creature)
    {
      _card = Errors.CheckNotNull(card);
      _creature = creature;
      _card.gameObject.SetActive(false);
      _creature.AnimationPaused = true;

      foreach (var spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
      {
        spriteRenderer.color = Color.gray;
      }
    }

    void Update()
    {
      var mousePosition = Root.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
      if (mousePosition.y >= Constants.IndicatorBottomY &&
          mousePosition.x <= Constants.IndicatorRightX)
      {
        transform.position = new Vector3(mousePosition.x, mousePosition.y - 0.5f, 0);
      }
      else
      {
        SwitchToCard();
      }
    }

    void SwitchToCard()
    {
      _card.gameObject.SetActive(true);
      _card.transform.position = Input.mousePosition;

      _creature.Destroy();
    }
  }
}