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

using Nighthollow.Utils;
using UnityEngine;

namespace Nighthollow.Stats
{
  public readonly struct TimedModifier<TValue> : IModifier<TValue> where TValue : IStatValue
  {
    readonly StaticModifier<TValue> _modifier;
    readonly float _endTimeSeconds;

    public TimedModifier(StaticModifier<TValue> modifier, int durationMilliseconds)
    {
      _modifier = modifier;
      _endTimeSeconds = Time.time + (durationMilliseconds / 1000f);
    }

    TimedModifier(StaticModifier<TValue> modifier, float endTimeSeconds)
    {
      _modifier = modifier;
      _endTimeSeconds = endTimeSeconds;
    }

    public StaticModifier<TValue> BaseModifier => _modifier;

    public IModifier<T> Clone<T>() where T : IStatValue => Errors.CheckNotNull(this as IModifier<T>);

    public IModifier WithValue<TNew>(TNew value) where TNew : IStatValue =>
      new TimedModifier<TNew>((StaticModifier<TNew>) _modifier.WithValue(value), _endTimeSeconds);

    public IStatValue GetArgument() => BaseModifier.GetArgument();

    public bool IsDynamic() => true;

    public bool IsValid() => Time.time < _endTimeSeconds;
  }
}