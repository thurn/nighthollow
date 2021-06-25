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

namespace Nighthollow.World.Data
{

  public sealed partial class FocusNodeData
  {
    public FocusNodeData WithName(string? name) =>
      Equals(name, Name)
        ? this
        : new FocusNodeData(
          name,
          ImageAddress,
          Tier,
          Line,
          Modifiers,
          ConnectionIndices);

    public FocusNodeData WithImageAddress(string? imageAddress) =>
      Equals(imageAddress, ImageAddress)
        ? this
        : new FocusNodeData(
          Name,
          imageAddress,
          Tier,
          Line,
          Modifiers,
          ConnectionIndices);

    public FocusNodeData WithTier(int tier) =>
      Equals(tier, Tier)
        ? this
        : new FocusNodeData(
          Name,
          ImageAddress,
          tier,
          Line,
          Modifiers,
          ConnectionIndices);

    public FocusNodeData WithLine(int line) =>
      Equals(line, Line)
        ? this
        : new FocusNodeData(
          Name,
          ImageAddress,
          Tier,
          line,
          Modifiers,
          ConnectionIndices);

    public FocusNodeData WithModifiers(ImmutableList<ModifierData> modifiers) =>
      Equals(modifiers, Modifiers)
        ? this
        : new FocusNodeData(
          Name,
          ImageAddress,
          Tier,
          Line,
          modifiers,
          ConnectionIndices);

    public FocusNodeData WithConnectionIndices(ImmutableList<int> connectionIndices) =>
      Equals(connectionIndices, ConnectionIndices)
        ? this
        : new FocusNodeData(
          Name,
          ImageAddress,
          Tier,
          Line,
          Modifiers,
          connectionIndices);

  }

  public sealed partial class FocusTreeData
  {
    public FocusTreeData WithName(string name) =>
      Equals(name, Name)
        ? this
        : new FocusTreeData(
          name,
          Nodes,
          Tiers);

    public FocusTreeData WithNodes(ImmutableList<FocusNodeData> nodes) =>
      Equals(nodes, Nodes)
        ? this
        : new FocusTreeData(
          Name,
          nodes,
          Tiers);

    public FocusTreeData WithTiers(ImmutableList<FocusTierData> tiers) =>
      Equals(tiers, Tiers)
        ? this
        : new FocusTreeData(
          Name,
          Nodes,
          tiers);

  }

  public sealed partial class FocusTierData
  {
    public FocusTierData WithTierCost(ImmutableList<ResourceItemData> tierCost) =>
      Equals(tierCost, TierCost)
        ? this
        : new FocusTierData(
          tierCost);

  }

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
          OwningKingdom,
          FocusTree);

    public HexData WithPosition(HexPosition position) =>
      Equals(position, Position)
        ? this
        : new HexData(
          HexType,
          position,
          OwningKingdom,
          FocusTree);

    public HexData WithOwningKingdom(int? owningKingdom) =>
      Equals(owningKingdom, OwningKingdom)
        ? this
        : new HexData(
          HexType,
          Position,
          owningKingdom,
          FocusTree);

    public HexData WithFocusTree(FocusTreeData? focusTree) =>
      Equals(focusTree, FocusTree)
        ? this
        : new HexData(
          HexType,
          Position,
          OwningKingdom,
          focusTree);

  }
}
