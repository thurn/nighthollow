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

using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Editing
{
  public sealed class LabelEditorCell : EditorCell
  {
    readonly string? _text;

    public LabelEditorCell(string? text)
    {
      AddToClassList("editor-emphasized");
      var label = new Label {text = text};
      Add(label);
      _text = text;
    }

    public override string? Preview() => _text;
  }
}