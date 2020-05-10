// Copyright The Magewatch Project

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
using Magewatch.Components;
using Magewatch.Data;
using Magewatch.Utils;
using UnityEngine;

namespace Magewatch.Services
{
  public sealed class Root : MonoBehaviour
  {
    static Root _instance;

    [SerializeField] Camera _mainCamera;
    public Camera MainCamera => _mainCamera;

    [SerializeField] RectTransform _mainCanvas;
    public RectTransform MainCanvas => _mainCanvas;

    [SerializeField] Prefabs _prefabs;
    public Prefabs Prefabs => _prefabs;

    [SerializeField] AssetService _assetService;
    public AssetService AssetService => _assetService;

    [SerializeField] CommandService _commandService;
    public CommandService CommandService => _commandService;

    [SerializeField] ObjectPoolService _objectPoolService;
    public ObjectPoolService ObjectPoolService => _objectPoolService;

    [SerializeField] CreatureService _creatureService;
    public CreatureService CreatureService => _creatureService;

    [SerializeField] MainButton _mainButton;
    public MainButton MainButton => _mainButton;

    [SerializeField] Player _user;
    [SerializeField] Player _enemy;

    public static Root Instance
    {
      get
      {
        if (_instance == null)
        {
          throw new NullReferenceException("Attempted to access Root before Awake!");
        }

        return _instance;
      }
    }

    public Player GetPlayer(PlayerName playerName)
    {
      switch (playerName)
      {
        case PlayerName.User: return _user;
        case PlayerName.Enemy: return _enemy;
        default: throw Errors.UnknownEnumValue(playerName);
      }
    }

    void Awake()
    {
      Errors.CheckNotNull(_mainCamera);
      Errors.CheckNotNull(_mainCanvas);
      Errors.CheckNotNull(_prefabs);
      Errors.CheckNotNull(_assetService);
      Errors.CheckNotNull(_commandService);
      Errors.CheckNotNull(_objectPoolService);
      Errors.CheckNotNull(_creatureService);
      Errors.CheckNotNull(_user);
      Errors.CheckNotNull(_enemy);

      _instance = this;
    }
  }
}