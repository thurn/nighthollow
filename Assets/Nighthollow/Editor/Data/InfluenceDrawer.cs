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
  [CustomPropertyDrawer(typeof(Influence))]
  public class InfluenceDrawer : PropertyDrawer
  {
    public static readonly Color[] Colors =
    {
      new Color(1f, 0.961f, 0.616f),
      new Color(0.565f, 0.792f, 0.976f),
      new Color(1f, 0.8f, 0.502f),
      new Color(0.961f, 0.961f, 0.961f),
      new Color(0.773f, 0.882f, 0.647f),
      new Color(0.702f, 0.616f, 0.859f),
    };

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
      foreach (School type in Enum.GetValues(typeof(School)))
      {
        GUI.backgroundColor = Colors[count];
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

    string PropertyName(School school)
    {
      switch (school)
      {
        case School.Light:
          return "_light";
        case School.Sky:
          return "_sky";
        case School.Flame:
          return "_flame";
        case School.Ice:
          return "_ice";
        case School.Earth:
          return "_earth";
        case School.Shadow:
          return "_shadow";
        default:
          throw new ArgumentOutOfRangeException(nameof(school), school, null);
      }
    }
  }
}