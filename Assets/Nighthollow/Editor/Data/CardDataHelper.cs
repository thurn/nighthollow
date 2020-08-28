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

using System.Linq;
using Nighthollow.Data;
using UnityEditor;
using UnityEngine;

namespace Nighthollow.Editor.Data
{
  public static class CardDataHelper
  {
    [MenuItem("Tools/Data/Create Item %#i")]
    public static void Import()
    {
      if (Selection.assetGUIDs.Length == 1)
      {
        var path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs.First());
        var card = AssetDatabase.LoadAssetAtPath<CardData>(path);
        var item = ScriptableObject.CreateInstance<CardItemData>();
        item.InitializeFromEditor(card);
        AssetDatabase.CreateAsset(item, path.Replace(".asset", " Item.asset"));
      }
    }
  }
}