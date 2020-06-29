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
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Nighthollow.Components;
using Nighthollow.Model;
using Nighthollow.Utils;
using UnityEngine;

namespace Nighthollow.Services
{
  public sealed class CreatureService : MonoBehaviour
  {
    readonly Dictionary<int, Creature> _creatures = new Dictionary<int, Creature>();
    [SerializeField] List<File> _files;

    void Awake()
    {
      InitializeFiles();
    }

    void InitializeFiles()
    {
      _files.Clear();

      for (var i = 0; i < 6; ++i)
      {
        _files.Add(new File());
      }
    }

    void Update()
    {
      if (Input.GetMouseButtonDown(0))
      {
        var mousePosition = Root.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
        if (mousePosition.y >= Constants.IndicatorBottomY &&
            mousePosition.x <= Constants.IndicatorRightX)
        {
          // Clowntown version of a drag handler. Switch this to use proper collider detection or something.
          var file = _files[BoardPositions.ClosestFileForYPosition(mousePosition.y - 0.5f).ToIndex()];
          var rank = BoardPositions.ClosestRankForXPosition(mousePosition.x);
          var draggingCreature =
            file.GetAtPosition(rank);

          if (!draggingCreature)
          {
            file = _files[BoardPositions.ClosestFileForYPosition(mousePosition.y - 1.5f).ToIndex()];
            draggingCreature = file.GetAtPosition(rank);
          }

          if (draggingCreature)
          {
            file.RemoveAtPosition(rank);
            draggingCreature.gameObject.AddComponent<CreaturePositionSelector>().Initialize(draggingCreature);
          }
        }
      }
    }

    public Creature CreateUserCreature(CreatureData creatureData)
    {
      var result =
        ComponentUtils.Instantiate<Creature>(creatureData.Prefab);
      result.Initialize(creatureData);
      return result;
    }

    public void CreateCreature(CreatureData creatureData, RankValue? rank, FileValue file)
    {
      Root.Instance.AssetService.FetchCreatureAssets(creatureData, () =>
      {
        var result =
          ComponentUtils.Instantiate<Creature>(creatureData.Prefab);
        result.Initialize(creatureData);
        result.ActivateCreature(rank, file);
        _creatures[creatureData.CreatureId.Value] = result;
      });
    }

    public bool HasCreature(CreatureId creatureId) => _creatures.ContainsKey(creatureId.Value);

    public Creature GetCreature(CreatureId creatureId)
    {
      if (!_creatures.ContainsKey(creatureId.Value))
      {
        throw new ArgumentException($"Creature with ID {creatureId} not found!");
      }

      return _creatures[creatureId.Value];
    }

    public void Destroy(CreatureId creatureId)
    {
      var creature = GetCreature(creatureId);
      if (creature.RankPosition.HasValue)
      {
        _files[creature.FilePosition.ToIndex()].RemoveAtPosition(creature.RankPosition.Value);
      }

      _creatures.Remove(creatureId.Value);
      creature.Destroy();
    }

    public void AddUserCreatureAtPosition(Creature creature, RankValue rank, FileValue file)
    {
      creature.ActivateCreature(rank, file);
      _files[file.ToIndex()].AddCreature(creature, rank);
      _creatures[creature.CreatureId.Value] = creature;
      foreach (var f in _files)
      {
        f.ToDefaultPositions();
      }
    }

    /// <summary>Gets the position closest file to 'filePosition' which is not full.</summary>
    public FileValue GetClosestAvailableFile(FileValue filePosition, PlayerName owner)
    {
      foreach (var f in Closest(filePosition))
      {
        if (_files[f.ToIndex()].Count() < 8)
        {
          return f;
        }
      }

      throw new InvalidOperationException("Board is Full!");
    }

    /// <summary>
    /// Shifts the positions of creatures such that the provided 'rank,file' position is available. Only one position
    /// shift can be in effect at a time.
    /// </summary>
    public void ShiftPositions(RankValue rankValue, FileValue fileValue)
    {
      foreach (var file in _files)
      {
        file.ToDefaultPositions();
      }

      _files[fileValue.ToIndex()].ShiftPositions(rankValue);
    }

    public void DestroyAllCreatures()
    {
      foreach (var creature in _creatures.Values)
      {
        if (creature && creature.gameObject)
        {
          Destroy(creature.gameObject);
        }
        else
        {
          Debug.LogError($"Already destroyed: {creature}");
        }
      }

      _creatures.Clear();
      InitializeFiles();
    }

    public void FireProjectile(ProjectileData projectileData)
    {
      Root.Instance.AssetService.FetchProjectileAssets(projectileData, () =>
      {
        var firingPoint = GetCreature(projectileData.FiredBy).ProjectileSource;
        var projectile = Root.Instance.ObjectPoolService.Create(projectileData.Prefab, firingPoint.position);
        ComponentUtils.GetComponent<Projectile>(projectile).Initialize(projectileData, firingPoint);
      });
    }

    static IEnumerable<FileValue> Closest(FileValue f)
    {
      switch (f)
      {
        case FileValue.File1:
          return new[]
            {FileValue.File1, FileValue.File2, FileValue.File3, FileValue.File4, FileValue.File5};
        case FileValue.File2:
          return new[]
            {FileValue.File2, FileValue.File1, FileValue.File3, FileValue.File4, FileValue.File5};
        case FileValue.File3:
          return new[]
            {FileValue.File3, FileValue.File2, FileValue.File4, FileValue.File1, FileValue.File5};
        case FileValue.File4:
          return new[]
            {FileValue.File4, FileValue.File3, FileValue.File5, FileValue.File2, FileValue.File1};
        case FileValue.File5:
          return new[]
            {FileValue.File5, FileValue.File4, FileValue.File3, FileValue.File2, FileValue.File1};
        default: throw Errors.UnknownEnumValue(f);
      }
    }
  }

  [Serializable]
  sealed class File
  {
    [SerializeField] List<Creature> _creatures = new List<Creature> {null, null, null, null, null, null, null, null};

    public int Count() => _creatures.Count(c => c != null);

    public Creature GetAtPosition(RankValue rankValue) => _creatures[rankValue.ToIndex()];

    public void AddCreature(Creature creature, RankValue rank)
    {
      if (_creatures[rank.ToIndex()])
      {
        _creatures = ComputeShiftsForIndex(rank.ToIndex());
        _creatures[rank.ToIndex()] = creature;
      }
      else
      {
        _creatures[rank.ToIndex()] = creature;
      }
    }

    public void RemoveAtPosition(RankValue rankValue) => _creatures[rankValue.ToIndex()] = null;

    public void ShiftPositions(RankValue rank)
    {
      var index = rank.ToIndex();
      if (Count() == 8)
      {
        throw new ArgumentException("File is full!");
      }

      if (_creatures[index])
      {
        var shifts = ComputeShiftsForIndex(rank.ToIndex());
        AnimateToPositions(shifts);
      }
    }

    string Display(IEnumerable<Creature> creatures) =>
      "[" + string.Join(",", creatures.Select(c => c == null ? "null" : c.CreatureId.ToString())) + "]";

    List<Creature> ComputeShiftsForIndex(int index)
    {
      var leftNullDistance = int.MaxValue;
      var rightNullDistance = int.MaxValue;
      var leftNullIndex = 0;
      var rightNullIndex = 0;

      for (var i = 0; i < _creatures.Count; ++i)
      {
        if (!_creatures[i])
        {
          var distance = Math.Abs(index - i);
          if (i < index && distance < leftNullDistance)
          {
            leftNullDistance = distance;
            leftNullIndex = i;
          }

          if (i > index && distance < rightNullDistance)
          {
            rightNullDistance = distance;
            rightNullIndex = i;
          }
        }
      }

      var result = new List<Creature>(_creatures);
      if (leftNullDistance <= rightNullDistance)
      {
        while (leftNullIndex != index)
        {
          result[leftNullIndex] = result[leftNullIndex + 1];
          leftNullIndex++;
        }
      }
      else
      {
        while (rightNullIndex != index)
        {
          result[rightNullIndex] = result[rightNullIndex - 1];
          rightNullIndex--;
        }
      }

      result[index] = null;

      return result;
    }

    public void ToDefaultPositions()
    {
      AnimateToPositions(_creatures);
    }

    void AnimateToPositions(IReadOnlyList<Creature> creatures)
    {
      for (var i = 0; i < creatures.Count; ++i)
      {
        if (creatures[i])
        {
          creatures[i].transform.DOMoveX(BoardPositions.RankForIndex(i).ToXPosition(), 0.2f);
        }
      }
    }
  }
}