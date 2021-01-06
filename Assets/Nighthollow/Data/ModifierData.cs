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
using Nighthollow.Generated;
using Nighthollow.Stats2;

#nullable enable

namespace Nighthollow.Data
{
  [MessagePackObject]
  public sealed partial class ModifierData
  {
    public ModifierData(
      DelegateId? delegateId,
      IStatModifier? statModifier,
      bool targeted)
    {
      DelegateId = delegateId;
      StatModifier = statModifier;
      Targeted = targeted;
    }

    [Key(0)] public DelegateId? DelegateId { get; }
    [Key(1)] public IStatModifier? StatModifier { get; }
    [Key(2)] public bool Targeted { get; }
  }
}
