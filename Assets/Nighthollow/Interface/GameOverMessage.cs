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
using Nighthollow.Services;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Nighthollow.Interface
{
  public sealed class GameOverMessage : HideableElement<GameOverMessage.Args>
  {
    public readonly struct Args
    {
      public readonly string Text;
      public readonly string LoadScene;

      public Args(string text, string loadScene)
      {
        Text = text;
        LoadScene = loadScene;
      }
    }

    Label _text = null!;

    public new sealed class UxmlFactory : UxmlFactory<GameOverMessage, UxmlTraits>
    {
    }

    protected override void Initialize()
    {
      _text = Find<Label>("GameOverText");
    }

    protected override void OnShow(Args args)
    {
      Root.Instance.User.OnGameOver();
      _text.text = args.Text;
      ButtonUtil.DisplayMainButtons(Controller,
        new List<ButtonUtil.Button> {new ButtonUtil.Button("Continue", () =>
        {
          SceneManager.LoadScene(args.LoadScene);
        }, large: true)});
    }
  }
}
