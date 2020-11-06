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

using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Delegates.Effects
{
  public sealed class ApplyModifierEffect : Effect
  {
    public StatEntity Target { get; }
    public IStatModifier Modifier { get; }

    public ApplyModifierEffect(StatEntity target, IStatModifier modifier)
    {
      Target = target;
      Modifier = modifier;
    }

    public override void Execute()
    {
      Modifier.ApplyTo(Target.Stats);
    }
  }
}