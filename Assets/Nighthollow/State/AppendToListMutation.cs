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

#nullable enable

namespace Nighthollow.State
{
  public sealed class AppendToListMutation<T> : Mutation<ImmutableList<T>> where T : notnull
  {
    readonly T _value;

    public AppendToListMutation(Key<ImmutableList<T>> key, T value) : base(key)
    {
      _value = value;
    }

    public override ImmutableList<T> NotFoundValue() => ImmutableList.Create(_value);

    public override ImmutableList<T> Apply(ImmutableList<T> currentValue) => currentValue.Add(_value);
  }
}
