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

#nullable enable

namespace Nighthollow.Rules
{
  /** <summary>
   * Mutable object representing the output of invoking one or more rules. Can be passed to a RulesEngine when
   * an interesting result value may be obtained from a rule.
   * </summary>
   */
  public sealed class RuleOutput
  {
    /// <summary>
    /// Allows rules to request that the default behavior for the current event be prevented -- e.g. because they
    /// are going to supply their own behavior.
    /// </summary>
    public bool PreventDefault { get; set; }
  }
}
