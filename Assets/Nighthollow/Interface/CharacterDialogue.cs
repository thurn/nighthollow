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
using Nighthollow.Triggers.Effects;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public sealed class CharacterDialogue : HideableElement<CharacterDialogueEffect>
  {
    VisualElement _portrait = null!;
    Label _text = null!;
    VisualElement _continueButton = null!;

    public new sealed class UxmlFactory : UxmlFactory<CharacterDialogue, UxmlTraits>
    {
    }

    protected override void Initialize()
    {
      _portrait = FindElement("DialoguePortrait");
      _text = Find<Label>("DialogueText");
      _continueButton = FindElement("DialogueContinueButton");
    }

    protected override void OnShow(CharacterDialogueEffect argument)
    {
      _portrait.ClearClassList();
      _portrait.AddToClassList("portrait");
      _portrait.AddToClassList(argument.CharacterName switch
      {
        CharacterName.Ocerak => "ocerak",
        CharacterName.You => "you",
        _ => throw new ArgumentOutOfRangeException()
      });
      _text.text = argument.Text;
    }
  }
}