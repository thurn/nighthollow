// Generated Code - Do not Edit!

using System.Collections.Immutable;
using Nighthollow.Generated;
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Data
{

  public sealed partial class AffixTypeData : IData
  {
    public AffixTypeData WithMinLevel(int minLevel) =>
      new AffixTypeData(
        minLevel,
        Weight,
        ManaCost,
        Modifiers,
        InfluenceType);

    public AffixTypeData WithWeight(int weight) =>
      new AffixTypeData(
        MinLevel,
        weight,
        ManaCost,
        Modifiers,
        InfluenceType);

    public AffixTypeData WithManaCost(IntRangeValue manaCost) =>
      new AffixTypeData(
        MinLevel,
        Weight,
        manaCost,
        Modifiers,
        InfluenceType);

    public AffixTypeData WithModifiers(ImmutableList<ModifierTypeData> modifiers) =>
      new AffixTypeData(
        MinLevel,
        Weight,
        ManaCost,
        modifiers,
        InfluenceType);

    public AffixTypeData WithInfluenceType(School? influenceType) =>
      new AffixTypeData(
        MinLevel,
        Weight,
        ManaCost,
        Modifiers,
        influenceType);

  }

  public sealed partial class StatData : IData
  {
    public StatData WithName(string name) =>
      new StatData(
        name,
        StatType,
        DefaultValue,
        Description,
        Comment);

    public StatData WithStatType(StatType statType) =>
      new StatData(
        Name,
        statType,
        DefaultValue,
        Description,
        Comment);

    public StatData WithDefaultValue(IValueData defaultValue) =>
      new StatData(
        Name,
        StatType,
        defaultValue,
        Description,
        Comment);

    public StatData WithDescription(string description) =>
      new StatData(
        Name,
        StatType,
        DefaultValue,
        description,
        Comment);

    public StatData WithComment(string comment) =>
      new StatData(
        Name,
        StatType,
        DefaultValue,
        Description,
        comment);

  }

  public sealed partial class GameData : IData
  {
    public GameData WithTableMetadata(ImmutableDictionary<int, TableMetadata> tableMetadata) =>
      new GameData(
        tableMetadata,
        CreatureTypes,
        AffixTypes,
        SkillTypes,
        StatData);

    public GameData WithCreatureTypes(ImmutableDictionary<int, CreatureTypeData> creatureTypes) =>
      new GameData(
        TableMetadata,
        creatureTypes,
        AffixTypes,
        SkillTypes,
        StatData);

    public GameData WithAffixTypes(ImmutableDictionary<int, AffixTypeData> affixTypes) =>
      new GameData(
        TableMetadata,
        CreatureTypes,
        affixTypes,
        SkillTypes,
        StatData);

    public GameData WithSkillTypes(ImmutableDictionary<int, SkillTypeData> skillTypes) =>
      new GameData(
        TableMetadata,
        CreatureTypes,
        AffixTypes,
        skillTypes,
        StatData);

    public GameData WithStatData(ImmutableDictionary<int, StatData> statData) =>
      new GameData(
        TableMetadata,
        CreatureTypes,
        AffixTypes,
        SkillTypes,
        statData);

  }

  public sealed partial class CreatureSkillAnimation : IData
  {
    public CreatureSkillAnimation WithSkillAnimationNumber(SkillAnimationNumber skillAnimationNumber) =>
      new CreatureSkillAnimation(
        skillAnimationNumber,
        SkillAnimationType);

    public CreatureSkillAnimation WithSkillAnimationType(SkillAnimationType skillAnimationType) =>
      new CreatureSkillAnimation(
        SkillAnimationNumber,
        skillAnimationType);

  }

  public sealed partial class CreatureTypeData : IData
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
        ImplicitSkills,
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
        ImplicitSkills,
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
        ImplicitSkills,
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
        ImplicitSkills,
        IsManaCreature);

    public CreatureTypeData WithSkillAnimations(ImmutableList<CreatureSkillAnimation> skillAnimations) =>
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
        ImplicitSkills,
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
        ImplicitSkills,
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
        ImplicitSkills,
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
        ImplicitSkills,
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
        ImplicitSkills,
        IsManaCreature);

    public CreatureTypeData WithImplicitSkills(ImmutableList<SkillTypeData> implicitSkills) =>
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
        implicitSkills,
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
        ImplicitSkills,
        isManaCreature);

  }

  public sealed partial class SkillTypeData : IData
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

  public sealed partial class TableMetadata : IData
  {
    public TableMetadata WithNextId(int nextId) =>
      new TableMetadata(
        nextId);

  }

  public sealed partial class ModifierTypeData : IData
  {
    public ModifierTypeData WithStatId(StatId? statId) =>
      new ModifierTypeData(
        statId,
        StatOperator,
        DelegateId,
        ValueLow,
        ValueHigh,
        Targeted);

    public ModifierTypeData WithStatOperator(Operator? statOperator) =>
      new ModifierTypeData(
        StatId,
        statOperator,
        DelegateId,
        ValueLow,
        ValueHigh,
        Targeted);

    public ModifierTypeData WithDelegateId(DelegateId? delegateId) =>
      new ModifierTypeData(
        StatId,
        StatOperator,
        delegateId,
        ValueLow,
        ValueHigh,
        Targeted);

    public ModifierTypeData WithValueLow(IValueData? valueLow) =>
      new ModifierTypeData(
        StatId,
        StatOperator,
        DelegateId,
        valueLow,
        ValueHigh,
        Targeted);

    public ModifierTypeData WithValueHigh(IValueData? valueHigh) =>
      new ModifierTypeData(
        StatId,
        StatOperator,
        DelegateId,
        ValueLow,
        valueHigh,
        Targeted);

    public ModifierTypeData WithTargeted(bool targeted) =>
      new ModifierTypeData(
        StatId,
        StatOperator,
        DelegateId,
        ValueLow,
        ValueHigh,
        targeted);

  }
}
