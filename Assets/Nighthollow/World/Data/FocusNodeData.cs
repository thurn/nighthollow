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
using Nighthollow.Data;

#nullable enable

namespace Nighthollow.World.Data
{
  [MessagePackObject]
  public sealed partial class FocusNodeData
  {
    public FocusNodeData(
      string? name,
      string? imageAddress,
      int tier,
      int line,
      ImmutableList<ModifierData>? modifiers = null,
      ImmutableList<int>? connectionIndices = null)
    {
      Name = name;
      ImageAddress = imageAddress;
      Tier = tier;
      Line = line;
      Modifiers = modifiers ?? ImmutableList<ModifierData>.Empty;
      ConnectionIndices = connectionIndices ?? ImmutableList<int>.Empty;
    }

    /// <summary>User-visible name for this node</summary>
    [Key(0)] public string? Name { get; }

    /// <summary>Image for this node</summary>
    [Key(1)] public string? ImageAddress { get; }

    /// <summary>Tier for this node, controls its tree position and resource cost. Used as an index into the parent
    /// <see cref="FocusTreeData"/>'s tier list.
    /// </summary>
    [Key(2)] public int Tier { get; }

    /// <summary>
    /// Line number for this node. Nodes with a line number are automatically connected to every node in the
    /// *previous* tier with that line number and every node in the *next* tier with that line number.
    /// </summary>
    [Key(3)] public int Line { get; }

    /// <summary>Modifiers for this node, describing the effect of allocating it</summary>
    [Key(4)] public ImmutableList<ModifierData> Modifiers { get; }

    /// <summary>
    /// Indices of Focus Nodes within the parent <see cref="FocusTreeData"/>'s node list which this node should be
    /// connected to, in addition to the standard connections of nodes sharing the same line number. Nodes are
    /// considered connected if *either* node declares a connection to the other, bidirectional links are not required
    /// and have no special significance.
    /// </summary>
    [Key(5)] public ImmutableList<int> ConnectionIndices { get; }

    public override string ToString() => Name ?? "<Unnamed>";
  }
}