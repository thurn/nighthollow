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

// ReSharper disable UnusedTypeParameter

using System;

#nullable enable

namespace Nighthollow.Rules
{
  public interface IKey
  {
    string Name { get; }

    /// <summary>
    /// Should return the key to use for dependency tracking -- in a derived key this is the parent, otherwise it should
    /// just be this key.
    /// </summary>
    IKey Root { get; }
  }

  public sealed class ReaderKey<T> : IKey
  {
    public ReaderKey(string name)
    {
      Name = name;
    }

    public string Name { get; }
    public IKey Root => this;
  }

  public sealed class MutatorKey<T> : IKey
  {
    public MutatorKey(string name)
    {
      Name = name;
    }

    public string Name { get; }
    public IKey Root => this;
  }

  public sealed class DerivedKey<TParent, TChild> : IKey
  {
    public DerivedKey(string name, MutatorKey<TParent> parent, Func<TParent, TChild> function)
    {
      Name = name;
      Parent = parent;
      Function = function;
    }

    public string Name { get; }
    public MutatorKey<TParent> Parent { get; }
    public Func<TParent, TChild> Function { get; }
    public IKey Root => Parent;
  }
}