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

using Nighthollow.Components;
using Nighthollow.Data;
using Nighthollow.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nighthollow.Services
{
  public sealed class CreatureService : MonoBehaviour
  {
    readonly Dictionary<(RankValue, FileValue), Creature> _userCreatures =
      new Dictionary<(RankValue, FileValue), Creature>();
    readonly HashSet<Creature> _enemyCreatures = new HashSet<Creature>();

    public Creature CreateUserCreature(CreatureData creatureData)
    {
      var result =
        ComponentUtils.Instantiate(creatureData.Prefab);
      result.Initialize(creatureData);
      return result;
    }

    public Creature CreateEnemyCreature(CreatureData creatureData, FileValue file)
    {
      var result = ComponentUtils.Instantiate(creatureData.Prefab);
      result.Initialize(creatureData);
      result.ActivateCreature(null, file);
      _enemyCreatures.Add(result);

      foreach (var creature in _userCreatures.Values)
      {
        creature.OnOpponentCreaturePlayed(result);
      }

      return result;
    }

    public void AddUserCreatureAtPosition(Creature creature, RankValue rank, FileValue file)
    {
      creature.ActivateCreature(rank, file);
      _userCreatures[(rank, file)] = creature;

      foreach (var enemy in _enemyCreatures)
      {
        enemy.OnOpponentCreaturePlayed(creature);
      }
    }

    public void RemoveCreature(Creature creature)
    {
      if (creature.Owner == PlayerName.User)
      {
        _userCreatures.Remove((creature.RankPosition.Value, creature.FilePosition));
      }
      else
      {
        _enemyCreatures.Remove(creature);
      }
    }

    /// <summary>Gets the position closest file to 'filePosition' which is not full.</summary>
    public (RankValue, FileValue) GetClosestAvailablePosition(Vector2 position)
    {
      RankValue? closestRank = null;
      FileValue? closestFile = null;
      var closestDistance = float.MaxValue;

      foreach (RankValue rank in Enum.GetValues(typeof(RankValue)))
      {
        foreach (FileValue file in Enum.GetValues(typeof(FileValue)))
        {
          if (rank == RankValue.Unknown ||
            file == FileValue.Unknown ||
            _userCreatures.ContainsKey((rank, file)))
          {
            continue;
          }

          var distance = Vector2.Distance(position,
            new Vector2(rank.ToXPosition(), file.ToYPosition()));
          if (distance < closestDistance)
          {
            closestDistance = distance;
            closestRank = rank;
            closestFile = file;
          }
        }
      }

      if (closestRank != null && closestFile != null)
      {
        return (closestRank.Value, closestFile.Value);
      }
      else
      {
        throw new InvalidOperationException("Board is full!");
      }
    }
  }
}