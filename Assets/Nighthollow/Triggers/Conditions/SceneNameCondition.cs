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
using Nighthollow.Triggers.Events;

#nullable enable

namespace Nighthollow.Triggers.Conditions
{
  /// <summary>
  /// Checks the loaded scene's name.
  /// </summary>
  [MessagePackObject]
  public sealed class SceneNameCondition : EnumCondition<SceneReadyEvent, SceneReadyEvent.Name>
  {
    public SceneNameCondition(SceneReadyEvent.Name target, EnumOperator op) : base(target, op)
    {
    }

    protected override SceneReadyEvent.Name GetSource(SceneReadyEvent triggerEvent, GameData data) =>
      triggerEvent.SceneName;

    protected override EnumCondition<SceneReadyEvent, SceneReadyEvent.Name> Clone(
      SceneReadyEvent.Name target,
      EnumOperator op) =>
      new SceneNameCondition(target, op);

    public static Description Describe => new Description("that scene's name", nameof(Operator), nameof(Target));
  }
}