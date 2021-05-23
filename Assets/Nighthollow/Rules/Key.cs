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

using Nighthollow.Data;
using Nighthollow.Interface;
using Nighthollow.Services;
using Nighthollow.Triggers;

#nullable enable

namespace Nighthollow.Rules
{
  public sealed class Key<T>
  {
  }

  public sealed class MutatorKey<T>
  {
  }

  public static class Key
  {
    // Registry-scoped
    public static readonly MutatorKey<Database> Database = new MutatorKey<Database>();
    public static readonly MutatorKey<AssetService> AssetService = new MutatorKey<AssetService>();
    public static readonly MutatorKey<ScreenController> ScreenController = new MutatorKey<ScreenController>();
    public static readonly MutatorKey<TriggerService> TriggerService = new MutatorKey<TriggerService>();

    public static readonly MutatorKey<CreatureService.Controller> CreatureController =
      new MutatorKey<CreatureService.Controller>();

    // Rule-scoped
    public static readonly Key<CreatureState> Self = new Key<CreatureState>();
    public static readonly Key<SkillData> Skill = new Key<SkillData>();

    // Trigger-scoped
    public static readonly Key<int> FiringDelayMs = new Key<int>();
  }
}