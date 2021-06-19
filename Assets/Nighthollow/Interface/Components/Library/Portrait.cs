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

using System;
using Nighthollow.Interface.Components.Core;
using Nighthollow.Rules;

#nullable enable

namespace Nighthollow.Interface.Components.Library
{
  public sealed record Portrait : LayoutComponent
  {
    public Portrait(CharacterName name)
    {
      Name = name;
    }

    public enum CharacterName
    {
      You,
      Ocerak
    }

    public CharacterName Name { get; }

    protected override BaseComponent OnRender(Scope scope) => new Row
    {
      Width = 84,
      Height = 84,
      BackgroundImage = Name switch
      {
        CharacterName.You => "Portraits/You",
        CharacterName.Ocerak => "Portraits/Ocerak",
        _ => throw new ArgumentOutOfRangeException()
      }
    };
  }
}