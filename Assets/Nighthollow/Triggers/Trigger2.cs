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
using Nighthollow.Data;
using Nighthollow.World;

#nullable enable

namespace Nighthollow.Triggers
{
  public sealed class Key<T>
  {
  }

  public sealed class Resolver
  {
    readonly List<Type> _inputTypes = new List<Type>();

    public T Get<T>()
    {
      return default!;
    }

    public void HandleEffect<TContext>(IEffect2<TContext> effect) where TContext : ITriggerContext, new()
    {
      var context = new TContext();
      context.AddInputs(this);
    }

    public void Add<T>(out T input)
    {
      _inputTypes.Add(typeof(T));
      input = default!;
    }
  }

  public interface ITriggerContext
  {
    Description Describe { get; }

    void AddInputs(Resolver resolver);
  }

  public interface IEffect2<in TContext> where TContext : ITriggerContext, new()
  {
    void Execute(TContext context);
  }

  public sealed class InitializeWorldMapEffectTriggerContext : ITriggerContext
  {
    public WorldMapRenderer WorldMapRenderer = null!;

    public Description Describe => new Description("initialize the world map");

    public void AddInputs(Resolver resolver)
    {
      resolver.Add(out WorldMapRenderer);
    }
  }
}