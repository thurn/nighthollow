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

using System.Collections.Generic;
using DG.Tweening;
using Nighthollow.Data;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public sealed class RewardsWindow : HideableElement<RewardsWindow.Args>
  {
    const float AnimationOneDuration = 1f;
    const float AnimationOnePause = 1.5f;
    const float AnimationTwoDuration = 1f;
    const Ease AnimationOneEase = Ease.OutBounce;
    const Ease AnimationTwoEase = Ease.OutCirc;
    const float SequenceDelaySeconds = 0.1f;

    VisualElement _content = null!;

    public readonly struct Args
    {
      public readonly IReadOnlyList<CreatureItemData> Items;

      public Args(IReadOnlyList<CreatureItemData> items)
      {
        Items = items;
      }
    }

    public new sealed class UxmlFactory : UxmlFactory<RewardsWindow, UxmlTraits>
    {
    }

    protected override void Initialize()
    {
      _content = FindElement("Content");
    }

    protected override void OnShow(Args argument)
    {
      ItemRenderer.AddItems(Controller, _content, argument.Items,
        new ItemRenderer.Config(count: 20, shouldAddTooltip: false));
      RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    List<ItemImage> RewardImages()
    {
      var result = new List<ItemImage>();
      this.Query().Class("card").Children<ItemImage>().ForEach(e => result.Add(e));
      return result;
    }

    public void AnimateOpening()
    {
      var images = RewardImages();
      var sequence = DOTween.Sequence();
      for (var i = 0; i < images.Count; ++i)
      {
        var element = images[i];
        var width = element.worldBound.width;
        var height = element.worldBound.height;
        element.style.width = new StyleLength(width / 2f);
        element.style.height = new StyleLength(height / 2f);

        var position = new Vector3(element.style.left.value.value, element.style.top.value.value, z: 0);
        // Interpolate target position along a circle
        var lerp = Quaternion.AngleAxis(
          Mathf.Lerp(a: 0f, b: 360f, i / (float) images.Count + 0.1f),
          Vector3.forward) * Vector3.up;
        var target = position + lerp * 300;
        var delay = SequenceDelaySeconds * i;

        sequence
          .InsertCallback(delay, () => { element.style.visibility = new StyleEnum<Visibility>(Visibility.Visible); })
          .Insert(delay,
            DOTween.To(
                () => element.style.left.value.value,
                left => element.style.left = left,
                target.x,
                AnimationOneDuration)
              .SetEase(AnimationOneEase))
          .Insert(delay,
            DOTween.To(
                () => element.style.top.value.value,
                top => element.style.top = top,
                target.y,
                AnimationOneDuration)
              .SetEase(AnimationOneEase))
          .Insert(delay,
            DOTween.To(
                () => element.style.width.value.value,
                w => element.style.width = w,
                width * 2f,
                AnimationOneDuration)
              .SetEase(AnimationOneEase))
          .Insert(delay,
            DOTween.To(
                () => element.style.height.value.value,
                h => element.style.height = h,
                height * 2f,
                AnimationOneDuration)
              .SetEase(AnimationOneEase))
          .Insert(AnimationOneDuration + AnimationOnePause + delay,
            DOTween.To(
                () => element.style.left.value.value,
                left => element.style.left = left,
                endValue: 16f,
                AnimationTwoDuration)
              .SetEase(AnimationTwoEase))
          .Insert(AnimationOneDuration + AnimationOnePause + delay,
            DOTween.To(
                () => element.style.top.value.value,
                top => element.style.top = top,
                endValue: 16f,
                AnimationTwoDuration)
              .SetEase(AnimationTwoEase))
          .Insert(AnimationOneDuration + AnimationOnePause + delay,
            DOTween.To(
                () => element.style.width.value.value,
                w => element.style.width = new StyleLength(w),
                width,
                AnimationTwoDuration)
              .SetEase(AnimationTwoEase))
          .Insert(AnimationOneDuration + AnimationOnePause + delay,
            DOTween.To(
                () => element.style.height.value.value,
                h => element.style.height = new StyleLength(h),
                height,
                AnimationTwoDuration)
              .SetEase(AnimationTwoEase))
          .AppendCallback(() =>
          {
            foreach (var image in RewardImages())
            {
              image.AddTooltip();
            }
          });
      }

      sequence.Play();
    }

    void OnGeometryChange(GeometryChangedEvent evt)
    {
      foreach (var image in RewardImages())
      {
        SetStartingPosition(image);
      }

      UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    static void SetStartingPosition(VisualElement element)
    {
      var target = new Vector2(x: 1400, y: 425);
      var converted = element.WorldToLocal(target);

      element.style.position = new StyleEnum<Position>(Position.Absolute);
      element.style.left = new StyleLength(converted.x);
      element.style.top = new StyleLength(converted.y);
      element.style.visibility = new StyleEnum<Visibility>(Visibility.Hidden);
    }
  }
}
