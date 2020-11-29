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


using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public sealed class MainMenu : VisualElement
  {
    VisualElement? _about;
    VisualElement? _main;
    VisualElement? _settings;
    Label? _title;

    public MainMenu()
    {
      if (Application.isPlaying) RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    void OnGeometryChange(GeometryChangedEvent evt)
    {
      _main = this.Q("Main");
      _settings = this.Q("Settings");
      _about = this.Q("About");
      _title = this.Q<Label>("MenuTitle")!;

      _main.Q("PlayTutorial").RegisterCallback<ClickEvent>(e => { SceneManager.LoadScene("Introduction"); });
      _main.Q("ShowSettings").RegisterCallback<ClickEvent>(e =>
      {
        _main.style.display = DisplayStyle.None;
        _settings.style.display = DisplayStyle.Flex;
        _title.text = "Settings";
      });
      _main.Q("ShowAbout").RegisterCallback<ClickEvent>(e =>
      {
        _main.style.display = DisplayStyle.None;
        _about.style.display = DisplayStyle.Flex;
        _title.text = "About";
      });
      _settings.Q("GoBack").RegisterCallback<ClickEvent>(e =>
      {
        _main.style.display = DisplayStyle.Flex;
        _settings.style.display = DisplayStyle.None;
        _title.text = "Nighthollow";
      });
      _about.Q("GoBack").RegisterCallback<ClickEvent>(e =>
      {
        _main.style.display = DisplayStyle.Flex;
        _about.style.display = DisplayStyle.None;
        _title.text = "Nighthollow";
      });
      UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    public new sealed class UxmlFactory : UxmlFactory<MainMenu, UxmlTraits>
    {
    }

    public new sealed class UxmlTraits : VisualElement.UxmlTraits
    {
    }
  }
}
