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
using Nighthollow.Utils;
using UnityEngine;

namespace Nighthollow.Data
{
  public static class BoardPositions
  {
    public static float ToXPosition(this RankValue rank)
    {
      switch (rank)
      {
        case RankValue.Rank1: return -8.8f;
        case RankValue.Rank2: return -6.3f;
        case RankValue.Rank3: return -3.8f;
        case RankValue.Rank4: return -1.3f;
        case RankValue.Rank5: return 1.2f;
        case RankValue.Rank6: return 3.7f;
        case RankValue.Rank7: return 6.2f;
        case RankValue.Rank8: return 8.7f;
        default: throw Errors.UnknownEnumValue(rank);
      }
    }

    public static int ToIndex(this RankValue rank)
    {
      switch (rank)
      {
        case RankValue.Rank1: return 0;
        case RankValue.Rank2: return 1;
        case RankValue.Rank3: return 2;
        case RankValue.Rank4: return 3;
        case RankValue.Rank5: return 4;
        case RankValue.Rank6: return 5;
        case RankValue.Rank7: return 6;
        case RankValue.Rank8: return 7;
        default: throw Errors.UnknownEnumValue(rank);
      }
    }

    public static RankValue ClosestRankForXPosition(float xPosition)
    {
      var closestDistance = float.MaxValue;
      var closestRank = RankValue.Rank1;
      foreach (RankValue rank in Enum.GetValues(typeof(RankValue)))
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

    public static RankValue RankForIndex(int index)
    {
      switch (index)
      {
        case 0: return RankValue.Rank1;
        case 1: return RankValue.Rank2;
        case 2: return RankValue.Rank3;
        case 3: return RankValue.Rank4;
        case 4: return RankValue.Rank5;
        case 5: return RankValue.Rank6;
        case 6: return RankValue.Rank7;
        case 7: return RankValue.Rank8;
        default: throw Errors.UnknownIntEnumValue(index, 0, 7);
      }
    }

    public static float ToYPosition(this FileValue file)
    {
      switch (file)
      {
        case FileValue.File1: return -9.8f;
        case FileValue.File2: return -7.3f;
        case FileValue.File3: return -4.8f;
        case FileValue.File4: return -2.3f;
        case FileValue.File5: return 0.2f;
        default: throw Errors.UnknownEnumValue(file);
      }
    }

    public static int ToIndex(this FileValue file)
    {
      switch (file)
      {
        case FileValue.File1: return 0;
        case FileValue.File2: return 1;
        case FileValue.File3: return 2;
        case FileValue.File4: return 3;
        case FileValue.File5: return 4;
        default: throw Errors.UnknownEnumValue(file);
      }
    }

    public static FileValue ClosestFileForYPosition(float yPosition)
    {
      var closestDistance = float.MaxValue;
      var closestFile = FileValue.File1;
      foreach (FileValue file in Enum.GetValues(typeof(FileValue)))
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
  }
}