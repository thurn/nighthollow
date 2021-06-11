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

using Nighthollow.Interface.Components;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public sealed class VictoryWindow : DefaultHideableElement
  {
    VisualElement _fixedRewards = null!;

    public new sealed class UxmlFactory : UxmlFactory<VictoryWindow, UxmlTraits>
    {
    }

    protected override void Initialize()
    {
      _fixedRewards = FindElement("FixedRewards");
    }

    protected override void OnShow()
    {
      // var rewards = Registry.Rewards.CreateRewardsForCurrentBattle();
      // ItemRenderer.AddItems(Registry, _fixedRewards, rewards.FixedRewards,
      //   new ItemRenderer.Config(5, ItemSlot.Size.Small));
    }
  }
}