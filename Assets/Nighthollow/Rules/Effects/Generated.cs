// Generated Code - Do not Edit!

using System.Collections.Immutable;
using Nighthollow.Data;
using Nighthollow.Delegates;
using Nighthollow.Services;
using Nighthollow.Stats;
using Nighthollow.State;
using Nighthollow.Rules;
using Nighthollow.Rules.Effects;
using Nighthollow.Rules.Events;
using Nighthollow.World.Data;

#nullable enable

namespace Nighthollow.Rules.Effects
{

  public sealed partial class CharacterDialogueEffect
  {
    public CharacterDialogueEffect WithCharacterName(CharacterName characterName) =>
      Equals(characterName, CharacterName)
        ? this
        : new CharacterDialogueEffect(
          characterName,
          Text,
          OnContinueRuleId);

    public CharacterDialogueEffect WithText(string text) =>
      Equals(text, Text)
        ? this
        : new CharacterDialogueEffect(
          CharacterName,
          text,
          OnContinueRuleId);

    public CharacterDialogueEffect WithOnContinueRuleId(int? onContinueRuleId) =>
      Equals(onContinueRuleId, OnContinueRuleId)
        ? this
        : new CharacterDialogueEffect(
          CharacterName,
          Text,
          onContinueRuleId);

  }

  public sealed partial class CenterCameraOnHexEffect
  {
    public CenterCameraOnHexEffect WithPosition(HexPosition position) =>
      Equals(position, Position)
        ? this
        : new CenterCameraOnHexEffect(
          position);

  }

  public sealed partial class DisplayHelpTextEffect
  {
    public DisplayHelpTextEffect WithText(string text) =>
      Equals(text, Text)
        ? this
        : new DisplayHelpTextEffect(
          text,
          XPosition,
          YPosition,
          ArrowDirection);

    public DisplayHelpTextEffect WithXPosition(int xPosition) =>
      Equals(xPosition, XPosition)
        ? this
        : new DisplayHelpTextEffect(
          Text,
          xPosition,
          YPosition,
          ArrowDirection);

    public DisplayHelpTextEffect WithYPosition(int yPosition) =>
      Equals(yPosition, YPosition)
        ? this
        : new DisplayHelpTextEffect(
          Text,
          XPosition,
          yPosition,
          ArrowDirection);

    public DisplayHelpTextEffect WithArrowDirection(Direction arrowDirection) =>
      Equals(arrowDirection, ArrowDirection)
        ? this
        : new DisplayHelpTextEffect(
          Text,
          XPosition,
          YPosition,
          arrowDirection);

  }

  public sealed partial class PlaceCreaturesFromListEffect
  {
    public PlaceCreaturesFromListEffect WithItemList(int itemList) =>
      Equals(itemList, ItemList)
        ? this
        : new PlaceCreaturesFromListEffect(
          itemList);

  }

  public sealed partial class LoadSceneEffect
  {
    public LoadSceneEffect WithSceneName(SceneName sceneName) =>
      Equals(sceneName, SceneName)
        ? this
        : new LoadSceneEffect(
          sceneName);

  }
}
