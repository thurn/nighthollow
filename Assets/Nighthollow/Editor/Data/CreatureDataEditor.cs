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

using Nighthollow.Data;
using UnityEditor;
using UnityEngine.UIElements;

namespace Nighthollow.Editor.Data
{
  [CustomEditor(typeof(CreatureData))]
  [CanEditMultipleObjects]
  public sealed class CreatureDataEditor : UnityEditor.Editor
  {
    public override VisualElement CreateInspectorGUI()
    {
      var root = new VisualElement();

      EditorHelper.AddDefaultInspector(root, serializedObject);

      var writeDefaults = new Button {text = "Write Defaults"};
      writeDefaults.clickable.clicked += () =>
      {
        ((CreatureData) target).SetDefaults();
        EditorUtility.SetDirty(target);
      };
      writeDefaults.style.marginTop = 10;
      root.Add(writeDefaults);

      return root;
    }
  }
}