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
using UnityEngine;

#nullable enable

namespace Nighthollow.World.Data
{
  /// <summary>
  /// Represents a color -- components are stored in CSS style, using integers in the (0,255) range.
  /// </summary>
  [MessagePackObject]
  public sealed partial class ColorData
  {
    public ColorData(int redComponent, int greenComponent, int blueComponent)
    {
      RedComponent = redComponent;
      GreenComponent = greenComponent;
      BlueComponent = blueComponent;
    }

    [Key(0)] public int RedComponent { get; }
    [Key(1)] public int GreenComponent { get; }
    [Key(2)] public int BlueComponent { get; }

    public Color AsUnityColor() => new Color(RedComponent / 255f, GreenComponent / 255f, BlueComponent / 255f);

    public override string ToString() => $"rgb({RedComponent}, {GreenComponent}, {BlueComponent})";
  }

  [MessagePackObject]
  public sealed partial class KingdomData
  {
    public KingdomData(KingdomName name, HexPosition startingPosition, string imageAddress, ColorData color)
    {
      Name = name;
      StartingPosition = startingPosition;
      ImageAddress = imageAddress;
      Color = color;
    }

    [Key(0)] public KingdomName Name { get; }
    [Key(1)] public HexPosition StartingPosition { get; }
    [Key(2)] public string ImageAddress { get; }
    [Key(3)] public ColorData Color { get; }
  }
}