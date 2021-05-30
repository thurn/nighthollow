// Generated Code - Do not Edit!

using System.Collections.Immutable;
using Nighthollow.Delegates;
using Nighthollow.Services;
using Nighthollow.Stats;
using Nighthollow.State;
using Nighthollow.Rules;
using Nighthollow.Rules.Effects;
using Nighthollow.Rules.Events;
using Nighthollow.World.Data;

#nullable enable

namespace Nighthollow.Editing.Scenarios
{

  public sealed partial class ScenarioData
  {
    public ScenarioData WithName(string name) =>
      Equals(name, Name)
        ? this
        : new ScenarioData(
          name,
          Description,
          Scene,
          Effects,
          Repeating);

    public ScenarioData WithDescription(string description) =>
      Equals(description, Description)
        ? this
        : new ScenarioData(
          Name,
          description,
          Scene,
          Effects,
          Repeating);

    public ScenarioData WithScene(SceneName scene) =>
      Equals(scene, Scene)
        ? this
        : new ScenarioData(
          Name,
          Description,
          scene,
          Effects,
          Repeating);

    public ScenarioData WithEffects(ImmutableList<RuleEffect> effects) =>
      Equals(effects, Effects)
        ? this
        : new ScenarioData(
          Name,
          Description,
          Scene,
          effects,
          Repeating);

    public ScenarioData WithRepeating(bool repeating) =>
      Equals(repeating, Repeating)
        ? this
        : new ScenarioData(
          Name,
          Description,
          Scene,
          Effects,
          repeating);

  }
}
