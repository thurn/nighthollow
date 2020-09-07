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
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Nighthollow.Editor.Data
{
  public static class ClearHideFlags
  {
    [MenuItem("Tools/Data/Clear Hide Flags")]
    public static void Run()
    {
      if (Selection.assetGUIDs.Length != 1)
      {
        throw new InvalidOperationException("Please select only one game object");
      }

      var path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs.First());
      var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
      asset.hideFlags = HideFlags.None;
    }
  }
}