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

#nullable enable

namespace Nighthollow.World.Data
{
  [MessagePackObject]
  public sealed partial class FocusTreeData
  {
    public FocusTreeData(
      string name,
      ImmutableList<FocusNodeData>? nodes = null,
      ImmutableList<FocusTierData>? tiers = null)
    {
      Name = name;
      Nodes = nodes ?? ImmutableList<FocusNodeData>.Empty;
      Tiers = tiers ?? ImmutableList<FocusTierData>.Empty;
    }

    /// <summary>Identifier for this tree, currently only used for debugging.</summary>
    [Key(0)] public string Name { get; }

    [Key(1)] public ImmutableList<FocusNodeData> Nodes { get; }

    [Key(2)] public ImmutableList<FocusTierData> Tiers { get; }
  }
}