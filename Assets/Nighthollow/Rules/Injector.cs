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

namespace Nighthollow.Rules
{
  public class Injector
  {
    public T Get<T>(Key<T> key) => default!;

    public Injector AddBinding<T>(Key<T> key, T value)
    {
      return this;
    }

    public Injector AddBinding<T>(MutatorKey<T> key, T value)
    {
      return this;
    }

    public Injector AddDependency<T>(Key<T> key)
    {
      return this;
    }
  }

  public sealed class EffectInjector : Injector
  {
    public EffectInjector AddDependency<T>(MutatorKey<T> key)
    {
      return this;
    }

    public T Get<T>(MutatorKey<T> key) => default!;
  }

}