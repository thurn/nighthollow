// Generated Code - Do not Edit!

using System.Collections.Immutable;
using Nighthollow.Delegates;
using Nighthollow.Services;
using Nighthollow.Stats;
using Nighthollow.State;
using Nighthollow.Rules;
using Nighthollow.World.Data;

#nullable enable

namespace Nighthollow.World.Data
{

  public sealed partial class HexPosition
  {
    public HexPosition WithX(int x) =>
      Equals(x, X)
        ? this
        : new HexPosition(
          x,
          Y);

    public HexPosition WithY(int y) =>
      Equals(y, Y)
        ? this
        : new HexPosition(
          X,
          y);

  }

  public sealed partial class ColorData
  {
    public ColorData WithRedComponent(int redComponent) =>
      Equals(redComponent, RedComponent)
        ? this
        : new ColorData(
          redComponent,
          GreenComponent,
          BlueComponent);

    public ColorData WithGreenComponent(int greenComponent) =>
      Equals(greenComponent, GreenComponent)
        ? this
        : new ColorData(
          RedComponent,
          greenComponent,
          BlueComponent);

    public ColorData WithBlueComponent(int blueComponent) =>
      Equals(blueComponent, BlueComponent)
        ? this
        : new ColorData(
          RedComponent,
          GreenComponent,
          blueComponent);

  }

  public sealed partial class KingdomData
  {
    public KingdomData WithName(KingdomName name) =>
      Equals(name, Name)
        ? this
        : new KingdomData(
          name,
          StartingPosition,
          TileImageAddress,
          Color);

    public KingdomData WithStartingPosition(HexPosition startingPosition) =>
      Equals(startingPosition, StartingPosition)
        ? this
        : new KingdomData(
          Name,
          startingPosition,
          TileImageAddress,
          Color);

    public KingdomData WithTileImageAddress(string tileImageAddress) =>
      Equals(tileImageAddress, TileImageAddress)
        ? this
        : new KingdomData(
          Name,
          StartingPosition,
          tileImageAddress,
          Color);

    public KingdomData WithColor(ColorData color) =>
      Equals(color, Color)
        ? this
        : new KingdomData(
          Name,
          StartingPosition,
          TileImageAddress,
          color);

  }

  public sealed partial class HexData
  {
    public HexData WithHexType(HexType hexType) =>
      Equals(hexType, HexType)
        ? this
        : new HexData(
          hexType,
          Position,
          OwningKingdom);

    public HexData WithPosition(HexPosition position) =>
      Equals(position, Position)
        ? this
        : new HexData(
          HexType,
          position,
          OwningKingdom);

    public HexData WithOwningKingdom(int? owningKingdom) =>
      Equals(owningKingdom, OwningKingdom)
        ? this
        : new HexData(
          HexType,
          Position,
          owningKingdom);

  }
}
