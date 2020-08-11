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
using Nighthollow.Data;
using Nighthollow.Utils;
using UnityEditor;
using UnityEngine;

namespace Nighthollow.Editor.Data
{
  [CustomPropertyDrawer(typeof(Damage))]
  public class DamageDrawer : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      // Using BeginProperty / EndProperty on the parent property means that
      // prefab override logic works on the entire property.
      EditorGUI.BeginProperty(position, label, property);

      // Draw label
      position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

      // Don't make child fields be indented
      var indent = EditorGUI.indentLevel;
      EditorGUI.indentLevel = 0;

      var count = 0;
      foreach (DamageType type in Enum.GetValues(typeof(DamageType)))
      {
        GUI.backgroundColor = InfluenceDrawer.Colors[count];
        var rect = new Rect(position.x + (count * 35), position.y, 30, position.height);
        EditorGUI.PropertyField(rect,
          Errors.CheckNotNull(property.FindPropertyRelative(PropertyName(type))),
          GUIContent.none);
        count++;
      }

      GUI.backgroundColor = Color.white;

      // Set indent back to what it was
      EditorGUI.indentLevel = indent;

      EditorGUI.EndProperty();
    }

    string PropertyName(DamageType damageType)
    {
      switch (damageType)
      {
        case DamageType.Radiant:
          return "_radiant";
        case DamageType.Lightning:
          return "_lightning";
        case DamageType.Fire:
          return "_fire";
        case DamageType.Cold:
          return "_cold";
        case DamageType.Physical:
          return "_physical";
        case DamageType.Necrotic:
          return "_necrotic";
        default:
          throw new ArgumentOutOfRangeException(nameof(damageType), damageType, null);
      }
    }
  }
}