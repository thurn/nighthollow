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

using System.Collections;
using System.Collections.Generic;
using Nighthollow.Data;
using Nighthollow.Editing;
using Nighthollow.Interface;
using Nighthollow.Services;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Editor.Data
{
  public sealed class GameDataEditorWindow : EditorWindow, IStartCoroutine
  {
    Database? _database;
    AssetService? _assetService;
    bool _open;

    [MenuItem("Tools/Show Game Data Editor %#^e")]
    public static void ToggleWindow()
    {
      var window = GetWindow<GameDataEditorWindow>();
      if (window._open)
      {
        window.rootVisualElement.Clear();
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
      window.Initialize(fetchResult.Database, fetchResult.AssetService);
    }

    void OnEnable()
    {
      if (_database != null && _assetService != null)
      {
        Initialize(_database, _assetService);
      }
    }

    void Initialize(Database database, AssetService assetService)
    {
      _database = database;
      _assetService = assetService;
      rootVisualElement.Clear();
      rootVisualElement.styleSheets.Clear();
      rootVisualElement.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
      _open = true;

      var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Nighthollow/Interface/UXML/Screen.uxml");
      tree.CloneTree(rootVisualElement);
      var registry = new EditorServiceRegistry(database, assetService, rootVisualElement, Camera.main!, this);
      registry.ScreenController.Screen.AddToClassList("rendered-in-editor");
      registry.ScreenController.Get(ScreenController.GameDataEditor).Show(new GameDataEditor.Args(database));
    }

    void OnGeometryChanged(GeometryChangedEvent evt)
    {
      rootVisualElement.transform.scale = new Vector3(0.5f, 0.5f, 1);
      rootVisualElement.style.width = evt.newRect.width * 2;
      rootVisualElement.style.height = evt.newRect.height * 2;

      rootVisualElement.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    public Coroutine StartCoroutine(IEnumerator routine)
    {
      throw new System.NotImplementedException();
    }
  }
}