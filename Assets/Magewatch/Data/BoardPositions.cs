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

using System.Runtime.CompilerServices;
using Magewatch.Utils;
using UnityEngine;

namespace Magewatch.Data
{
  public enum RankValue
  {
    Rank0,
    Rank1,
    Rank2,
    Rank3,
    Rank4,
    Rank5
  }

  public enum FileValue
  {
    File0,
    File1,
    File2,
    File3,
    File4,
    File5
  }

  public sealed class BoardPosition
  {
    public readonly RankValue RankValue;
    public readonly FileValue FileValue;

    public BoardPosition(RankValue rankValue, FileValue fileValue)
    {
      RankValue = rankValue;
      FileValue = fileValue;
    }
  }

  public static class BoardPositions
  {
    public static int ToXPosition(this RankValue rank)
    {
      switch (rank)
      {
        case RankValue.Rank0: return -8;
        case RankValue.Rank1: return -7;
        case RankValue.Rank2: return -6;
        case RankValue.Rank3: return -5;
        case RankValue.Rank4: return -4;
        case RankValue.Rank5: return -3;
        default: throw Errors.UnknownEnumValue(rank);
      }
    }

    public static int ToIndex(this RankValue rank)
    {
      switch (rank)
      {
        case RankValue.Rank0: return 0;
        case RankValue.Rank1: return 1;
        case RankValue.Rank2: return 2;
        case RankValue.Rank3: return 3;
        case RankValue.Rank4: return 4;
        case RankValue.Rank5: return 5;
        default: throw Errors.UnknownEnumValue(rank);
      }
    }

    public static RankValue ClosestRankForXPosition(float xPosition)
    {
      var rounded = Mathf.RoundToInt(xPosition);
      switch (rounded)
      {
        case -8: return RankValue.Rank0;
        case -7: return RankValue.Rank1;
        case -6: return RankValue.Rank2;
        case -5: return RankValue.Rank3;
        case -4: return RankValue.Rank4;
        case -3: return RankValue.Rank5;
        default: return rounded < -8 ? RankValue.Rank0 : RankValue.Rank5;
      }
    }

    public static RankValue RankForIndex(int index)
    {
      switch (index)
      {
        case 0: return RankValue.Rank0;
        case 1: return RankValue.Rank1;
        case 2: return RankValue.Rank2;
        case 3: return RankValue.Rank3;
        case 4: return RankValue.Rank4;
        case 5: return RankValue.Rank5;
        default: throw Errors.UnknownIntEnumValue(index, 0, 5);
      }
    }

    public static int ToYPosition(this FileValue file)
    {
      switch (file)
      {
        case FileValue.File0: return -3;
        case FileValue.File1: return -2;
        case FileValue.File2: return -1;
        case FileValue.File3: return 0;
        case FileValue.File4: return 1;
        case FileValue.File5: return 2;
        default: throw Errors.UnknownEnumValue(file);
      }
    }

    public static FileValue ClosestFileForYPosition(float yPosition)
    {
      var rounded = Mathf.RoundToInt(yPosition);
      switch (rounded)
      {
        case -3: return FileValue.File0;
        case -2: return FileValue.File1;
        case -1: return FileValue.File2;
        case 0: return FileValue.File3;
        case 1: return FileValue.File4;
        case 2: return FileValue.File5;
        default: return rounded < -3 ? FileValue.File0 : FileValue.File5;
      }
    }
  }
}