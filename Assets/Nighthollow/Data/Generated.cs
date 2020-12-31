// Generated Code - Do not Edit!

using System.Collections.Immutable;
using Nighthollow.Generated;
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Data
{

  public sealed partial class AffixTypeData
  {
    public AffixTypeData WithMinLevel(int minLevel) =>
      new AffixTypeData(
        minLevel,
        Weight,
        ManaCost,
        Modifiers,
        IsTargeted,
        InfluenceType);

    public AffixTypeData WithWeight(int weight) =>
      new AffixTypeData(
        MinLevel,
        weight,
        ManaCost,
        Modifiers,
        IsTargeted,
        InfluenceType);

    public AffixTypeData WithManaCost(IntRangeValue manaCost) =>
      new AffixTypeData(
        MinLevel,
        Weight,
        manaCost,
        Modifiers,
        IsTargeted,
        InfluenceType);

    public AffixTypeData WithModifiers(ImmutableList<ModifierTypeData> modifiers) =>
      new AffixTypeData(
        MinLevel,
        Weight,
        ManaCost,
        modifiers,
        IsTargeted,
        InfluenceType);

    public AffixTypeData WithIsTargeted(bool isTargeted) =>
      new AffixTypeData(
        MinLevel,
        Weight,
        ManaCost,
        Modifiers,
        isTargeted,
        InfluenceType);

    public AffixTypeData WithInfluenceType(School? influenceType) =>
      new AffixTypeData(
        MinLevel,
        Weight,
        ManaCost,
        Modifiers,
        IsTargeted,
        influenceType);

  }

  public sealed partial class GameData
  {
    public GameData WithCreatureTypes(ImmutableDictionary<int, CreatureTypeData> creatureTypes) =>
      new GameData(
        creatureTypes,
        AffixTypes,
        SkillTypes);

    public GameData WithAffixTypes(ImmutableDictionary<int, AffixTypeData> affixTypes) =>
      new GameData(
        CreatureTypes,
        affixTypes,
        SkillTypes);

    public GameData WithSkillTypes(ImmutableDictionary<int, SkillTypeData> skillTypes) =>
      new GameData(
        CreatureTypes,
        AffixTypes,
        skillTypes);

  }

  public sealed partial class CreatureTypeData
  {
    public CreatureTypeData WithName(string name) =>
      new CreatureTypeData(
        name,
        PrefabAddress,
        Owner,
        Health,
        SkillAnimations,
        ImageAddress,
        BaseManaCost,
        Speed,
        ImplicitAffix,
        ImplicitSkill,
        IsManaCreature);

    public CreatureTypeData WithPrefabAddress(string prefabAddress) =>
      new CreatureTypeData(
        Name,
        prefabAddress,
        Owner,
        Health,
        SkillAnimations,
        ImageAddress,
        BaseManaCost,
        Speed,
        ImplicitAffix,
        ImplicitSkill,
        IsManaCreature);

    public CreatureTypeData WithOwner(PlayerName owner) =>
      new CreatureTypeData(
        Name,
        PrefabAddress,
        owner,
        Health,
        SkillAnimations,
        ImageAddress,
        BaseManaCost,
        Speed,
        ImplicitAffix,
        ImplicitSkill,
        IsManaCreature);

    public CreatureTypeData WithHealth(IntRangeValue health) =>
      new CreatureTypeData(
        Name,
        PrefabAddress,
        Owner,
        health,
        SkillAnimations,
        ImageAddress,
        BaseManaCost,
        Speed,
        ImplicitAffix,
        ImplicitSkill,
        IsManaCreature);

    public CreatureTypeData WithSkillAnimations(ImmutableDictionary<SkillAnimationNumber, SkillAnimationType> skillAnimations) =>
      new CreatureTypeData(
        Name,
        PrefabAddress,
        Owner,
        Health,
        skillAnimations,
        ImageAddress,
        BaseManaCost,
        Speed,
        ImplicitAffix,
        ImplicitSkill,
        IsManaCreature);

    public CreatureTypeData WithImageAddress(string? imageAddress) =>
      new CreatureTypeData(
        Name,
        PrefabAddress,
        Owner,
        Health,
        SkillAnimations,
        imageAddress,
        BaseManaCost,
        Speed,
        ImplicitAffix,
        ImplicitSkill,
        IsManaCreature);

    public CreatureTypeData WithBaseManaCost(int baseManaCost) =>
      new CreatureTypeData(
        Name,
        PrefabAddress,
        Owner,
        Health,
        SkillAnimations,
        ImageAddress,
        baseManaCost,
        Speed,
        ImplicitAffix,
        ImplicitSkill,
        IsManaCreature);

    public CreatureTypeData WithSpeed(int speed) =>
      new CreatureTypeData(
        Name,
        PrefabAddress,
        Owner,
        Health,
        SkillAnimations,
        ImageAddress,
        BaseManaCost,
        speed,
        ImplicitAffix,
        ImplicitSkill,
        IsManaCreature);

    public CreatureTypeData WithImplicitAffix(AffixTypeData? implicitAffix) =>
      new CreatureTypeData(
        Name,
        PrefabAddress,
        Owner,
        Health,
        SkillAnimations,
        ImageAddress,
        BaseManaCost,
        Speed,
        implicitAffix,
        ImplicitSkill,
        IsManaCreature);

    public CreatureTypeData WithImplicitSkill(SkillTypeData? implicitSkill) =>
      new CreatureTypeData(
        Name,
        PrefabAddress,
        Owner,
        Health,
        SkillAnimations,
        ImageAddress,
        BaseManaCost,
        Speed,
        ImplicitAffix,
        implicitSkill,
        IsManaCreature);

    public CreatureTypeData WithIsManaCreature(bool isManaCreature) =>
      new CreatureTypeData(
        Name,
        PrefabAddress,
        Owner,
        Health,
        SkillAnimations,
        ImageAddress,
        BaseManaCost,
        Speed,
        ImplicitAffix,
        ImplicitSkill,
        isManaCreature);

  }

  public sealed partial class SkillTypeData
  {
    public SkillTypeData WithName(string name) =>
      new SkillTypeData(
        name,
        SkillAnimationType,
        SkillType,
        ImplicitAffix,
        Address,
        ProjectileSpeed,
        UsesAccuracy,
        CanCrit,
        CanStun);

    public SkillTypeData WithSkillAnimationType(SkillAnimationType skillAnimationType) =>
      new SkillTypeData(
        Name,
        skillAnimationType,
        SkillType,
        ImplicitAffix,
        Address,
        ProjectileSpeed,
        UsesAccuracy,
        CanCrit,
        CanStun);

    public SkillTypeData WithSkillType(SkillType skillType) =>
      new SkillTypeData(
        Name,
        SkillAnimationType,
        skillType,
        ImplicitAffix,
        Address,
        ProjectileSpeed,
        UsesAccuracy,
        CanCrit,
        CanStun);

    public SkillTypeData WithImplicitAffix(AffixTypeData? implicitAffix) =>
      new SkillTypeData(
        Name,
        SkillAnimationType,
        SkillType,
        implicitAffix,
        Address,
        ProjectileSpeed,
        UsesAccuracy,
        CanCrit,
        CanStun);

    public SkillTypeData WithAddress(string? address) =>
      new SkillTypeData(
        Name,
        SkillAnimationType,
        SkillType,
        ImplicitAffix,
        address,
        ProjectileSpeed,
        UsesAccuracy,
        CanCrit,
        CanStun);

    public SkillTypeData WithProjectileSpeed(int? projectileSpeed) =>
      new SkillTypeData(
        Name,
        SkillAnimationType,
        SkillType,
        ImplicitAffix,
        Address,
        projectileSpeed,
        UsesAccuracy,
        CanCrit,
        CanStun);

    public SkillTypeData WithUsesAccuracy(bool usesAccuracy) =>
      new SkillTypeData(
        Name,
        SkillAnimationType,
        SkillType,
        ImplicitAffix,
        Address,
        ProjectileSpeed,
        usesAccuracy,
        CanCrit,
        CanStun);

    public SkillTypeData WithCanCrit(bool canCrit) =>
      new SkillTypeData(
        Name,
        SkillAnimationType,
        SkillType,
        ImplicitAffix,
        Address,
        ProjectileSpeed,
        UsesAccuracy,
        canCrit,
        CanStun);

    public SkillTypeData WithCanStun(bool canStun) =>
      new SkillTypeData(
        Name,
        SkillAnimationType,
        SkillType,
        ImplicitAffix,
        Address,
        ProjectileSpeed,
        UsesAccuracy,
        CanCrit,
        canStun);

  }

  public sealed partial class ModifierTypeData
  {
    public ModifierTypeData WithStatId(StatId? statId) =>
      new ModifierTypeData(
        statId,
        StatOperator,
        DelegateId,
        ValueLow,
        ValueHigh);

    public ModifierTypeData WithStatOperator(Operator? statOperator) =>
      new ModifierTypeData(
        StatId,
        statOperator,
        DelegateId,
        ValueLow,
        ValueHigh);

    public ModifierTypeData WithDelegateId(DelegateId? delegateId) =>
      new ModifierTypeData(
        StatId,
        StatOperator,
        delegateId,
        ValueLow,
        ValueHigh);

    public ModifierTypeData WithValueLow(IValueData? valueLow) =>
      new ModifierTypeData(
        StatId,
        StatOperator,
        DelegateId,
        valueLow,
        ValueHigh);

    public ModifierTypeData WithValueHigh(IValueData? valueHigh) =>
      new ModifierTypeData(
        StatId,
        StatOperator,
        DelegateId,
        ValueLow,
        valueHigh);

  }
}
