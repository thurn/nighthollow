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


using Nighthollow.Utils;
using Nighthollow.World.Data;
using UnityEngine;

#nullable enable

namespace Nighthollow.World
{
  public static class HexUtils
  {
    public enum Direction
    {
      UpLeft,
      UpRight,
      Left,
      Right,
      DownLeft,
      DownRight
    }

    public static bool AreAdjacent(HexPosition a, HexPosition b)
    {
      return GetInDirection(a, Direction.UpLeft) == b ||
             GetInDirection(a, Direction.UpRight) == b ||
             GetInDirection(a, Direction.Left) == b ||
             GetInDirection(a, Direction.Right) == b ||
             GetInDirection(a, Direction.DownLeft) == b ||
             GetInDirection(a, Direction.DownRight) == b;
    }

    public static HexPosition GetInDirection(HexPosition hex, Direction direction)
    {
      // This is an offset coordinate system
      // See: https://www.redblobgames.com/grids/hexagons/#coordinates

      if (hex.Y % 2 == 0)
      {
        return hex + direction switch
        {
          Direction.Right => new HexPosition(x: 1, y: 0),
          Direction.UpRight => new HexPosition(x: 0, y: 1),
          Direction.UpLeft => new HexPosition(x: -1, y: 1),
          Direction.Left => new HexPosition(x: -1, y: 0),
          Direction.DownLeft => new HexPosition(x: -1, y: -1),
          Direction.DownRight => new HexPosition(x: 0, y: -1),
          _ => throw Errors.UnknownEnumValue(direction)
        };
      }
      else
      {
        return hex + direction switch
        {
          Direction.Right => new HexPosition(x: 1, y: 0),
          Direction.UpRight => new HexPosition(x: 1, y: 1),
          Direction.UpLeft => new HexPosition(x: 0, y: 1),
          Direction.Left => new HexPosition(x: -1, y: 0),
          Direction.DownLeft => new HexPosition(x: 0, y: -1),
          Direction.DownRight => new HexPosition(x: 1, y: -1),
          _ => throw Errors.UnknownEnumValue(direction)
        };
      }
    }
  }
}