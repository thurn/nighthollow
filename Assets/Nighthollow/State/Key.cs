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

using System.Collections.Generic;
using Nighthollow.Components;

#nullable enable

namespace Nighthollow.State
{
  public static class Key
  {
    public static readonly Key<int> TimesChained =
      new Key<int>(1);
    public static readonly Key<IReadOnlyList<Creature>> SkipProjectileImpacts =
      new Key<IReadOnlyList<Creature>>(2, new List<Creature>());
  }

  public sealed class Key<T>
  {
    public int Id { get;  }
    public T DefaultValue { get; }

    public Key(int id, T defaultValue = default)
    {
      Id = id;
      DefaultValue = defaultValue;
    }
  }
}