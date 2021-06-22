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

using Nighthollow.Interface.Components.Core;
using Nighthollow.Interface.Components.Library;
using Nighthollow.Rules;
using Nighthollow.World.Data;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface.Components.Windows
{
  public sealed record HexWindow : LayoutComponent
  {
    public HexWindow(HexData hexData, Sprite tileSprite)
    {
      HexData = hexData;
      TileSprite = tileSprite;
    }

    public HexData HexData { get; }
    public Sprite TileSprite { get; }

    protected override BaseComponent OnRender(Scope scope)
    {
      var gameData = scope.Get(Key.GameData);
      var owner = HexData.OwningKingdom.HasValue
        ? gameData.Kingdoms[HexData.OwningKingdom.Value].Name.GetName()
        : "None";

      return new Window
      {
        Title = "Bogdul, Master of Construction",
        TitlePortrait = Portrait.CharacterName.Ocerak,
        Size = Window.WindowSize.WorldFullScreen,
        Content = new Row
        {
          FlexPosition = Position.Absolute,
          InsetAll = 64,
          AlignItems = Align.Stretch,
          Children = List(
            new Column
            {
              AlignItems = Align.FlexStart,
              BackgroundImage = "GUI/BasicBar",
              BackgroundSliceAll = 32,
              PaddingAll = 32,
              Children = List(
                new Row
                {
                  AlignItems = Align.FlexEnd,
                  MarginBottom = 16,
                  Height = 300,
                  Children = List(
                    new Row
                    {
                      BackgroundSprite = TileSprite,
                      Height = 192,
                      Width = 128
                    },
                    new Text(HexData.HexType.GetName())
                    {
                      Type = TextType.Headline,
                      HierarchyLevel = HierarchyLevel.Three,
                      MarginBottom = 32,
                      MarginLeft = 32
                    }
                  )
                },
                new Text($"Owner: {owner}")
                {
                  Type = TextType.Body,
                  HierarchyLevel = HierarchyLevel.Three,
                  MarginBottom = 16,
                  MaxWidth = 350
                },
                new Text("Current Consumption:")
                {
                  Type = TextType.Body,
                  HierarchyLevel = HierarchyLevel.Three,
                  MarginBottom = 8
                },
                new Column
                {
                  AlignItems = Align.FlexStart,
                  Children = List(
                    ResourceRow("Food", "Icons/Food", 0),
                    ResourceRow("Lumber", "Icons/Lumber", 0),
                    ResourceRow("Metal", "Icons/Metal", 0),
                    ResourceRow("Arcanite", "Icons/Arcanite", 0)
                  )
                })
            },
            new FocusTree(HexData)
          )
        }
      };
    }

    static BaseComponent ResourceRow(string resourceName, string imageAddress, int quantity) =>
      new Row
      {
        AlignItems = Align.Center,
        MarginAll = 16,
        Children = List(
          new Row
          {
            BackgroundImage = imageAddress,
            Width = 80,
            Height = 80,
          },
          new Text($"{resourceName}: {quantity}")
          {
            Type = TextType.Body,
            HierarchyLevel = HierarchyLevel.Four,
            MarginLeftRight = 16,
          }
        )
      };
  }
}