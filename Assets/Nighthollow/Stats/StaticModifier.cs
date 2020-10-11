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

namespace Nighthollow.Stats
{
  public readonly struct StaticModifier<TValue> : IModifier<TValue> where TValue : IStatValue
  {
    public readonly TValue Argument;

    public StaticModifier(TValue argument)
    {
      Argument = argument;
    }

    public StaticModifier<TValue> BaseModifier => this;

    public IModifier WithValue<TNew>(TNew value) where TNew : IStatValue => value.AsStaticModifier();

    public IStatValue GetArgument() => Argument;

    public bool IsDynamic() => false;

    public bool IsValid() => true;

    public IModifier<T> Clone<T>() where T : IStatValue => Errors.CheckNotNull(this as IModifier<T>);
  }
}