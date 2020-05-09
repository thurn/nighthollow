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

using Magewatch.Data;
using Magewatch.Services;
using Magewatch.Utils;
using UnityEngine;

namespace Magewatch.Components
{
  public sealed class AttachmentPositionSelector : MonoBehaviour
  {
    [Header("State")] [SerializeField] Card _card;
    [SerializeField] Attachment _attachment;
    [SerializeField] Creature _target;

    public void Initialize(Attachment attachment, Card card = null)
    {
      _attachment = attachment;
      attachment.SetColor(Color.gray);
      _card = card;
    }

    void Update()
    {
      var mousePosition = Root.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
      if (!_card || (mousePosition.y >= Constants.IndicatorBottomY &&
                     mousePosition.x <= Constants.IndicatorRightX))
      {
        transform.position = new Vector3(mousePosition.x, mousePosition.y + 0.25f, 0);
        var target = Root.Instance.CreatureService.GetClosestCreature(PlayerName.User, mousePosition);
        if (!target)
        {
          return; // should not happen since you should not be able to play an attachment with no creatures
        }

        if (Input.GetMouseButtonUp(0))
        {
          if (_card)
          {
            _card.OnPlayed();
          }

          target.Highlighted = false;
          _attachment.SetColor(Color.white);
          target.AddAttachment(_attachment);
          Destroy(this);
        }
        else
        {
          if (!_target || target != _target)
          {
            if (_target)
            {
              _target.Highlighted = false;
            }

            target.Highlighted = true;
            _target = target;
          }
        }
      }
      else if (_card)
      {
        if (_target)
        {
          _target.Highlighted = false;
        }

        SwitchToCard();
      }
    }

    void SwitchToCard()
    {
      _card.gameObject.SetActive(true);
      _card.transform.position = Input.mousePosition;
      Destroy(gameObject);
    }
  }
}