// Generated Code - Do not Edit!

using System.Collections.Immutable;
using Nighthollow.Delegates.Core;
using Nighthollow.Stats;
using Nighthollow.State;

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
        ItemLists,
        UserModifiers,
        Collection,
        Deck,
        GameState,
        StatusEffects);

    public GameData WithCreatureTypes(ImmutableDictionary<int, CreatureTypeData> creatureTypes) =>
      new GameData(
        TableMetadata,
        creatureTypes,
        AffixTypes,
        SkillTypes,
        StatData,
        ItemLists,
        UserModifiers,
        Collection,
        Deck,
        GameState,
        StatusEffects);

    public GameData WithAffixTypes(ImmutableDictionary<int, AffixTypeData> affixTypes) =>
      new GameData(
        TableMetadata,
        CreatureTypes,
        affixTypes,
        SkillTypes,
        StatData,
        ItemLists,
        UserModifiers,
        Collection,
        Deck,
        GameState,
        StatusEffects);

    public GameData WithSkillTypes(ImmutableDictionary<int, SkillTypeData> skillTypes) =>
      new GameData(
        TableMetadata,
        CreatureTypes,
        AffixTypes,
        skillTypes,
        StatData,
        ItemLists,
        UserModifiers,
        Collection,
        Deck,
        GameState,
        StatusEffects);

    public GameData WithStatData(ImmutableDictionary<int, StatData> statData) =>
      new GameData(
        TableMetadata,
        CreatureTypes,
        AffixTypes,
        SkillTypes,
        statData,
        ItemLists,
        UserModifiers,
        Collection,
        Deck,
        GameState,
        StatusEffects);

    public GameData WithItemLists(ImmutableDictionary<int, StaticItemListData> itemLists) =>
      new GameData(
        TableMetadata,
        CreatureTypes,
        AffixTypes,
        SkillTypes,
        StatData,
        itemLists,
        UserModifiers,
        Collection,
        Deck,
        GameState,
        StatusEffects);

    public GameData WithUserModifiers(ImmutableDictionary<int, ModifierData> userModifiers) =>
      new GameData(
        TableMetadata,
        CreatureTypes,
        AffixTypes,
        SkillTypes,
        StatData,
        ItemLists,
        userModifiers,
        Collection,
        Deck,
        GameState,
        StatusEffects);

    public GameData WithCollection(ImmutableDictionary<int, CreatureItemData> collection) =>
      new GameData(
        TableMetadata,
        CreatureTypes,
        AffixTypes,
        SkillTypes,
        StatData,
        ItemLists,
        UserModifiers,
        collection,
        Deck,
        GameState,
        StatusEffects);

    public GameData WithDeck(ImmutableDictionary<int, CreatureItemData> deck) =>
      new GameData(
        TableMetadata,
        CreatureTypes,
        AffixTypes,
        SkillTypes,
        StatData,
        ItemLists,
        UserModifiers,
        Collection,
        deck,
        GameState,
        StatusEffects);

    public GameData WithGameState(GameState gameState) =>
      new GameData(
        TableMetadata,
        CreatureTypes,
        AffixTypes,
        SkillTypes,
        StatData,
        ItemLists,
        UserModifiers,
        Collection,
        Deck,
        gameState,
        StatusEffects);

    public GameData WithStatusEffects(ImmutableDictionary<int, StatusEffectTypeData> statusEffects) =>
      new GameData(
        TableMetadata,
        CreatureTypes,
        AffixTypes,
        SkillTypes,
        StatData,
        ItemLists,
        UserModifiers,
        Collection,
        Deck,
        GameState,
        statusEffects);

  }

  public sealed partial class CreatureSkillAnimation
  {
    public CreatureSkillAnimation WithSkillAnimationNumber(SkillAnimationNumber skillAnimationNumber) =>
      new CreatureSkillAnimation(
        skillAnimationNumber,
        SkillAnimationType,
        Duration,
        SkillTypeId);

    public CreatureSkillAnimation WithSkillAnimationType(SkillAnimationType skillAnimationType) =>
      new CreatureSkillAnimation(
        SkillAnimationNumber,
        skillAnimationType,
        Duration,
        SkillTypeId);

    public CreatureSkillAnimation WithDuration(DurationValue? duration) =>
      new CreatureSkillAnimation(
        SkillAnimationNumber,
        SkillAnimationType,
        duration,
        SkillTypeId);

    public CreatureSkillAnimation WithSkillTypeId(int? skillTypeId) =>
      new CreatureSkillAnimation(
        SkillAnimationNumber,
        SkillAnimationType,
        Duration,
        skillTypeId);

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
        SkillAnimations,
        IsManaCreature);

    public CreatureTypeData WithImplicitModifiers(ImmutableList<ModifierData> implicitModifiers) =>
      new CreatureTypeData(
        Name,
        PrefabAddress,
        Owner,
        Health,
        ImageAddress,
        BaseManaCost,
        Speed,
        implicitModifiers,
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
        SkillAnimations,
        isManaCreature);

  }

  public sealed partial class ResourceItemData
  {
    public ResourceItemData WithName(string name) =>
      new ResourceItemData(
        name,
        ImageAddress,
        Description);

    public ResourceItemData WithImageAddress(string imageAddress) =>
      new ResourceItemData(
        Name,
        imageAddress,
        Description);

    public ResourceItemData WithDescription(string description) =>
      new ResourceItemData(
        Name,
        ImageAddress,
        description);

  }

  public sealed partial class StaticItemListData
  {
    public StaticItemListData WithName(StaticItemListName name) =>
      new StaticItemListData(
        name,
        Creatures,
        Resources);

    public StaticItemListData WithCreatures(ImmutableList<CreatureItemData> creatures) =>
      new StaticItemListData(
        Name,
        creatures,
        Resources);

    public StaticItemListData WithResources(ImmutableList<ResourceItemData> resources) =>
      new StaticItemListData(
        Name,
        Creatures,
        resources);

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
        CanStun,
        StatusEffectIds);

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
        CanStun,
        StatusEffectIds);

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
        CanStun,
        StatusEffectIds);

    public SkillTypeData WithImplicitModifiers(ImmutableList<ModifierData> implicitModifiers) =>
      new SkillTypeData(
        Name,
        SkillAnimationType,
        SkillType,
        implicitModifiers,
        Address,
        ProjectileSpeed,
        UsesAccuracy,
        CanCrit,
        CanStun,
        StatusEffectIds);

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
        CanStun,
        StatusEffectIds);

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
        CanStun,
        StatusEffectIds);

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
        CanStun,
        StatusEffectIds);

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
        CanStun,
        StatusEffectIds);

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
        canStun,
        StatusEffectIds);

    public SkillTypeData WithStatusEffectIds(ImmutableList<int> statusEffectIds) =>
      new SkillTypeData(
        Name,
        SkillAnimationType,
        SkillType,
        ImplicitModifiers,
        Address,
        ProjectileSpeed,
        UsesAccuracy,
        CanCrit,
        CanStun,
        statusEffectIds);

  }

  public sealed partial class SkillItemData
  {
    public SkillItemData WithSkillTypeId(int skillTypeId) =>
      new SkillItemData(
        skillTypeId,
        Affixes,
        ImplicitModifiers,
        Name,
        Summons);

    public SkillItemData WithAffixes(ImmutableList<AffixData> affixes) =>
      new SkillItemData(
        SkillTypeId,
        affixes,
        ImplicitModifiers,
        Name,
        Summons);

    public SkillItemData WithImplicitModifiers(ImmutableList<ModifierData> implicitModifiers) =>
      new SkillItemData(
        SkillTypeId,
        Affixes,
        implicitModifiers,
        Name,
        Summons);

    public SkillItemData WithName(string name) =>
      new SkillItemData(
        SkillTypeId,
        Affixes,
        ImplicitModifiers,
        name,
        Summons);

    public SkillItemData WithSummons(ImmutableList<CreatureItemData> summons) =>
      new SkillItemData(
        SkillTypeId,
        Affixes,
        ImplicitModifiers,
        Name,
        summons);

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
    public SkillData WithDelegate(DelegateList @delegate) =>
      new SkillData(
        @delegate,
        Stats,
        BaseTypeId,
        BaseType,
        ItemData);

    public SkillData WithStats(StatTable stats) =>
      new SkillData(
        Delegate,
        stats,
        BaseTypeId,
        BaseType,
        ItemData);

    public SkillData WithBaseTypeId(int baseTypeId) =>
      new SkillData(
        Delegate,
        Stats,
        baseTypeId,
        BaseType,
        ItemData);

    public SkillData WithBaseType(SkillTypeData baseType) =>
      new SkillData(
        Delegate,
        Stats,
        BaseTypeId,
        baseType,
        ItemData);

    public SkillData WithItemData(SkillItemData itemData) =>
      new SkillData(
        Delegate,
        Stats,
        BaseTypeId,
        BaseType,
        itemData);

  }

  public sealed partial class CreatureData
  {
    public CreatureData WithDelegate(DelegateList @delegate) =>
      new CreatureData(
        @delegate,
        Stats,
        Skills,
        BaseType,
        ItemData,
        KeyValueStore);

    public CreatureData WithStats(StatTable stats) =>
      new CreatureData(
        Delegate,
        stats,
        Skills,
        BaseType,
        ItemData,
        KeyValueStore);

    public CreatureData WithSkills(ImmutableList<SkillData> skills) =>
      new CreatureData(
        Delegate,
        Stats,
        skills,
        BaseType,
        ItemData,
        KeyValueStore);

    public CreatureData WithBaseType(CreatureTypeData baseType) =>
      new CreatureData(
        Delegate,
        Stats,
        Skills,
        baseType,
        ItemData,
        KeyValueStore);

    public CreatureData WithItemData(CreatureItemData itemData) =>
      new CreatureData(
        Delegate,
        Stats,
        Skills,
        BaseType,
        itemData,
        KeyValueStore);

    public CreatureData WithKeyValueStore(KeyValueStore keyValueStore) =>
      new CreatureData(
        Delegate,
        Stats,
        Skills,
        BaseType,
        ItemData,
        keyValueStore);

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

  public sealed partial class StatusEffectTypeData
  {
    public StatusEffectTypeData WithName(string name) =>
      new StatusEffectTypeData(
        name,
        IsFirstClass,
        MaxStacks,
        ImplicitModifiers,
        Duration,
        ImageAddress,
        EffectAddress);

    public StatusEffectTypeData WithIsFirstClass(bool isFirstClass) =>
      new StatusEffectTypeData(
        Name,
        isFirstClass,
        MaxStacks,
        ImplicitModifiers,
        Duration,
        ImageAddress,
        EffectAddress);

    public StatusEffectTypeData WithMaxStacks(int maxStacks) =>
      new StatusEffectTypeData(
        Name,
        IsFirstClass,
        maxStacks,
        ImplicitModifiers,
        Duration,
        ImageAddress,
        EffectAddress);

    public StatusEffectTypeData WithImplicitModifiers(ImmutableList<ModifierData> implicitModifiers) =>
      new StatusEffectTypeData(
        Name,
        IsFirstClass,
        MaxStacks,
        implicitModifiers,
        Duration,
        ImageAddress,
        EffectAddress);

    public StatusEffectTypeData WithDuration(DurationValue? duration) =>
      new StatusEffectTypeData(
        Name,
        IsFirstClass,
        MaxStacks,
        ImplicitModifiers,
        duration,
        ImageAddress,
        EffectAddress);

    public StatusEffectTypeData WithImageAddress(string? imageAddress) =>
      new StatusEffectTypeData(
        Name,
        IsFirstClass,
        MaxStacks,
        ImplicitModifiers,
        Duration,
        imageAddress,
        EffectAddress);

    public StatusEffectTypeData WithEffectAddress(string? effectAddress) =>
      new StatusEffectTypeData(
        Name,
        IsFirstClass,
        MaxStacks,
        ImplicitModifiers,
        Duration,
        ImageAddress,
        effectAddress);

  }

  public sealed partial class CurrentEnemyState
  {
    public CurrentEnemyState WithEnemies(ImmutableList<CreatureItemData> enemies) =>
      new CurrentEnemyState(
        enemies);

  }

  public sealed partial class EnemyData
  {
    public EnemyData WithEnemies(ImmutableList<CreatureItemData> enemies) =>
      new EnemyData(
        enemies,
        Stats);

    public EnemyData WithStats(StatTable stats) =>
      new EnemyData(
        Enemies,
        stats);

  }

  public sealed partial class CreatureItemData
  {
    public CreatureItemData WithCreatureTypeId(int creatureTypeId) =>
      new CreatureItemData(
        creatureTypeId,
        Name,
        School,
        Skills,
        Affixes,
        ImplicitModifiers,
        Health,
        ManaCost,
        InfluenceCost,
        BaseDamage);

    public CreatureItemData WithName(string name) =>
      new CreatureItemData(
        CreatureTypeId,
        name,
        School,
        Skills,
        Affixes,
        ImplicitModifiers,
        Health,
        ManaCost,
        InfluenceCost,
        BaseDamage);

    public CreatureItemData WithSchool(School school) =>
      new CreatureItemData(
        CreatureTypeId,
        Name,
        school,
        Skills,
        Affixes,
        ImplicitModifiers,
        Health,
        ManaCost,
        InfluenceCost,
        BaseDamage);

    public CreatureItemData WithSkills(ImmutableList<SkillItemData> skills) =>
      new CreatureItemData(
        CreatureTypeId,
        Name,
        School,
        skills,
        Affixes,
        ImplicitModifiers,
        Health,
        ManaCost,
        InfluenceCost,
        BaseDamage);

    public CreatureItemData WithAffixes(ImmutableList<AffixData> affixes) =>
      new CreatureItemData(
        CreatureTypeId,
        Name,
        School,
        Skills,
        affixes,
        ImplicitModifiers,
        Health,
        ManaCost,
        InfluenceCost,
        BaseDamage);

    public CreatureItemData WithImplicitModifiers(ImmutableList<ModifierData> implicitModifiers) =>
      new CreatureItemData(
        CreatureTypeId,
        Name,
        School,
        Skills,
        Affixes,
        implicitModifiers,
        Health,
        ManaCost,
        InfluenceCost,
        BaseDamage);

    public CreatureItemData WithHealth(int health) =>
      new CreatureItemData(
        CreatureTypeId,
        Name,
        School,
        Skills,
        Affixes,
        ImplicitModifiers,
        health,
        ManaCost,
        InfluenceCost,
        BaseDamage);

    public CreatureItemData WithManaCost(int manaCost) =>
      new CreatureItemData(
        CreatureTypeId,
        Name,
        School,
        Skills,
        Affixes,
        ImplicitModifiers,
        Health,
        manaCost,
        InfluenceCost,
        BaseDamage);

    public CreatureItemData WithInfluenceCost(ImmutableDictionaryValue<School, int> influenceCost) =>
      new CreatureItemData(
        CreatureTypeId,
        Name,
        School,
        Skills,
        Affixes,
        ImplicitModifiers,
        Health,
        ManaCost,
        influenceCost,
        BaseDamage);

    public CreatureItemData WithBaseDamage(ImmutableDictionaryValue<DamageType, IntRangeValue> baseDamage) =>
      new CreatureItemData(
        CreatureTypeId,
        Name,
        School,
        Skills,
        Affixes,
        ImplicitModifiers,
        Health,
        ManaCost,
        InfluenceCost,
        baseDamage);

  }

  public sealed partial class GameState
  {
    public GameState WithTutorialState(TutorialState tutorialState) =>
      new GameState(
        tutorialState,
        CurrentEnemy);

    public GameState WithCurrentEnemy(CurrentEnemyState currentEnemy) =>
      new GameState(
        TutorialState,
        currentEnemy);

  }

  public sealed partial class UserData
  {
    public UserData WithState(GameState state) =>
      new UserData(
        state,
        Stats);

    public UserData WithStats(StatTable stats) =>
      new UserData(
        State,
        stats);

  }
}
