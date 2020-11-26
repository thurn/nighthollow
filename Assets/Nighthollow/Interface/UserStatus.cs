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

#nullable enable

using System.Collections.Generic;
using Nighthollow.Generated;
using Nighthollow.Utils;
using UnityEngine.UIElements;

namespace Nighthollow.Interface
{
  public sealed class UserStatus : HideableElement
  {
    Label _manaText = null!;
    Label _lifeText = null!;
    VisualElement _influenceContainer = null!;

    public new sealed class UxmlFactory : UxmlFactory<UserStatus, UxmlTraits>
    {
    }

    public int Mana
    {
      set => _manaText.text = value.ToString();
    }

    public int Life
    {
      set => _lifeText.text = value.ToString();
    }

    public IReadOnlyDictionary<School, int> Influence
    {
      set
      {
        _influenceContainer.Clear();

        foreach (var pair in value)
        {
          for (var i = 0; i < pair.Value; ++i)
          {
            var element = new VisualElement();
            element.AddToClassList("influence-symbol");
            element.AddToClassList(pair.Key switch
            {
              School.Light => "light",
              School.Sky => "sky",
              School.Flame => "flame",
              School.Ice => "ice",
              School.Earth => "earth",
              School.Shadow => "shadow",
              _ => throw Errors.UnknownEnumValue(pair.Key)
            });
            _influenceContainer.Add(element);
          }
        }
      }
    }

    protected override void OnRegister()
    {
      _manaText = Find<Label>("ManaText");
      _lifeText = Find<Label>("LifeText");
      _influenceContainer = FindElement("InfluenceContainer");
    }
  }
}
