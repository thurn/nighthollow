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

using MessagePack;

#nullable enable

namespace Nighthollow.World.Data
{
  [MessagePackObject]
  public sealed partial class HexPosition
  {
    public HexPosition(int x, int y)
    {
      X = x;
      Y = y;
    }

    [Key(0)] public int X { get; }
    [Key(1)] public int Y { get; }

    public override string ToString() => $"{X}, {Y}";

    public static HexPosition operator +(HexPosition a) => a;
    public static HexPosition operator -(HexPosition a) => new HexPosition(-a.X, -a.Y);
    public static HexPosition operator +(HexPosition a, HexPosition b) => new HexPosition(a.X + b.X, a.Y + b.Y);
    public static HexPosition operator -(HexPosition a, HexPosition b) => new HexPosition(a.X - b.X, a.Y - b.Y);

    bool Equals(HexPosition other)
    {
      return X == other.X && Y == other.Y;
    }

    public override bool Equals(object? obj)
    {
      return ReferenceEquals(this, obj) || obj is HexPosition other && Equals(other);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (X * 397) ^ Y;
      }
    }

    public static bool operator ==(HexPosition? left, HexPosition? right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(HexPosition? left, HexPosition? right)
    {
      return !Equals(left, right);
    }
  }
}