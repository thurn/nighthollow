// Generated Code - Do not Edit!

using System.Collections.Immutable;

#nullable enable

namespace Nighthollow.Triggers
{
  public sealed partial class Rule
  {
    public Rule WithTriggerEvent(EventType triggerEvent) =>
      Equals(triggerEvent, TriggerEvent)
        ? this
        : new Rule(
          triggerEvent,
          Name,
          Category,
          Conditions,
          Effects,
          OneTime,
          Disabled);

    public Rule WithName(string? name) =>
      Equals(name, Name)
        ? this
        : new Rule(
          TriggerEvent,
          name,
          Category,
          Conditions,
          Effects,
          OneTime,
          Disabled);

    public Rule WithCategory(TriggerCategory category) =>
      Equals(category, Category)
        ? this
        : new Rule(
          TriggerEvent,
          Name,
          category,
          Conditions,
          Effects,
          OneTime,
          Disabled);

    public Rule WithConditions(ImmutableList<ICondition> conditions) =>
      Equals(conditions, Conditions)
        ? this
        : new Rule(
          TriggerEvent,
          Name,
          Category,
          conditions,
          Effects,
          OneTime,
          Disabled);

    public Rule WithEffects(ImmutableList<IEffect> effects) =>
      Equals(effects, Effects)
        ? this
        : new Rule(
          TriggerEvent,
          Name,
          Category,
          Conditions,
          effects,
          OneTime,
          Disabled);

    public Rule WithOneTime(bool oneTime) =>
      Equals(oneTime, OneTime)
        ? this
        : new Rule(
          TriggerEvent,
          Name,
          Category,
          Conditions,
          Effects,
          oneTime,
          Disabled);

    public Rule WithDisabled(bool disabled) =>
      Equals(disabled, Disabled)
        ? this
        : new Rule(
          TriggerEvent,
          Name,
          Category,
          Conditions,
          Effects,
          OneTime,
          disabled);
  }
}