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
using Nighthollow.Generated;
using Nighthollow.Services;
using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public sealed class SchoolSelectionScreen : VisualElement
  {
    static readonly string InitialDescription =
      "Magic courses through the world of Nighthollow, granting great power to those who wield it. To begin your journey, you must select one of the six Schools of magic to focus on. Later on you'll be able to pick secondary Schools to supplement your arsenal.";

    readonly Dictionary<School, VisualElement> _buttons = new Dictionary<School, VisualElement>();
    VisualElement? _confirmButton;
    Label? _description;
    KeyValuePair<School, VisualElement>? _selected;

    public new sealed class UxmlFactory : UxmlFactory<SchoolSelectionScreen, UxmlTraits>
    {
    }

    public new sealed class UxmlTraits : VisualElement.UxmlTraits
    {
    }

    public SchoolSelectionScreen()
    {
      RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    void OnGeometryChange(GeometryChangedEvent evt)
    {
      _description = (Label) this.Q("Description");
      _buttons[School.Light] = this.Q("LightButton");
      _buttons[School.Sky] = this.Q("SkyButton");
      _buttons[School.Flame] = this.Q("FlameButton");
      _buttons[School.Ice] = this.Q("IceButton");
      _buttons[School.Earth] = this.Q("EarthButton");
      _buttons[School.Shadow] = this.Q("ShadowButton");
      _confirmButton = this.Q("ConfirmButton");

      _description.text = InitialDescription;

      foreach (var pair in _buttons)
      {
        pair.Value.RegisterCallback<ClickEvent>(e =>
        {
          _selected?.Value.RemoveFromClassList("selected");
          if (_selected?.Value == pair.Value)
          {
            _selected = null;
            _confirmButton.style.opacity = 0f;
            _description.text = InitialDescription;
          }
          else
          {
            _selected = pair;
            pair.Value.AddToClassList("selected");
            _confirmButton.style.opacity = 1f;
            _description.text = DescriptionForSchool(pair.Key);
          }
        });

        _description.RegisterCallback<ClickEvent>(e =>
        {
          _selected?.Value.RemoveFromClassList("selected");
          _selected = null;
          _description.text = InitialDescription;
          _confirmButton.style.opacity = 0f;
        });
      }

      _confirmButton.RegisterCallback<ClickEvent>(e =>
      {
        Database.Instance.UserData.TutorialState = UserDataService.Tutorial.Starting;
        Runner.Instance.StartCoroutine(ShowDialog());
      });

      UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    IEnumerator<YieldInstruction> ShowDialog()
    {
      this.Q("Schools").style.display = DisplayStyle.None;
      _confirmButton!.style.display = DisplayStyle.None;
      var dialog = this.Q("Dialog");
      dialog.style.display = DisplayStyle.Flex;
      InterfaceUtils.FadeIn(dialog);
      yield return new WaitForSeconds(seconds: 4f);
      InterfaceUtils.FadeOut(dialog);
      yield return new WaitForSeconds(seconds: 1f);
      SceneManager.LoadScene("World");
    }

    static string DescriptionForSchool(School school)
    {
      return school switch
      {
        School.Light =>
          "A mage specializing in the school of Light is a user of healing magic. They are able to heal damage or even resurrect creatures which have fallen in battle. They are also great believers in teamwork, focusing on lots of small creatures working together.",
        School.Sky =>
          "A Sky mage wields the power of thunder and lightning. They can call down lightning strikes on their enemies, as well as being the best at disrupting their opponent’s plans through moving creatures around.",
        School.Flame =>
          "A Flame mage attacks enemies by harnessing the power of fire. They have the greatest ability to directly deal damage to opposing creatures, individually or as a group, and their creatures often have high attack but low health",
        School.Ice =>
          "An Ice mage channels the fury of winter into attacking their enemies. They have the ability to freeze enemy creatures, and are known for being defensive. Their creatures often have high health, and are the hardest to destroy.",
        School.Earth =>
          "The school of Earth is all about the raw physical power of the land. They’re the best at making mana, and their creatures tend to have higher health & attack. A simple strategy, but very effective.",
        School.Shadow =>
          "Shadow is a subtle school, which prefers to deal damage indirectly and over time using tools like poison. They also specialize in sacrifice: they’re willing to give up their own creatures or life in order to achieve greater ends.",
        _ => throw Errors.UnknownEnumValue(school)
      };
    }
  }
}
