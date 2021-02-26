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

using System.Reflection;
using UnityEditor;

#nullable enable

namespace Nighthollow.Editor
{
  [InitializeOnLoad]
  public static class BuildHelper
  {

    static BuildHelper()
    {
      EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
      if (state == PlayModeStateChange.ExitingPlayMode)
      {
        AssetDatabase.Refresh();
      }
    }

    static void ClearEditorConsole()
    {
      var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
      var type = assembly.GetType("UnityEditor.LogEntries");
      var method = type.GetMethod("Clear");
      method!.Invoke(new object(), null);
    }

    [MenuItem("Tools/Run Build Helper &#^b")]
    public static void Run()
    {
      // The default limit on nested generics in IL2CPP is 7, but microsoft loves nesting, so we need to increase it
      // for System.Collections.Immutable to work.
      PlayerSettings.SetAdditionalIl2CppArgs("--maximum-recursive-generic-depth 30");
    }
  }
}
