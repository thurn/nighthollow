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
using Nighthollow.Rules.Conditions;

#nullable enable

namespace Nighthollow.Rules
{
  [Union(0, typeof(UserDeckSizeCondition))]
  [Union(1, typeof(HotkeyEqualsCondition))]
  public abstract class RuleCondition : IHasDescription
  {
    public abstract Description Describe();

    public abstract ImmutableHashSet<IKey> GetDependencies();

    public abstract bool Satisfied(IScope scope);

    public override string ToString()
    {
      var description = Description.Describe(this);
      return description.Length < 100 ? description : $"{description.Substring(0, 100)}...";
    }
  }
}
