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

using Nighthollow.Services;
using Nighthollow.Utils;
using UnityEngine;

#nullable enable

namespace Nighthollow.Stats
{
  public interface ILifetime
  {
    bool IsValid(GameContext c);
  }

  public readonly struct WhileAliveLifetime : ILifetime
  {
    readonly CreatureId _creatureId;

    public WhileAliveLifetime(CreatureId creatureId)
    {
      _creatureId = creatureId;
    }

    public bool IsValid(GameContext c) => c.CreatureService.GetCreatureState(_creatureId).IsAlive;
  }

  public sealed class TimedLifetime : ILifetime
  {
    readonly float _endTimeSeconds;

    public TimedLifetime(DurationValue duration)
    {
      _endTimeSeconds = Time.time + Errors.CheckPositive(duration.AsSeconds());
    }

    public bool IsValid(GameContext c) => Time.time < _endTimeSeconds;
  }
}
