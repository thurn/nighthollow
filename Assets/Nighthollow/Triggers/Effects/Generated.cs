// Generated Code - Do not Edit!

using System.Collections.Immutable;
using Nighthollow.Delegates;
using Nighthollow.Services;
using Nighthollow.Stats;
using Nighthollow.State;
using Nighthollow.Triggers;

#nullable enable

namespace Nighthollow.Triggers.Effects
{

  public sealed partial class CharacterDialogueEffect
  {
    public CharacterDialogueEffect WithCharacterName(CharacterName characterName) =>
      Equals(characterName, CharacterName)
        ? this
        : new CharacterDialogueEffect(
          characterName,
          Text,
          Timeout);

    public CharacterDialogueEffect WithText(string text) =>
      Equals(text, Text)
        ? this
        : new CharacterDialogueEffect(
          CharacterName,
          text,
          Timeout);

    public CharacterDialogueEffect WithTimeout(DurationValue? timeout) =>
      Equals(timeout, Timeout)
        ? this
        : new CharacterDialogueEffect(
          CharacterName,
          Text,
          timeout);

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
}
