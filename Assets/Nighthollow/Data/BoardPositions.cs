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


using System.Collections.Generic;
using System.Linq;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Data
{
  public static class BoardPositions
  {
    const float TopY = 5f;
    const float CellSpacing = 2.5f;
    const float RightX = 5f;

    public static readonly IEnumerable<RankValue> AllRanks = new List<RankValue>
    {
      RankValue.Rank1,
      RankValue.Rank2,
      RankValue.Rank3,
      RankValue.Rank4,
      RankValue.Rank5,
      RankValue.Rank6,
      RankValue.Rank7,
      RankValue.Rank8
    };

    public static readonly IEnumerable<FileValue> AllFiles = new List<FileValue>
    {
      FileValue.File1,
      FileValue.File2,
      FileValue.File3,
      FileValue.File4,
      FileValue.File5
    };

    public static float ToXPosition(this RankValue rank)
    {
      return rank switch
      {
        RankValue.Rank1 => RightX - (CellSpacing * 7),
        RankValue.Rank2 => RightX - (CellSpacing * 6),
        RankValue.Rank3 => RightX - (CellSpacing * 5),
        RankValue.Rank4 => RightX - (CellSpacing * 4),
        RankValue.Rank5 => RightX - (CellSpacing * 3),
        RankValue.Rank6 => RightX - (CellSpacing * 2),
        RankValue.Rank7 => RightX - (CellSpacing * 1),
        RankValue.Rank8 => RightX - (CellSpacing * 0),
        _ => throw Errors.UnknownEnumValue(rank)
      };
    }


    public static float ToCenterXPosition(this RankValue rank) => ToXPosition(rank);

    public static RankValue ClosestRankForXPosition(float xPosition)
    {
      var closestDistance = float.MaxValue;
      var closestRank = RankValue.Rank1;
      foreach (var rank in AllRanks)
      {
        var distance = Mathf.Abs(rank.ToXPosition() - xPosition);
        if (distance < closestDistance)
        {
          closestDistance = distance;
          closestRank = rank;
        }
      }

      return closestRank;
    }

    public static RankValue? Increment(this RankValue rankValue)
    {
      return rankValue switch
      {
        RankValue.Rank1 => RankValue.Rank2,
        RankValue.Rank2 => RankValue.Rank3,
        RankValue.Rank3 => RankValue.Rank4,
        RankValue.Rank4 => RankValue.Rank5,
        RankValue.Rank5 => RankValue.Rank6,
        RankValue.Rank6 => RankValue.Rank7,
        RankValue.Rank7 => RankValue.Rank8,
        RankValue.Rank8 => null,
        _ => throw Errors.UnknownEnumValue(rankValue)
      };
    }

    public static IEnumerable<RankValue> AdjacentRanks(RankValue rankValue)
    {
      if ((int) rankValue > 1)
      {
        yield return rankValue - 1;
      }

      yield return rankValue;

      if ((int) rankValue < 8)
      {
        yield return rankValue + 1;
      }
    }

    public static float ToYPosition(this FileValue file)
    {
      return file switch
      {
        FileValue.File1 => TopY - (CellSpacing * 4),
        FileValue.File2 => TopY - (CellSpacing * 3),
        FileValue.File3 => TopY - (CellSpacing * 2),
        FileValue.File4 => TopY - (CellSpacing * 1),
        FileValue.File5 => TopY - (CellSpacing * 0),
        _ => throw Errors.UnknownEnumValue(file)
      };
    }

    public static float ToCenterYPosition(this FileValue file) => ToYPosition(file) + 1.25f;

    public static FileValue ClosestFileForYPosition(float yPosition)
    {
      var closestDistance = float.MaxValue;
      var closestFile = FileValue.File1;
      foreach (var file in AllFiles)
      {
        var distance = Mathf.Abs(file.ToYPosition() - yPosition);
        if (distance < closestDistance)
        {
          closestDistance = distance;
          closestFile = file;
        }
      }

      return closestFile;
    }

    public static IEnumerable<FileValue> AdjacentFiles(FileValue fileValue)
    {
      if ((int) fileValue > 1)
      {
        yield return fileValue - 1;
      }

      yield return fileValue;

      if ((int) fileValue < 5)
      {
        yield return fileValue + 1;
      }
    }

    public static IEnumerable<(RankValue, FileValue)> AllPositions() =>
      from rank in AllRanks
      from file in AllFiles
      select (rank, file);
  }
}