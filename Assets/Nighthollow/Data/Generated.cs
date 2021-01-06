// Generated Code - Do not Edit!

using System.Collections.Immutable;
using Nighthollow.Generated;
using Nighthollow.Stats2;

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

  public sealed partial class StatData
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

  public sealed partial class AffixData
  {
    public AffixData WithAffixTypeId(int affixTypeId) =>
      new AffixData(
        affixTypeId,
        Modifiers);

    public AffixData WithModifiers(ImmutableList<ModifierData> modifiers) =>
      new AffixData(
        AffixTypeId,
        modifiers);

  }

  public sealed partial class GameData
  {
    public GameData WithTableMetadata(ImmutableDictionary<int, TableMetadata> tableMetadata) =>
      new GameData(
        tableMetadata,
        CreatureTypes,
        AffixTypes,
        SkillTypes,
        StatData,
        CreatureLists,
        UserModifiers,
        Collection,
        Deck);

    public GameData WithCreatureTypes(ImmutableDictionary<int, CreatureTypeData> creatureTypes) =>
      new GameData(
        TableMetadata,
        creatureTypes,
        AffixTypes,
        SkillTypes,
        StatData,
        CreatureLists,
        UserModifiers,
        Collection,
        Deck);

    public GameData WithAffixTypes(ImmutableDictionary<int, AffixTypeData> affixTypes) =>
      new GameData(
        TableMetadata,
        CreatureTypes,
        affixTypes,
        SkillTypes,
        StatData,
        CreatureLists,
        UserModifiers,
        Collection,
        Deck);

    public GameData WithSkillTypes(ImmutableDictionary<int, SkillTypeData> skillTypes) =>
      new GameData(
        TableMetadata,
        CreatureTypes,
        AffixTypes,
        skillTypes,
        StatData,
        CreatureLists,
        UserModifiers,
        Collection,
        Deck);

    public GameData WithStatData(ImmutableDictionary<int, StatData> statData) =>
      new GameData(
        TableMetadata,
        CreatureTypes,
        AffixTypes,
        SkillTypes,
        statData,
        CreatureLists,
        UserModifiers,
        Collection,
        Deck);

    public GameData WithCreatureLists(ImmutableDictionary<int, StaticCreatureListData> creatureLists) =>
      new GameData(
        TableMetadata,
        CreatureTypes,
        AffixTypes,
        SkillTypes,
        StatData,
        creatureLists,
        UserModifiers,
        Collection,
        Deck);

    public GameData WithUserModifiers(ImmutableDictionary<int, ModifierData> userModifiers) =>
      new GameData(
        TableMetadata,
        CreatureTypes,
        AffixTypes,
        SkillTypes,
        StatData,
        CreatureLists,
        userModifiers,
        Collection,
        Deck);

    public GameData WithCollection(ImmutableDictionary<int, CreatureItemData> collection) =>
      new GameData(
        TableMetadata,
        CreatureTypes,
        AffixTypes,
        SkillTypes,
        StatData,
        CreatureLists,
        UserModifiers,
        collection,
        Deck);

    public GameData WithDeck(ImmutableDictionary<int, CreatureItemData> deck) =>
      new GameData(
        TableMetadata,
        CreatureTypes,
        AffixTypes,
        SkillTypes,
        StatData,
        CreatureLists,
        UserModifiers,
        Collection,
        deck);

  }

  public sealed partial class CreatureSkillAnimation
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
        ImplicitModifiers,
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
        ImplicitModifiers,
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
        ImplicitModifiers,
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
        ImplicitModifiers,
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
        ImplicitModifiers,
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
        ImplicitModifiers,
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
        ImplicitModifiers,
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
        ImplicitModifiers,
        ImplicitSkills,
        IsManaCreature);

    public CreatureTypeData WithImplicitModifiers(ImmutableList<ModifierTypeData>? implicitModifiers) =>
      new CreatureTypeData(
        Name,
        PrefabAddress,
        Owner,
        Health,
        SkillAnimations,
        ImageAddress,
        BaseManaCost,
        Speed,
        implicitModifiers,
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
        ImplicitModifiers,
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
        ImplicitModifiers,
        ImplicitSkills,
        isManaCreature);

  }

  public sealed partial class SkillTypeData
  {
    public SkillTypeData WithName(string name) =>
      new SkillTypeData(
        name,
        SkillAnimationType,
        SkillType,
        ImplicitModifiers,
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
        ImplicitModifiers,
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
        ImplicitModifiers,
        Address,
        ProjectileSpeed,
        UsesAccuracy,
        CanCrit,
        CanStun);

    public SkillTypeData WithImplicitModifiers(ImmutableList<ModifierTypeData>? implicitModifiers) =>
      new SkillTypeData(
        Name,
        SkillAnimationType,
        SkillType,
        implicitModifiers,
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
        ImplicitModifiers,
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
        ImplicitModifiers,
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
        ImplicitModifiers,
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
        ImplicitModifiers,
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
        ImplicitModifiers,
        Address,
        ProjectileSpeed,
        UsesAccuracy,
        CanCrit,
        canStun);

  }

  public sealed partial class SkillItemData
  {
    public SkillItemData WithSkillTypeId(int skillTypeId) =>
      new SkillItemData(
        skillTypeId,
        Affixes);

    public SkillItemData WithAffixes(ImmutableList<AffixData> affixes) =>
      new SkillItemData(
        SkillTypeId,
        affixes);

  }

  public sealed partial class ModifierData
  {
    public ModifierData WithDelegateId(DelegateId? delegateId) =>
      new ModifierData(
        delegateId,
        StatModifier,
        Targeted);

    public ModifierData WithModifier(IStatModifier? modifier) =>
      new ModifierData(
        DelegateId,
        modifier,
        Targeted);

    public ModifierData WithTargeted(bool targeted) =>
      new ModifierData(
        DelegateId,
        StatModifier,
        targeted);

  }

  public sealed partial class SkillData
  {
  }

  public sealed partial class StaticCreatureListData
  {
    public StaticCreatureListData WithName(string name) =>
      new StaticCreatureListData(
        name,
        List);

    public StaticCreatureListData WithList(ImmutableList<CreatureItemData> list) =>
      new StaticCreatureListData(
        Name,
        list);

  }

  public sealed partial class CreatureData
  {
  }

  public sealed partial class TableMetadata
  {
    public TableMetadata WithNextId(int nextId) =>
      new TableMetadata(
        nextId);

  }

  public sealed partial class CreatureItemData
  {
    public CreatureItemData WithCreatureTypeId(int creatureTypeId) =>
      new CreatureItemData(
        creatureTypeId,
        Name,
        School,
        Skills,
        Affixes);

    public CreatureItemData WithName(string name) =>
      new CreatureItemData(
        CreatureTypeId,
        name,
        School,
        Skills,
        Affixes);

    public CreatureItemData WithSchool(School school) =>
      new CreatureItemData(
        CreatureTypeId,
        Name,
        school,
        Skills,
        Affixes);

    public CreatureItemData WithSkills(ImmutableList<SkillItemData> skills) =>
      new CreatureItemData(
        CreatureTypeId,
        Name,
        School,
        skills,
        Affixes);

    public CreatureItemData WithAffixes(ImmutableList<AffixData> affixes) =>
      new CreatureItemData(
        CreatureTypeId,
        Name,
        School,
        Skills,
        affixes);

  }

  public sealed partial class ModifierTypeData
  {
    public ModifierTypeData WithStatId(StatId? statId) =>
      new ModifierTypeData(
        statId,
        ModifierType,
        DelegateId,
        ValueLow,
        ValueHigh,
        Targeted);

    public ModifierTypeData WithModifierType(ModifierType? modifierType) =>
      new ModifierTypeData(
        StatId,
        modifierType,
        DelegateId,
        ValueLow,
        ValueHigh,
        Targeted);

    public ModifierTypeData WithDelegateId(DelegateId? delegateId) =>
      new ModifierTypeData(
        StatId,
        ModifierType,
        delegateId,
        ValueLow,
        ValueHigh,
        Targeted);

    public ModifierTypeData WithValueLow(IValueData? valueLow) =>
      new ModifierTypeData(
        StatId,
        ModifierType,
        DelegateId,
        valueLow,
        ValueHigh,
        Targeted);

    public ModifierTypeData WithValueHigh(IValueData? valueHigh) =>
      new ModifierTypeData(
        StatId,
        ModifierType,
        DelegateId,
        ValueLow,
        valueHigh,
        Targeted);

    public ModifierTypeData WithTargeted(bool targeted) =>
      new ModifierTypeData(
        StatId,
        ModifierType,
        DelegateId,
        ValueLow,
        ValueHigh,
        targeted);

  }

  public sealed partial class UserData
  {
  }
}
