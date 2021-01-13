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

using JetBrains.Annotations;

#nullable enable

namespace Nighthollow.State
{
  public interface IMutation
  {
    [MustUseReturnValue] KeyValueStore Mutate(KeyValueStore input);
  }

  public abstract class Mutation<T> : IMutation where T : notnull
  {
    protected Mutation(Key<T> key)
    {
      Key = key;
    }

    public Key<T> Key { get; }

    public abstract T NotFoundValue();

    public abstract T Apply(T currentValue);

    public KeyValueStore Mutate(KeyValueStore input) => input.Mutate(this);
  }
}
