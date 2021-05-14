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

using Nighthollow.Services;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.World
{
  public sealed class CameraMover : MonoBehaviour
  {
    [SerializeField] Camera _camera = null!;
    [SerializeField] float _zoomDelta;
    [SerializeField] float _keyboardMovementSpeed;
    [SerializeField] float _scrollWheelZoomSpeed;
    [SerializeField] float _keyboardZoomSpeed;
    [SerializeField] float _minimumCameraSize;
    [SerializeField] float _maximumCameraSize;
    [SerializeField] float _maxXZoomedIn;
    [SerializeField] float _maxXZoomedOut;
    [SerializeField] float _maxYZoomedIn;
    [SerializeField] float _maxYZoomedOut;
    WorldServiceRegistry? _registry;

    public Camera Camera => _camera;

    public void Initialize(WorldServiceRegistry registry)
    {
      _registry = registry;
    }

    void Awake()
    {
      Errors.CheckNotNull(_camera);
    }

    void Update()
    {
      if (_registry?.ScreenController.HasExclusiveFocus() == true)
      {
        return;
      }

      var translation = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), z: 0) *
                        (_keyboardMovementSpeed * Time.deltaTime);
      if (translation != Vector3.zero)
      {
        transform.Translate(translation, Space.Self);
        _registry?.WorldMapRenderer.ClearSelection();
      }

      _zoomDelta += Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * _scrollWheelZoomSpeed;
      _zoomDelta += ZoomDirection() * Time.deltaTime * _keyboardZoomSpeed;
      Errors.CheckNotNull(_camera).orthographicSize = Mathf.Lerp(
        _minimumCameraSize, _maximumCameraSize, Mathf.Clamp01(_zoomDelta));

      var maxX = Mathf.Lerp(_maxXZoomedIn, _maxXZoomedOut, _zoomDelta);
      var maxY = Mathf.Lerp(_maxYZoomedIn, _maxYZoomedOut, _zoomDelta);
      transform.position = new Vector3(
        Mathf.Clamp(transform.position.x, -maxX, maxX),
        Mathf.Clamp(transform.position.y, -maxY, maxY),
        transform.position.z);
    }

    int ZoomDirection()
    {
      var zoomIn = Input.GetKey(KeyCode.Z);
      var zoomOut = Input.GetKey(KeyCode.X);

      if (zoomIn && zoomOut)
      {
        return 0;
      }
      else if (!zoomIn && zoomOut)
      {
        return 1;
      }
      else if (zoomIn && !zoomOut)
      {
        return -1;
      }
      else
      {
        return 0;
      }
    }
  }
}