// Generated Code - Do not Edit!

using System.Collections.Immutable;

#nullable enable

namespace Nighthollow.Rules
{
  public sealed partial class Rule
  {
    public Rule WithEventName(EventName eventName) =>
      Equals(eventName, EventName)
        ? this
        : new Rule(
          eventName,
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
          EventName,
          name,
          Category,
          Conditions,
          Effects,
          OneTime,
          Disabled);

    public Rule WithCategory(RuleCategory category) =>
      Equals(category, Category)
        ? this
        : new Rule(
          EventName,
          Name,
          category,
          Conditions,
          Effects,
          OneTime,
          Disabled);

    public Rule WithConditions(ImmutableList<RuleCondition> conditions) =>
      Equals(conditions, Conditions)
        ? this
        : new Rule(
          EventName,
          Name,
          Category,
          conditions,
          Effects,
          OneTime,
          Disabled);

    public Rule WithEffects(ImmutableList<RuleEffect> effects) =>
      Equals(effects, Effects)
        ? this
        : new Rule(
          EventName,
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
          EventName,
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
          EventName,
          Name,
          Category,
          Conditions,
          Effects,
          OneTime,
          disabled);
  }
}