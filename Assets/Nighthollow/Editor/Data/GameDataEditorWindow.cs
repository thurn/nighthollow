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
using Nighthollow.Editing;
using Nighthollow.Interface;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Editor.Data
{
  public sealed class GameDataEditorWindow : EditorWindow
  {
    Database? _database;
    GameObject? _editorDocument;

    [MenuItem("Tools/Show Game Data Editor %#^g")]
    public static void ToggleWindow()
    {
      var window = GetWindow<GameDataEditorWindow>();
      if (window._editorDocument)
      {
        window.rootVisualElement.Clear();
        DestroyImmediate(window._editorDocument);
        window._editorDocument = null;
        window.Close();
      }
      else
      {
        var root = new GameObject("Temp");
        var dataService = root.AddComponent<DataService>();
        dataService.Initialize(synchronous: true).MoveNext();
        dataService.OnReady(fetchResult => OnReady(window, fetchResult));
        DestroyImmediate(root);
      }
    }

    static void OnReady(GameDataEditorWindow window, FetchResult fetchResult)
    {
      window.titleContent = new GUIContent("Game Data Editor");
      window.minSize = new Vector2(250, 50);
      window.Initialize(fetchResult.Database);
    }

    void OnEnable()
    {
      if (_database != null)
      {
        Initialize(_database);
      }
    }

    void Initialize(Database database)
    {
      if (_editorDocument)
      {
        DestroyImmediate(_editorDocument);
      }

      _database = database;
      rootVisualElement.Clear();
      rootVisualElement.styleSheets.Clear();

      var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Nighthollow/Interface/UXML/Screen.uxml");
      tree.CloneTree(rootVisualElement);
      _editorDocument = new GameObject("EditorDocument");
      var controller = _editorDocument.AddComponent<ScreenController>();
      controller.Initialize(rootVisualElement);
      controller.Get(ScreenController.GameDataEditor).Show(new GameDataEditor.Args(database));

      // rootVisualElement.styleSheets.Clear();
      // rootVisualElement.styleSheets.Add(
      //   AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Nighthollow/Interface/Styles/styles.uss"));
      // rootVisualElement.AddToClassList("full-screen");
      // rootVisualElement.name = "Screen";
      //
      // var editor = new GameDataEditor();
      // editor.Show(new GameDataEditor.Args(database));
      // rootVisualElement.Add(editor);
    }
  }
}