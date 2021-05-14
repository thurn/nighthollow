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
using Nighthollow.Data;

#nullable enable

namespace Nighthollow.World.Data
{
  [MessagePackObject]
  public sealed partial class HexData
  {
    public HexData(HexType hexType, HexPosition position, int? owningKingdom = null)
    {
      HexType = hexType;
      Position = position;
      OwningKingdom = owningKingdom;
    }

    [Key(0)] public HexType HexType { get; }
    [Key(1)] public HexPosition Position { get; }

    [ForeignKey(typeof(KingdomData))]
    [Key(2)] public int? OwningKingdom { get; }
  }
}