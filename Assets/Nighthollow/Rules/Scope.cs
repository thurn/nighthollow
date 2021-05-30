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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Rules
{
  public interface IScope
  {
    T Get<T>(ReaderKey<T> key);

    TChild Get<TParent, TChild>(DerivedKey<TParent, TChild> key);
  }

  public interface IEffectScope : IScope
  {
    T Get<T>(MutatorKey<T> key);

    Scope ClearDependencies();
  }

  public sealed class Scope : IEffectScope
  {
    public static Builder CreateBuilder(ImmutableHashSet<IKey> expectedKeys, Scope? parent = null) =>
      new Builder(expectedKeys, parent);

    readonly ImmutableHashSet<IKey> _dependencies;
    readonly ImmutableDictionary<IKey, object> _bindings;

    Scope(ImmutableHashSet<IKey> dependencies, ImmutableDictionary<IKey, object> bindings)
    {
      _dependencies = dependencies;
      _bindings = bindings;
    }

    public T Get<T>(ReaderKey<T> key) => (T) GetInternal(key);

    public T Get<T>(MutatorKey<T> key) => (T) GetInternal(key);

    public TChild Get<TParent, TChild>(DerivedKey<TParent, TChild> key) => key.Function(Get(key.Parent));

    public IEffectScope WithDependencies(ImmutableHashSet<IKey> dependencies) =>
      new Scope(dependencies.Select(k => k.Root).ToImmutableHashSet(), _bindings);

    public Scope ClearDependencies() => new Scope(ImmutableHashSet<IKey>.Empty, _bindings);

    object GetInternal(IKey key)
    {
        Errors.CheckState(_dependencies.Contains(key), $"Key {key.Name} was not registered as a dependency");
      Errors.CheckState(_bindings.ContainsKey(key), $"Binding not found for {key.Name}");
      return _bindings[key];
    }

    public sealed class Builder
    {
      readonly HashSet<IKey> _expectedKeys;
      readonly ImmutableDictionary<IKey, object>.Builder _builder;

      public Builder(ImmutableHashSet<IKey> expectedKeys, Scope? parent = null)
      {
        _expectedKeys = new HashSet<IKey>(expectedKeys);
        _builder = parent?._bindings.ToBuilder() ?? ImmutableDictionary.CreateBuilder<IKey, object>();
      }

      public Builder AddBinding<T>(ReaderKey<T> key, T value) where T : class =>
        AddBindingInternal(key, Errors.CheckNotNull(value));

      public Builder AddValueBinding<T>(ReaderKey<T> key, T value) where T : struct =>
        AddBindingInternal(key, value);

      public Builder AddBinding<T>(MutatorKey<T> key, T value) where T : class =>
        AddBindingInternal(key, Errors.CheckNotNull(value));

      public Builder AddValueBinding<T>(MutatorKey<T> key, T value) where T : struct =>
        AddBindingInternal(key, value);

      Builder AddBindingInternal(IKey key, object value)
      {
        Errors.CheckArgument(_expectedKeys.Contains(key), $"Unexpected key bound: {key.Name}");
        _builder[key] = value;
        _expectedKeys.Remove(key);
        return this;
      }

      public Scope Build()
      {
        if (_expectedKeys.Count != 0)
        {
          throw new InvalidOperationException(
            $"Scope builder did not bind keys: [{string.Join(", ", _expectedKeys.Select(k => k.Name))}]");
        }

        return new Scope(ImmutableHashSet<IKey>.Empty, _builder.ToImmutable());
      }
    }
  }
}