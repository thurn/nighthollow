// Generated Code - Do not Edit!

using System.Collections.Immutable;
using Nighthollow.Delegates.Core;
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

    public AffixTypeData WithModifiers(ImmutableList<ModifierData> modifiers) =>
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
        DescriptionTemplate,
        DefaultValue,
        TagType,
        StatValueType,
        Comment);

    public StatData WithStatType(StatType statType) =>
      new StatData(
        Name,
        statType,
        DescriptionTemplate,
        DefaultValue,
        TagType,
        StatValueType,
        Comment);

    public StatData WithDescriptionTemplate(string? descriptionTemplate) =>
      new StatData(
        Name,
        StatType,
        descriptionTemplate,
        DefaultValue,
        TagType,
        StatValueType,
        Comment);

    public StatData WithDefaultValue(IValueData? defaultValue) =>
      new StatData(
        Name,
        StatType,
        DescriptionTemplate,
        defaultValue,
        TagType,
        StatValueType,
        Comment);

    public StatData WithTagType(StatTagType? tagType) =>
      new StatData(
        Name,
        StatType,
        DescriptionTemplate,
        DefaultValue,
        tagType,
        StatValueType,
        Comment);

    public StatData WithStatValueType(StatType? statValueType) =>
      new StatData(
        Name,
        StatType,
        DescriptionTemplate,
        DefaultValue,
        TagType,
        statValueType,
        Comment);

    public StatData WithComment(string? comment) =>
      new StatData(
        Name,
        StatType,
        DescriptionTemplate,
        DefaultValue,
        TagType,
        StatValueType,
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
        SkillAnimationType,
        Duration);

    public CreatureSkillAnimation WithSkillAnimationType(SkillAnimationType skillAnimationType) =>
      new CreatureSkillAnimation(
        SkillAnimationNumber,
        skillAnimationType,
        Duration);

    public CreatureSkillAnimation WithDuration(DurationValue? duration) =>
      new CreatureSkillAnimation(
        SkillAnimationNumber,
        SkillAnimationType,
        duration);

  }

  public sealed partial class CreatureTypeData
  {
    public CreatureTypeData WithName(string name) =>
      new CreatureTypeData(
        name,
        PrefabAddress,
        Owner,
        Health,
        ImageAddress,
        BaseManaCost,
        Speed,
        ImplicitModifiers,
        ImplicitSkills,
        SkillAnimations,
        IsManaCreature);

    public CreatureTypeData WithPrefabAddress(string prefabAddress) =>
      new CreatureTypeData(
        Name,
        prefabAddress,
        Owner,
        Health,
        ImageAddress,
        BaseManaCost,
        Speed,
        ImplicitModifiers,
        ImplicitSkills,
        SkillAnimations,
        IsManaCreature);

    public CreatureTypeData WithOwner(PlayerName owner) =>
      new CreatureTypeData(
        Name,
        PrefabAddress,
        owner,
        Health,
        ImageAddress,
        BaseManaCost,
        Speed,
        ImplicitModifiers,
        ImplicitSkills,
        SkillAnimations,
        IsManaCreature);

    public CreatureTypeData WithHealth(IntRangeValue health) =>
      new CreatureTypeData(
        Name,
        PrefabAddress,
        Owner,
        health,
        ImageAddress,
        BaseManaCost,
        Speed,
        ImplicitModifiers,
        ImplicitSkills,
        SkillAnimations,
        IsManaCreature);

    public CreatureTypeData WithImageAddress(string? imageAddress) =>
      new CreatureTypeData(
        Name,
        PrefabAddress,
        Owner,
        Health,
        imageAddress,
        BaseManaCost,
        Speed,
        ImplicitModifiers,
        ImplicitSkills,
        SkillAnimations,
        IsManaCreature);

    public CreatureTypeData WithBaseManaCost(int baseManaCost) =>
      new CreatureTypeData(
        Name,
        PrefabAddress,
        Owner,
        Health,
        ImageAddress,
        baseManaCost,
        Speed,
        ImplicitModifiers,
        ImplicitSkills,
        SkillAnimations,
        IsManaCreature);

    public CreatureTypeData WithSpeed(int speed) =>
      new CreatureTypeData(
        Name,
        PrefabAddress,
        Owner,
        Health,
        ImageAddress,
        BaseManaCost,
        speed,
        ImplicitModifiers,
        ImplicitSkills,
        SkillAnimations,
        IsManaCreature);

    public CreatureTypeData WithImplicitModifiers(ImmutableList<ModifierData>? implicitModifiers) =>
      new CreatureTypeData(
        Name,
        PrefabAddress,
        Owner,
        Health,
        ImageAddress,
        BaseManaCost,
        Speed,
        implicitModifiers,
        ImplicitSkills,
        SkillAnimations,
        IsManaCreature);

    public CreatureTypeData WithImplicitSkills(ImmutableList<SkillTypeData> implicitSkills) =>
      new CreatureTypeData(
        Name,
        PrefabAddress,
        Owner,
        Health,
        ImageAddress,
        BaseManaCost,
        Speed,
        ImplicitModifiers,
        implicitSkills,
        SkillAnimations,
        IsManaCreature);

    public CreatureTypeData WithSkillAnimations(ImmutableList<CreatureSkillAnimation> skillAnimations) =>
      new CreatureTypeData(
        Name,
        PrefabAddress,
        Owner,
        Health,
        ImageAddress,
        BaseManaCost,
        Speed,
        ImplicitModifiers,
        ImplicitSkills,
        skillAnimations,
        IsManaCreature);

    public CreatureTypeData WithIsManaCreature(bool isManaCreature) =>
      new CreatureTypeData(
        Name,
        PrefabAddress,
        Owner,
        Health,
        ImageAddress,
        BaseManaCost,
        Speed,
        ImplicitModifiers,
        ImplicitSkills,
        SkillAnimations,
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

    public SkillTypeData WithImplicitModifiers(ImmutableList<ModifierData>? implicitModifiers) =>
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
    public ModifierData WithStatId(StatId? statId) =>
      new ModifierData(
        statId,
        ModifierType,
        DelegateId,
        Value,
        ValueLow,
        ValueHigh,
        Targeted);

    public ModifierData WithModifierType(ModifierType? modifierType) =>
      new ModifierData(
        StatId,
        modifierType,
        DelegateId,
        Value,
        ValueLow,
        ValueHigh,
        Targeted);

    public ModifierData WithDelegateId(DelegateId? delegateId) =>
      new ModifierData(
        StatId,
        ModifierType,
        delegateId,
        Value,
        ValueLow,
        ValueHigh,
        Targeted);

    public ModifierData WithValue(IValueData? value) =>
      new ModifierData(
        StatId,
        ModifierType,
        DelegateId,
        value,
        ValueLow,
        ValueHigh,
        Targeted);

    public ModifierData WithValueLow(IValueData? valueLow) =>
      new ModifierData(
        StatId,
        ModifierType,
        DelegateId,
        Value,
        valueLow,
        ValueHigh,
        Targeted);

    public ModifierData WithValueHigh(IValueData? valueHigh) =>
      new ModifierData(
        StatId,
        ModifierType,
        DelegateId,
        Value,
        ValueLow,
        valueHigh,
        Targeted);

    public ModifierData WithTargeted(bool targeted) =>
      new ModifierData(
        StatId,
        ModifierType,
        DelegateId,
        Value,
        ValueLow,
        ValueHigh,
        targeted);

  }

  public sealed partial class SkillData
  {
    public SkillData WithDelegate(IDelegate @delegate) =>
      new SkillData(
        @delegate,
        Stats,
        BaseType,
        ItemData);

    public SkillData WithStats(StatTable stats) =>
      new SkillData(
        Delegate,
        stats,
        BaseType,
        ItemData);

    public SkillData WithBaseType(SkillTypeData baseType) =>
      new SkillData(
        Delegate,
        Stats,
        baseType,
        ItemData);

    public SkillData WithItemData(SkillItemData itemData) =>
      new SkillData(
        Delegate,
        Stats,
        BaseType,
        itemData);

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
    public CreatureData WithDelegate(IDelegate @delegate) =>
      new CreatureData(
        @delegate,
        Stats,
        Skills,
        BaseType,
        ItemData);

    public CreatureData WithStats(StatTable stats) =>
      new CreatureData(
        Delegate,
        stats,
        Skills,
        BaseType,
        ItemData);

    public CreatureData WithSkills(ImmutableList<SkillData> skills) =>
      new CreatureData(
        Delegate,
        Stats,
        skills,
        BaseType,
        ItemData);

    public CreatureData WithBaseType(CreatureTypeData baseType) =>
      new CreatureData(
        Delegate,
        Stats,
        Skills,
        baseType,
        ItemData);

    public CreatureData WithItemData(CreatureItemData itemData) =>
      new CreatureData(
        Delegate,
        Stats,
        Skills,
        BaseType,
        itemData);

  }

  public sealed partial class ColumnMetadata
  {
    public ColumnMetadata WithColumnNumber(int columnNumber) =>
      new ColumnMetadata(
        columnNumber,
        Width);

    public ColumnMetadata WithWidth(int? width) =>
      new ColumnMetadata(
        ColumnNumber,
        width);

  }

  public sealed partial class TableMetadata
  {
    public TableMetadata WithNextId(int nextId) =>
      new TableMetadata(
        nextId,
        LastAccessedTime,
        ColumnMetadata);

    public TableMetadata WithLastAccessedTime(long lastAccessedTime) =>
      new TableMetadata(
        NextId,
        lastAccessedTime,
        ColumnMetadata);

    public TableMetadata WithColumnMetadata(ImmutableList<ColumnMetadata> columnMetadata) =>
      new TableMetadata(
        NextId,
        LastAccessedTime,
        columnMetadata);

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

  public sealed partial class UserData
  {
  }
}
