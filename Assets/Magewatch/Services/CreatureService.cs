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
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Magewatch.Components;
using Magewatch.Data;
using Magewatch.Utils;
using UnityEngine;

namespace Magewatch.Services
{
  public sealed class CreatureService : MonoBehaviour
  {
    readonly Dictionary<int, Creature> _creatures = new Dictionary<int, Creature>();

    [SerializeField] List<File> _files = new List<File>
    {
      new File(),
      new File(),
      new File(),
      new File(),
      new File(),
      new File(),
    };

    public Creature Create(CreatureData creatureData)
    {
      var result = ComponentUtils.Instantiate<Creature>(creatureData.Prefab.Value);
      _creatures[creatureData.CreatureId] = result;
      result.Initialize(creatureData);
      return result;
    }

    public void AddEnemyCreature(Creature creature)
    {
      _creatures[creature.CreatureId] = creature;
    }

    public Creature Get(int creatureId) => _creatures[creatureId];

    public void AddCreatureAtPosition(Creature creature, RankValue rank, FileValue file)
    {
      _files[file.ToIndex()].AddCreature(creature, rank);
      _creatures[creature.CreatureId] = creature;
      foreach (var f in _files)
      {
        f.ToDefaultPositions();
      }
    }

    /// <summary>Gets the position closest file to 'filePosition' which is not full.</summary>
    public FileValue GetClosestAvailableFile(FileValue filePosition)
    {
      foreach (var f in Closest(filePosition))
      {
        if (_files[f.ToIndex()].Count() < 6)
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

    static IEnumerable<FileValue> Closest(FileValue f)
    {
      switch (f)
      {
        case FileValue.File0:
          return new[]
            {FileValue.File0, FileValue.File1, FileValue.File2, FileValue.File3, FileValue.File4, FileValue.File5};
        case FileValue.File1:
          return new[]
            {FileValue.File1, FileValue.File0, FileValue.File2, FileValue.File3, FileValue.File4, FileValue.File5};
        case FileValue.File2:
          return new[]
            {FileValue.File2, FileValue.File1, FileValue.File3, FileValue.File0, FileValue.File4, FileValue.File5};
        case FileValue.File3:
          return new[]
            {FileValue.File3, FileValue.File2, FileValue.File4, FileValue.File1, FileValue.File0, FileValue.File5};
        case FileValue.File4:
          return new[]
            {FileValue.File4, FileValue.File3, FileValue.File5, FileValue.File2, FileValue.File1, FileValue.File0};
        case FileValue.File5:
          return new[]
            {FileValue.File5, FileValue.File4, FileValue.File3, FileValue.File2, FileValue.File1, FileValue.File0};
        default: throw Errors.UnknownEnumValue(f);
      }
    }
  }

  [Serializable]
  sealed class File
  {
    [SerializeField] List<Creature> _creatures = new List<Creature> {null, null, null, null, null, null};

    public int Count() => _creatures.Count(c => c != null);

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

    public void ShiftPositions(RankValue rank)
    {
      var index = rank.ToIndex();
      if (Count() == 6)
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