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
    [SerializeField] List<File> _userFiles;
    [SerializeField] List<File> _enemyFiles;

    void Awake()
    {
      InitializeFiles();
    }

    void InitializeFiles()
    {
      _userFiles.Clear();
      _enemyFiles.Clear();

      for (var i = 0; i < 6; ++i)
      {
        _userFiles.Add(new File(PlayerName.User));
        _enemyFiles.Add(new File(PlayerName.Enemy));
      }
    }

    void Update()
    {
      if (Input.GetMouseButtonDown(0))
      {
        var mousePosition = Injector.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
        if (mousePosition.y >= Constants.IndicatorBottomY &&
            mousePosition.x <= Constants.IndicatorRightX)
        {
          // Clowntown version of a drag handler. Switch this to use proper collider detection or something.
          var file = _userFiles[BoardPositions.ClosestFileForYPosition(mousePosition.y - 0.5f).ToIndex()];
          var rank = BoardPositions.ClosestRankForXPosition(mousePosition.x, PlayerName.User);
          var draggingCreature =
            file.GetAtPosition(rank);

          if (!draggingCreature)
          {
            file = _userFiles[BoardPositions.ClosestFileForYPosition(mousePosition.y - 1.5f).ToIndex()];
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

    public static Creature Create(CreatureData creatureData)
    {
      var result =
        ComponentUtils.Instantiate<Creature>(creatureData.Prefab);
      result.Initialize(creatureData);
      return result;
    }

    public bool HasCreature(CreatureId creatureId) => _creatures.ContainsKey(creatureId.Value);

    public Creature Get(CreatureId creatureId)
    {
      if (!_creatures.ContainsKey(creatureId.Value))
      {
        throw new ArgumentException($"Creature with ID {creatureId} not found!");
      }

      return _creatures[creatureId.Value];
    }

    public void Destroy(CreatureId creatureId)
    {
      var creature = Get(creatureId);
      var files = GetFiles(creature.Owner);
      files[creature.FilePosition.Value.ToIndex()].RemoveAtPosition(creature.RankPosition.Value);
      _creatures.Remove(creatureId.Value);
      creature.Destroy();
    }

    public void AddCreatureAtPosition(Creature creature, RankValue rank, FileValue file)
    {
      var files = GetFiles(creature.Owner);
      creature.SetPosition(rank, file);
      files[file.ToIndex()].AddCreature(creature, rank);
      _creatures[creature.CreatureId.Value] = creature;
      foreach (var f in files)
      {
        f.ToDefaultPositions();
      }
    }

    /// <summary>Gets the position closest file to 'filePosition' which is not full.</summary>
    public FileValue GetClosestAvailableFile(FileValue filePosition, PlayerName owner)
    {
      var files = GetFiles(owner);
      foreach (var f in Closest(filePosition))
      {
        if (files[f.ToIndex()].Count() < 8)
        {
          return f;
        }
      }

      throw new InvalidOperationException("Board is Full!");
    }

    /// <summary>Get the closest creature owned by 'owner' to 'position'</summary>
    public Creature GetClosestCreature(PlayerName owner, Vector2 position)
    {
      Creature result = null;
      var closestDistance = float.MaxValue;
      foreach (var creature in _creatures.Values)
      {
        var distance = Vector2.Distance(position, creature.transform.position);
        if (creature.Owner == owner && distance < closestDistance)
        {
          closestDistance = distance;
          result = creature;
        }
      }

      return result;
    }

    /// <summary>
    /// Shifts the positions of creatures such that the provided 'rank,file' position is available. Only one position
    /// shift can be in effect at a time.
    /// </summary>
    public void ShiftPositions(PlayerName owner, RankValue rankValue, FileValue fileValue)
    {
      var files = GetFiles(owner);
      foreach (var file in files)
      {
        file.ToDefaultPositions();
      }

      files[fileValue.ToIndex()].ShiftPositions(rankValue);
    }

    public void DestroyAllCreatures()
    {
      foreach (var creature in _creatures.Values)
      {
        Destroy(creature.gameObject);
      }

      _creatures.Clear();
      InitializeFiles();
    }

    List<File> GetFiles(PlayerName owner) => owner == PlayerName.User ? _userFiles : _enemyFiles;

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
    [SerializeField] PlayerName _owner;
    [SerializeField] List<Creature> _creatures = new List<Creature> {null, null, null, null, null, null, null, null};

    public File(PlayerName owner)
    {
      _owner = owner;
    }

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
          creatures[i].transform.DOMoveX(BoardPositions.RankForIndex(i).ToXPosition(_owner), 0.2f);
        }
      }
    }
  }
}