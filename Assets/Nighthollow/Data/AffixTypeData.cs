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
using Nighthollow.Generated;
using Nighthollow.Stats;

namespace Nighthollow.Data
{
  public sealed class ModifierRange
  {
    public ModifierData Modifier { get; }
    public Optional<IStat> Low { get; }
    public Optional<IStat> High { get; }
  }

  public sealed class AffixTypeData
  {
    public uint Id { get; }
    public uint MinLevel { get; }
    public uint Weight { get; }
    public AffixPool AffixPool { get; }
    public List<ModifierRange> Modifiers { get; }
  }
}