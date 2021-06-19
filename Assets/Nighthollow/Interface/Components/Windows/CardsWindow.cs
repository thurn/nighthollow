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

using System.Collections.Immutable;
using System.Linq;
using Nighthollow.Interface.Components.Core;
using Nighthollow.Interface.Components.Library;
using Nighthollow.Rules;
using UnityEngine;

#nullable enable

namespace Nighthollow.Interface.Components.Windows
{
  public sealed record CardsWindow : LayoutComponent
  {
    protected override BaseComponent OnRender(Scope scope)
    {
      var gameData = scope.Get(Key.GameData);
      return new Window
      {
        Title = "Ocerak, Master of Cards",
        TitlePortrait = Portrait.CharacterName.Ocerak,
        Size = Window.WindowSize.WorldFullScreen,
        Content = new SplitPanelLayout
        {
          LeftPanel = new SplitPanelLayout.Panel
          {
            Title = "Collection",
            Content = new ItemCollectionPanel
            {
              Items = gameData.Collection
            }
          },
          RightPanel = new SplitPanelLayout.Panel
          {
            Title = "Deck",
            Content = new DeckPanel
            {
              MainDeck = gameData.Deck
                .Where(pair => !gameData.CreatureTypes[pair.Value.CreatureTypeId].IsManaCreature)
                .ToImmutableList(),
              ManaDeck = gameData.Deck
                .Where(pair => gameData.CreatureTypes[pair.Value.CreatureTypeId].IsManaCreature)
                .ToImmutableList()
            }
          }
        }
      };
    }
  }
}