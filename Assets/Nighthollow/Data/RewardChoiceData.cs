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

#nullable enable

namespace Nighthollow.Data
{
  public sealed partial class RewardChoiceData
  {
    public RewardChoiceData(ImmutableList<IItemData> choices, ImmutableList<IItemData> fixedRewards)
    {
      Choices = choices;
      FixedRewards = fixedRewards;
    }

    /// <summary>The user may select from among these items.</summary>
    public ImmutableList<IItemData> Choices { get; }

    /// <summary>The user receives all of these items.</summary>
    public ImmutableList<IItemData> FixedRewards { get; }
  }
}