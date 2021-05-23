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
using System.Collections.Immutable;
using System.Linq;

#nullable enable

namespace Nighthollow.Rules
{
  public sealed class RulesEngine
  {
    readonly ImmutableDictionary<TriggerName, ImmutableList<IRule>> _rules;
    readonly RulesEngine? _parent;

    public RulesEngine(ImmutableDictionary<TriggerName, ImmutableList<IRule>> rules, RulesEngine? parent)
    {
      _rules = rules;
      _parent = parent;
    }

    public IEnumerable<IEffect> Invoke<THandler>(Injector injector, ITriggerEvent<THandler> trigger) =>
      InvokeRules(injector, trigger).SelectMany(r => r);

    public TResult First<THandler, TResult>(
      Injector injector,
      ITrigger<THandler, TResult> trigger,
      TResult notFound) where THandler : IHandler
    {
      foreach (var result in InvokeRules(injector, trigger))
      {
        return result;
      }

      return notFound;
    }

    public TResult? FirstNonNull<THandler, TResult>(Injector injector, ITrigger<THandler, TResult?> trigger)
      where TResult : class =>
      InvokeRules(injector, trigger).FirstOrDefault(result => result != null);

    public bool Any<THandler>(Injector injector, ITrigger<THandler, bool> trigger) =>
      InvokeRules(injector, trigger).Any(b => b);

    IEnumerable<TResult> InvokeRules<THandler, TResult>(
      Injector injector,
      ITrigger<THandler, TResult> trigger,
      int index = 1)
    {
      foreach (var rule in _rules[trigger.Name])
      {
        yield return rule.Invoke(injector, index++, trigger);
      }

      if (_parent != null)
      {
        foreach (var result in _parent.InvokeRules(injector, trigger, index))
        {
          yield return result;
        }
      }
    }
  }
}