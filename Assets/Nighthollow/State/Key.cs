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
using Nighthollow.Components;
using Nighthollow.Services;

#nullable enable

namespace Nighthollow.State
{
  public static class Key
  {
    public static readonly KeyWithDefault<int> TimesChained =
      new KeyWithDefault<int>(1, defaultValue: 0);

    public static readonly KeyWithDefault<ImmutableList<CreatureId>> SkipProjectileImpacts =
      new KeyWithDefault<ImmutableList<CreatureId>>(2, defaultValue: ImmutableList<CreatureId>.Empty);

    public static readonly Key<CreatureId> LastCreatureHit = new Key<CreatureId>(3);

    public static readonly KeyWithDefault<int> SequentialHitCount =
      new KeyWithDefault<int>(4, defaultValue: 0);
  }

  public class Key<T> where T : notnull
  {
    public Key(int id)
    {
      Id = id;
    }

    public int Id { get; }

    public Mutation<T> Set(T newValue) => new SetValueMutation<T>(this, newValue);
  }

  public sealed class KeyWithDefault<T> : Key<T> where T : notnull
  {
    public KeyWithDefault(int id, T defaultValue) : base(id)
    {
      DefaultValue = defaultValue;
    }

    public T DefaultValue { get; }
  }
}
