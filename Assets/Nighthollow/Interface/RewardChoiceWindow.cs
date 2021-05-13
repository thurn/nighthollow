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
using System.Linq;
using DG.Tweening;
using Nighthollow.Data;
using Nighthollow.Items;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public sealed class RewardChoiceWindow : HideableElement<RewardChoiceWindow.Args>, IDragManager<ItemImage, ItemSlot>
  {
    const float AnimationOneDuration = 0.1f;
    const float AnimationOnePause = 0.1f;
    const float AnimationTwoDuration = 0.1f;
    const Ease AnimationOneEase = Ease.OutBounce;
    const Ease AnimationTwoEase = Ease.OutCirc;
    const float SequenceDelaySeconds = 0.1f;

    VisualElement _optionsContainer = null!;
    VisualElement _pickedItemsContainer = null!;
    VisualElement _confirmButton = null!;
    VisualElement _costText = null!;

    List<ItemSlot> _optionSlots = null!;
    List<ItemSlot> _pickedItemSlots = null!;
    List<ItemImage> _images = null!;

    public readonly struct Args
    {
      public readonly IReadOnlyList<IItemData> Items;

      public Args(IReadOnlyList<IItemData> items)
      {
        Items = items;
      }
    }

    public new sealed class UxmlFactory : UxmlFactory<RewardChoiceWindow, UxmlTraits>
    {
    }

    protected override void Initialize()
    {
      _optionsContainer = FindElement("Content");
      _pickedItemsContainer = FindElement("Selections");
      _confirmButton = FindElement("ConfirmButton");
      _costText = FindElement("CostText");
    }

    protected override void OnShow(Args argument)
    {
      ItemRenderer.AddItems(Registry,
        _optionsContainer,
        argument.Items,
        new ItemRenderer.Config(argument.Items.Count, shouldAddTooltip: false));
      RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    public void AnimateOpening()
    {
      var sequence = DOTween.Sequence();
      for (var i = 0; i < _images.Count; ++i)
      {
        var element = _images[i];
        var width = element.worldBound.width;
        var height = element.worldBound.height;
        element.style.width = new StyleLength(width / 2f);
        element.style.height = new StyleLength(height / 2f);

        var position = new Vector3(element.style.left.value.value, element.style.top.value.value, z: 0);
        // Interpolate target position along a circle
        var lerp = Quaternion.AngleAxis(
          Mathf.Lerp(a: 0f, b: 360f, i / (float) _images.Count + 0.1f),
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
            foreach (var image in _images)
            {
              image.HasTooltip = true;
              image.MakeDraggable(this);
            }
          });
      }

      sequence.InsertCallback(AnimationOneDuration + AnimationOnePause + AnimationTwoDuration,
        () => { style.visibility = new StyleEnum<Visibility>(Visibility.Visible); });

      sequence.Play();
    }

    void OnGeometryChange(GeometryChangedEvent evt)
    {
      _optionSlots = new List<ItemSlot>();
      _optionsContainer.Query().Children<ItemSlot>().ForEach(slot => _optionSlots.Add(slot));

      _pickedItemSlots = new List<ItemSlot>();
      _pickedItemsContainer.Query().Children<ItemSlot>().ForEach(slot => _pickedItemSlots.Add(slot));

      _images = new List<ItemImage>();
      _optionsContainer.Query().Children<ItemSlot>().Children<ItemImage>().ForEach(image => _images.Add(image));

      Debug.Log($"Choice Slots: {_optionSlots.Count} Selection Slots: {_pickedItemSlots.Count}");

      foreach (var image in _images)
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

    public IEnumerable<ItemSlot> GetDragTargets(ItemImage element) =>
      _optionSlots.Where(slot => slot.Item == null).Concat(
        _pickedItemSlots.Where(slot => slot.Item == null));

    public void OnDragReceived(ItemSlot target, ItemImage element)
    {
      var pickCount = _pickedItemSlots.Count(slot => slot.Item != null);
      foreach (var slot in _pickedItemSlots)
      {
      }

      if (pickCount > 0)
      {
        _confirmButton.RemoveFromClassList("disabled");
      }
      else
      {
        _confirmButton.AddToClassList("disabled");
      }

      if (pickCount > 1)
      {
        _costText.style.visibility = new StyleEnum<Visibility>(Visibility.Visible);
        _costText.Query().Children<Label>().First().text = $"Cost: {pickCount - 1}";
      }
      else
      {
        _costText.style.visibility = new StyleEnum<Visibility>(Visibility.Hidden);
      }
    }
  }
}