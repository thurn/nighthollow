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
using System.Linq;
using System.Reflection;
using MessagePack;
using Nighthollow.Data;
using Nighthollow.Services;
using Nighthollow.Triggers.Events;
using UnityEngine;

#nullable enable

namespace Nighthollow.Triggers
{
  [Union(0, typeof(TriggerData<SceneReadyEvent>))]
  public interface ITriggerData
  {
    public string? Name { get; }
    [NestedSheet] string EventDescription { get; }
    [NestedSheet] string ConditionsDescription { get; }
    [NestedSheet] string EffectsDescription { get; }
    bool Disabled { get; }

    public ITriggerData WithDisabled(bool disabled);
  }

  [MessagePackFormatter(typeof(TriggerDataFormatter<>))]
  public sealed class TriggerData<TEvent> : ITriggerData where TEvent : TriggerEvent
  {
    public TriggerData(
      string? name = null,
      ImmutableList<ICondition<TEvent>>? conditions = null,
      ImmutableList<IEffect<TEvent>>? effects = null,
      bool disabled = false)
    {
      Name = name;
      Conditions = conditions ?? ImmutableList<ICondition<TEvent>>.Empty;
      Effects = effects ?? ImmutableList<IEffect<TEvent>>.Empty;
      Disabled = disabled;
    }

    public string? Name { get; }
    public ImmutableList<ICondition<TEvent>> Conditions { get; }
    public ImmutableList<IEffect<TEvent>> Effects { get; }
    public bool Disabled { get; }

    public string EventDescription =>
      (string) typeof(TEvent)
        .GetProperty("Description", BindingFlags.Static | BindingFlags.Public)!
        .GetValue(typeof(TEvent));

    public string ConditionsDescription => $"If {string.Join("\nAnd ", Conditions.Select(c => c.Description))}";

    public string EffectsDescription => $"Then {string.Join("\nAnd ", Effects.Select(e => e.Description))}";

    /// <summary>
    /// Check trigger conditions and then fire effects if they all pass -- returns true if effects fired.
    /// </summary>
    public bool Invoke(TEvent triggerEvent, ServiceRegistry registry)
    {
      Debug.Log($"TriggerData::Invoke ");
      var gameData = registry.Database.Snapshot();

      if (Conditions.Any(condition => !condition.Satisfied(triggerEvent, gameData)))
      {
        return false;
      }

      foreach (var effect in Effects)
      {
        effect.Execute(triggerEvent, registry);
      }

      return true;
    }

    public ITriggerData WithName(string? name) => ReferenceEquals(name, Name)
      ? this
      : new TriggerData<TEvent>(name, Conditions, Effects, Disabled);

    public ITriggerData WithDisabled(bool disabled) =>
      disabled == Disabled ? this : new TriggerData<TEvent>(Name, Conditions, Effects, disabled);
  }
}