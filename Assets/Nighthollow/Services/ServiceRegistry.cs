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
using Nighthollow.Interface;
using Nighthollow.Triggers;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Services
{
  public class ServiceRegistry
  {
    readonly UIDocument _document;

    public ServiceRegistry(
      Database database,
      AssetService assetService,
      UIDocument document,
      Camera mainCamera,
      ObjectPoolService objectPoolService)
    {
      Database = database;
      AssetService = assetService;
      _document = document;
      MainCamera = mainCamera;
      ObjectPoolService = objectPoolService;
      Globals = new GlobalsService(database);
      TriggerService = new TriggerService(Database, Globals);
    }

    public virtual void OnUpdate()
    {
      ScreenController.OnUpdate();
    }

    public Database Database { get; }
    public AssetService AssetService { get; }
    ScreenController? _screenController;
    public ScreenController ScreenController => _screenController ??= new ScreenController(_document, this);
    public Camera MainCamera { get; }
    public ObjectPoolService ObjectPoolService { get; }
    public GlobalsService Globals { get; }
    public TriggerService TriggerService { get; }
  }
}