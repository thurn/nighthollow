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
using Nighthollow.Components;
using Nighthollow.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Nighthollow.Services
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

    [SerializeField] EventService _eventService;
    public EventService EventService => _eventService;

    [SerializeField] ObjectPoolService _objectPoolService;
    public ObjectPoolService ObjectPoolService => _objectPoolService;

    [SerializeField] CreatureService _creatureService;
    public CreatureService CreatureService => _creatureService;

    [FormerlySerializedAs("_player")] [SerializeField] User _user;
    public User User => _user;

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

    void OnEnable()
    {
      Errors.CheckNotNull(_mainCamera);
      Errors.CheckNotNull(_mainCanvas);
      Errors.CheckNotNull(_prefabs);
      Errors.CheckNotNull(_eventService);
      Errors.CheckNotNull(_objectPoolService);
      Errors.CheckNotNull(_creatureService);
      Errors.CheckNotNull(_user);

      _instance = this;
    }
  }
}