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

using System.Linq;
using Nighthollow.Data;
using Nighthollow.Interface.Components.Core;
using Nighthollow.Interface.Components.Library;
using Nighthollow.Rules;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface.Components.Windows
{
  public sealed record VictoryWindow : LayoutComponent
  {
    protected override BaseComponent OnRender(Scope scope)
    {
      var rewards = scope.Get(Key.RewardsService).CreateRewardsForCurrentBattle();
      return new Window
      {
        Title = "Victory!",
        Size = Window.WindowSize.FitContent,
        Content = new Column
        {
          AlignItems = Align.Center,
          Children = List(
            new Text("Gained Resources")
            {
              Type = TextType.Headline,
              HierarchyLevel = HierarchyLevel.Three
            },
            new Row
            {
              Children = rewards.Choices.Select(choice => new ItemSlot
              {
                Size = ItemSlot.SlotSize.Small,
                Image = new ItemImage(choice)
              })
            },
            new Text("Select Reward")
            {
              Type = TextType.Headline,
              HierarchyLevel = HierarchyLevel.Three
            },
            new Row
            {
              Children = rewards.Choices.Select(RewardChoice)
            }
          )
        }
      };
    }

    static BaseComponent RewardChoice(IItemData item) => new Column
    {
      AlignItems = Align.Center,
      JustifyContent = Justify.Center,
      MarginAll = 16,
      Children = List(
        new ItemSlot
        {
          Image = new ItemImage(item),
          MarginBottom = 8
        },
        new LabelButton("Pick")
        {
          HierarchyLevel = HierarchyLevel.Three
        }
      )
    };
  }
}