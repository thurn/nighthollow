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
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Rules
{
  public interface IScope
  {
    T Get<T>(Key<T> key);
  }

  public interface IEffectScope : IScope
  {
    T Get<T>(MutatorKey<T> key);

    Scope ClearDependencies();
  }

  public sealed class Scope : IEffectScope
  {
    public static Builder CreateBuilder(Scope? parent = null) => new Builder(parent);

    readonly ImmutableHashSet<IKey> _dependencies;
    readonly ImmutableDictionary<IKey, object> _bindings;

    Scope(ImmutableHashSet<IKey> dependencies, ImmutableDictionary<IKey, object> bindings)
    {
      _dependencies = dependencies;
      _bindings = bindings;
    }

    public T Get<T>(Key<T> key) => (T) GetInternal(key);

    public T Get<T>(MutatorKey<T> key) => (T) GetInternal(key);

    public IEffectScope WithDependencies(ImmutableHashSet<IKey> dependencies) => new Scope(dependencies, _bindings);

    public Scope ClearDependencies() => new Scope(ImmutableHashSet<IKey>.Empty, _bindings);

    object GetInternal(IKey key)
    {
      Errors.CheckState(_dependencies.Contains(key), $"Key {key} was not registered as a dependency");
      Errors.CheckState(_bindings.ContainsKey(key), $"Binding not found for {key}");
      return _bindings[key];
    }

    public sealed class Builder
    {
      readonly ImmutableDictionary<IKey, object>.Builder _builder;

      public Builder(Scope? parent = null)
      {
        _builder = parent?._bindings.ToBuilder() ?? ImmutableDictionary.CreateBuilder<IKey, object>();
      }

      public Builder AddBinding<T>(Key<T> key, T value) where T : class =>
        AddBindingInternal(key, Errors.CheckNotNull(value));

      public Builder AddValueBinding<T>(Key<T> key, T value) where T : struct =>
        AddBindingInternal(key, value);

      public Builder AddBinding<T>(MutatorKey<T> key, T value) where T : class =>
        AddBindingInternal(key, Errors.CheckNotNull(value));

      public Builder AddValueBinding<T>(MutatorKey<T> key, T value) where T : struct =>
        AddBindingInternal(key, value);

      Builder AddBindingInternal(IKey key, object value)
      {
        _builder[key] = value;
        return this;
      }

      public Scope Build() => new Scope(ImmutableHashSet<IKey>.Empty, _builder.ToImmutable());
    }
  }
}
