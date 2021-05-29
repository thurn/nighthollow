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

using System.Collections.Immutable;
using MessagePack;
using Nighthollow.World.Data;
using UnityEngine;

#nullable enable

namespace Nighthollow.Rules.Effects
{
  [MessagePackObject]
  public sealed partial class CenterCameraOnHexEffect : RuleEffect
  {
    public static Description Describe => new Description(
      "center the camera on the hex",
      nameof(Position));

    public CenterCameraOnHexEffect(HexPosition position)
    {
      Position = position;
    }

    [Key(0)] public HexPosition Position { get; }

    public override ImmutableHashSet<IKey> GetDependencies() => ImmutableHashSet.Create<IKey>(
      Key.WorldMapRenderer,
      Key.MainCamera
    );

    public override void Execute(IEffectScope scope, RuleOutput? output)
    {
      var position = scope.Get(Key.WorldMapRenderer).GetWorldPosition(Position);
      var t = scope.Get(Key.MainCamera).transform;
      t.position = new Vector3(position.x, position.y, t.position.z);
    }
  }
}