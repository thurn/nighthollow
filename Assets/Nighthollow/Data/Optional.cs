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

using Nighthollow.Utils;

namespace Nighthollow.Data
{
  public sealed class Optional<T> where T : class
  {
    readonly T _value;
    readonly bool _hasValue;

    public static Optional<T> Of(T value) => new Optional<T>(Errors.CheckNotNull(value), true);

    public static Optional<T> None() => new Optional<T>(null, false);

    Optional(T value, bool hasValue)
    {
      _value = value;
      _hasValue = hasValue;
    }

    public bool HasValue => _hasValue;

    public T Value
    {
      get
      {
        Errors.CheckState(_hasValue, "No value!");
        return _value;
      }
    }
  }
}