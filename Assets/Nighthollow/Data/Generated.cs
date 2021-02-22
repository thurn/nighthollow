// Generated Code - Do not Edit!

using System.Collections.Immutable;
using Nighthollow.Components;
using Nighthollow.Delegates2.Core;
using Nighthollow.Stats;
using Nighthollow.State;

#nullable enable

namespace Nighthollow.Data
{

  public sealed partial class AffixTypeData
  {
    public AffixTypeData WithMinLevel(int minLevel) =>
      Equals(minLevel, MinLevel)
        ? this
        : new AffixTypeData(
          minLevel,
          Weight,
          ManaCost,
          Modifiers,
          InfluenceType);

    public AffixTypeData WithWeight(int weight) =>
      Equals(weight, Weight)
        ? this
        : new AffixTypeData(
          MinLevel,
          weight,
          ManaCost,
          Modifiers,
          InfluenceType);

    public AffixTypeData WithManaCost(IntRangeValue manaCost) =>
      Equals(manaCost, ManaCost)
        ? this
        : new AffixTypeData(
          MinLevel,
          Weight,
          manaCost,
          Modifiers,
          InfluenceType);

    public AffixTypeData WithModifiers(ImmutableList<ModifierData> modifiers) =>
      Equals(modifiers, Modifiers)
        ? this
        : new AffixTypeData(
          MinLevel,
          Weight,
          ManaCost,
          modifiers,
          InfluenceType);

    public AffixTypeData WithInfluenceType(School? influenceType) =>
      Equals(influenceType, InfluenceType)
        ? this
        : new AffixTypeData(
          MinLevel,
          Weight,
          ManaCost,
          Modifiers,
          influenceType);

  }

  public sealed partial class BattleData
  {
    public BattleData WithEnemies(ImmutableList<CreatureItemData> enemies) =>
      Equals(enemies, Enemies)
        ? this
        : new BattleData(
          enemies,
          EnemyModifiers,
          UserDeckOverride,
          EnemyListOverride);

    public BattleData WithEnemyModifiers(ImmutableList<ModifierData> enemyModifiers) =>
      Equals(enemyModifiers, EnemyModifiers)
        ? this
        : new BattleData(
          Enemies,
          enemyModifiers,
          UserDeckOverride,
          EnemyListOverride);

    public BattleData WithUserDeckOverride(int? userDeckOverride) =>
      Equals(userDeckOverride, UserDeckOverride)
        ? this
        : new BattleData(
          Enemies,
          EnemyModifiers,
          userDeckOverride,
          EnemyListOverride);

    public BattleData WithEnemyListOverride(int? enemyListOverride) =>
      Equals(enemyListOverride, EnemyListOverride)
        ? this
        : new BattleData(
          Enemies,
          EnemyModifiers,
          UserDeckOverride,
          enemyListOverride);

  }

  public sealed partial class StatData
  {
    public StatData WithName(string name) =>
      Equals(name, Name)
        ? this
        : new StatData(
          name,
          StatType,
          DescriptionTemplate,
          DefaultValue,
          TagType,
          StatValueType,
          Comment);

    public StatData WithStatType(StatType statType) =>
      Equals(statType, StatType)
        ? this
        : new StatData(
          Name,
          statType,
          DescriptionTemplate,
          DefaultValue,
          TagType,
          StatValueType,
          Comment);

    public StatData WithDescriptionTemplate(string? descriptionTemplate) =>
      Equals(descriptionTemplate, DescriptionTemplate)
        ? this
        : new StatData(
          Name,
          StatType,
          descriptionTemplate,
          DefaultValue,
          TagType,
          StatValueType,
          Comment);

    public StatData WithDefaultValue(IValueData? defaultValue) =>
      Equals(defaultValue, DefaultValue)
        ? this
        : new StatData(
          Name,
          StatType,
          DescriptionTemplate,
          defaultValue,
          TagType,
          StatValueType,
          Comment);

    public StatData WithTagType(StatTagType? tagType) =>
      Equals(tagType, TagType)
        ? this
        : new StatData(
          Name,
          StatType,
          DescriptionTemplate,
          DefaultValue,
          tagType,
          StatValueType,
          Comment);

    public StatData WithStatValueType(StatType? statValueType) =>
      Equals(statValueType, StatValueType)
        ? this
        : new StatData(
          Name,
          StatType,
          DescriptionTemplate,
          DefaultValue,
          TagType,
          statValueType,
          Comment);

    public StatData WithComment(string? comment) =>
      Equals(comment, Comment)
        ? this
        : new StatData(
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
      Equals(affixTypeId, AffixTypeId)
        ? this
        : new AffixData(
          affixTypeId,
          Modifiers);

    public AffixData WithModifiers(ImmutableList<ModifierData> modifiers) =>
      Equals(modifiers, Modifiers)
        ? this
        : new AffixData(
          AffixTypeId,
          modifiers);

  }

  public sealed partial class StatusEffectTypeData
  {
    public StatusEffectTypeData WithName(string name) =>
      Equals(name, Name)
        ? this
        : new StatusEffectTypeData(
          name,
          IsNamed,
          MaxStacks,
          ImplicitModifiers,
          Duration,
          DurationHigh,
          ImageAddress,
          EffectAddress);

    public StatusEffectTypeData WithIsNamed(bool isNamed) =>
      Equals(isNamed, IsNamed)
        ? this
        : new StatusEffectTypeData(
          Name,
          isNamed,
          MaxStacks,
          ImplicitModifiers,
          Duration,
          DurationHigh,
          ImageAddress,
          EffectAddress);

    public StatusEffectTypeData WithMaxStacks(int maxStacks) =>
      Equals(maxStacks, MaxStacks)
        ? this
        : new StatusEffectTypeData(
          Name,
          IsNamed,
          maxStacks,
          ImplicitModifiers,
          Duration,
          DurationHigh,
          ImageAddress,
          EffectAddress);

    public StatusEffectTypeData WithImplicitModifiers(ImmutableList<ModifierData> implicitModifiers) =>
      Equals(implicitModifiers, ImplicitModifiers)
        ? this
        : new StatusEffectTypeData(
          Name,
          IsNamed,
          MaxStacks,
          implicitModifiers,
          Duration,
          DurationHigh,
          ImageAddress,
          EffectAddress);

    public StatusEffectTypeData WithDuration(DurationValue? duration) =>
      Equals(duration, Duration)
        ? this
        : new StatusEffectTypeData(
          Name,
          IsNamed,
          MaxStacks,
          ImplicitModifiers,
          duration,
          DurationHigh,
          ImageAddress,
          EffectAddress);

    public StatusEffectTypeData WithDurationHigh(DurationValue? durationHigh) =>
      Equals(durationHigh, DurationHigh)
        ? this
        : new StatusEffectTypeData(
          Name,
          IsNamed,
          MaxStacks,
          ImplicitModifiers,
          Duration,
          durationHigh,
          ImageAddress,
          EffectAddress);

    public StatusEffectTypeData WithImageAddress(string? imageAddress) =>
      Equals(imageAddress, ImageAddress)
        ? this
        : new StatusEffectTypeData(
          Name,
          IsNamed,
          MaxStacks,
          ImplicitModifiers,
          Duration,
          DurationHigh,
          imageAddress,
          EffectAddress);

    public StatusEffectTypeData WithEffectAddress(string? effectAddress) =>
      Equals(effectAddress, EffectAddress)
        ? this
        : new StatusEffectTypeData(
          Name,
          IsNamed,
          MaxStacks,
          ImplicitModifiers,
          Duration,
          DurationHigh,
          ImageAddress,
          effectAddress);

  }

  public sealed partial class StatusEffectItemData
  {
    public StatusEffectItemData WithStatusEffectTypeId(int statusEffectTypeId) =>
      Equals(statusEffectTypeId, StatusEffectTypeId)
        ? this
        : new StatusEffectItemData(
          statusEffectTypeId,
          ImplicitModifiers,
          Duration);

    public StatusEffectItemData WithImplicitModifiers(ImmutableList<ModifierData> implicitModifiers) =>
      Equals(implicitModifiers, ImplicitModifiers)
        ? this
        : new StatusEffectItemData(
          StatusEffectTypeId,
          implicitModifiers,
          Duration);

    public StatusEffectItemData WithDuration(DurationValue? duration) =>
      Equals(duration, Duration)
        ? this
        : new StatusEffectItemData(
          StatusEffectTypeId,
          ImplicitModifiers,
          duration);

  }

  public sealed partial class GameData
  {
    public GameData WithBattleData(BattleData battleData) =>
      Equals(battleData, BattleData)
        ? this
        : new GameData(
          battleData,
          TableMetadata,
          CreatureTypes,
          AffixTypes,
          SkillTypes,
          StatData,
          ItemLists,
          UserModifiers,
          Collection,
          Deck,
          StatusEffects);

    public GameData WithTableMetadata(ImmutableDictionary<int, TableMetadata> tableMetadata) =>
      Equals(tableMetadata, TableMetadata)
        ? this
        : new GameData(
          BattleData,
          tableMetadata,
          CreatureTypes,
          AffixTypes,
          SkillTypes,
          StatData,
          ItemLists,
          UserModifiers,
          Collection,
          Deck,
          StatusEffects);

    public GameData WithCreatureTypes(ImmutableDictionary<int, CreatureTypeData> creatureTypes) =>
      Equals(creatureTypes, CreatureTypes)
        ? this
        : new GameData(
          BattleData,
          TableMetadata,
          creatureTypes,
          AffixTypes,
          SkillTypes,
          StatData,
          ItemLists,
          UserModifiers,
          Collection,
          Deck,
          StatusEffects);

    public GameData WithAffixTypes(ImmutableDictionary<int, AffixTypeData> affixTypes) =>
      Equals(affixTypes, AffixTypes)
        ? this
        : new GameData(
          BattleData,
          TableMetadata,
          CreatureTypes,
          affixTypes,
          SkillTypes,
          StatData,
          ItemLists,
          UserModifiers,
          Collection,
          Deck,
          StatusEffects);

    public GameData WithSkillTypes(ImmutableDictionary<int, SkillTypeData> skillTypes) =>
      Equals(skillTypes, SkillTypes)
        ? this
        : new GameData(
          BattleData,
          TableMetadata,
          CreatureTypes,
          AffixTypes,
          skillTypes,
          StatData,
          ItemLists,
          UserModifiers,
          Collection,
          Deck,
          StatusEffects);

    public GameData WithStatData(ImmutableDictionary<int, StatData> statData) =>
      Equals(statData, StatData)
        ? this
        : new GameData(
          BattleData,
          TableMetadata,
          CreatureTypes,
          AffixTypes,
          SkillTypes,
          statData,
          ItemLists,
          UserModifiers,
          Collection,
          Deck,
          StatusEffects);

    public GameData WithItemLists(ImmutableDictionary<int, StaticItemListData> itemLists) =>
      Equals(itemLists, ItemLists)
        ? this
        : new GameData(
          BattleData,
          TableMetadata,
          CreatureTypes,
          AffixTypes,
          SkillTypes,
          StatData,
          itemLists,
          UserModifiers,
          Collection,
          Deck,
          StatusEffects);

    public GameData WithUserModifiers(ImmutableDictionary<int, ModifierData> userModifiers) =>
      Equals(userModifiers, UserModifiers)
        ? this
        : new GameData(
          BattleData,
          TableMetadata,
          CreatureTypes,
          AffixTypes,
          SkillTypes,
          StatData,
          ItemLists,
          userModifiers,
          Collection,
          Deck,
          StatusEffects);

    public GameData WithCollection(ImmutableDictionary<int, CreatureItemData> collection) =>
      Equals(collection, Collection)
        ? this
        : new GameData(
          BattleData,
          TableMetadata,
          CreatureTypes,
          AffixTypes,
          SkillTypes,
          StatData,
          ItemLists,
          UserModifiers,
          collection,
          Deck,
          StatusEffects);

    public GameData WithDeck(ImmutableDictionary<int, CreatureItemData> deck) =>
      Equals(deck, Deck)
        ? this
        : new GameData(
          BattleData,
          TableMetadata,
          CreatureTypes,
          AffixTypes,
          SkillTypes,
          StatData,
          ItemLists,
          UserModifiers,
          Collection,
          deck,
          StatusEffects);

    public GameData WithStatusEffects(ImmutableDictionary<int, StatusEffectTypeData> statusEffects) =>
      Equals(statusEffects, StatusEffects)
        ? this
        : new GameData(
          BattleData,
          TableMetadata,
          CreatureTypes,
          AffixTypes,
          SkillTypes,
          StatData,
          ItemLists,
          UserModifiers,
          Collection,
          Deck,
          statusEffects);

  }

  public sealed partial class CreatureSkillAnimation
  {
    public CreatureSkillAnimation WithSkillAnimationNumber(SkillAnimationNumber skillAnimationNumber) =>
      Equals(skillAnimationNumber, SkillAnimationNumber)
        ? this
        : new CreatureSkillAnimation(
          skillAnimationNumber,
          SkillAnimationType,
          Duration,
          SkillTypeId);

    public CreatureSkillAnimation WithSkillAnimationType(SkillAnimationType skillAnimationType) =>
      Equals(skillAnimationType, SkillAnimationType)
        ? this
        : new CreatureSkillAnimation(
          SkillAnimationNumber,
          skillAnimationType,
          Duration,
          SkillTypeId);

    public CreatureSkillAnimation WithDuration(DurationValue? duration) =>
      Equals(duration, Duration)
        ? this
        : new CreatureSkillAnimation(
          SkillAnimationNumber,
          SkillAnimationType,
          duration,
          SkillTypeId);

    public CreatureSkillAnimation WithSkillTypeId(int? skillTypeId) =>
      Equals(skillTypeId, SkillTypeId)
        ? this
        : new CreatureSkillAnimation(
          SkillAnimationNumber,
          SkillAnimationType,
          Duration,
          skillTypeId);

  }

  public sealed partial class CreatureTypeData
  {
    public CreatureTypeData WithName(string name) =>
      Equals(name, Name)
        ? this
        : new CreatureTypeData(
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
      Equals(prefabAddress, PrefabAddress)
        ? this
        : new CreatureTypeData(
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
      Equals(owner, Owner)
        ? this
        : new CreatureTypeData(
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
      Equals(health, Health)
        ? this
        : new CreatureTypeData(
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
      Equals(imageAddress, ImageAddress)
        ? this
        : new CreatureTypeData(
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
      Equals(baseManaCost, BaseManaCost)
        ? this
        : new CreatureTypeData(
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
      Equals(speed, Speed)
        ? this
        : new CreatureTypeData(
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
      Equals(implicitModifiers, ImplicitModifiers)
        ? this
        : new CreatureTypeData(
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
      Equals(skillAnimations, SkillAnimations)
        ? this
        : new CreatureTypeData(
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
      Equals(isManaCreature, IsManaCreature)
        ? this
        : new CreatureTypeData(
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
      Equals(name, Name)
        ? this
        : new ResourceItemData(
          name,
          ImageAddress,
          Description);

    public ResourceItemData WithImageAddress(string imageAddress) =>
      Equals(imageAddress, ImageAddress)
        ? this
        : new ResourceItemData(
          Name,
          imageAddress,
          Description);

    public ResourceItemData WithDescription(string description) =>
      Equals(description, Description)
        ? this
        : new ResourceItemData(
          Name,
          ImageAddress,
          description);

  }

  public sealed partial class StaticItemListData
  {
    public StaticItemListData WithName(string name) =>
      Equals(name, Name)
        ? this
        : new StaticItemListData(
          name,
          Creatures,
          Resources);

    public StaticItemListData WithCreatures(ImmutableList<CreatureItemData> creatures) =>
      Equals(creatures, Creatures)
        ? this
        : new StaticItemListData(
          Name,
          creatures,
          Resources);

    public StaticItemListData WithResources(ImmutableList<ResourceItemData> resources) =>
      Equals(resources, Resources)
        ? this
        : new StaticItemListData(
          Name,
          Creatures,
          resources);

  }

  public sealed partial class SkillTypeData
  {
    public SkillTypeData WithName(string name) =>
      Equals(name, Name)
        ? this
        : new SkillTypeData(
          name,
          SkillAnimationType,
          SkillType,
          ImplicitModifiers,
          Address,
          ProjectileSpeed,
          UsesAccuracy,
          CanCrit,
          CanStun,
          SummonCreatures,
          StatusEffects,
          Cooldown,
          CooldownHigh);

    public SkillTypeData WithSkillAnimationType(SkillAnimationType skillAnimationType) =>
      Equals(skillAnimationType, SkillAnimationType)
        ? this
        : new SkillTypeData(
          Name,
          skillAnimationType,
          SkillType,
          ImplicitModifiers,
          Address,
          ProjectileSpeed,
          UsesAccuracy,
          CanCrit,
          CanStun,
          SummonCreatures,
          StatusEffects,
          Cooldown,
          CooldownHigh);

    public SkillTypeData WithSkillType(SkillType skillType) =>
      Equals(skillType, SkillType)
        ? this
        : new SkillTypeData(
          Name,
          SkillAnimationType,
          skillType,
          ImplicitModifiers,
          Address,
          ProjectileSpeed,
          UsesAccuracy,
          CanCrit,
          CanStun,
          SummonCreatures,
          StatusEffects,
          Cooldown,
          CooldownHigh);

    public SkillTypeData WithImplicitModifiers(ImmutableList<ModifierData> implicitModifiers) =>
      Equals(implicitModifiers, ImplicitModifiers)
        ? this
        : new SkillTypeData(
          Name,
          SkillAnimationType,
          SkillType,
          implicitModifiers,
          Address,
          ProjectileSpeed,
          UsesAccuracy,
          CanCrit,
          CanStun,
          SummonCreatures,
          StatusEffects,
          Cooldown,
          CooldownHigh);

    public SkillTypeData WithAddress(string? address) =>
      Equals(address, Address)
        ? this
        : new SkillTypeData(
          Name,
          SkillAnimationType,
          SkillType,
          ImplicitModifiers,
          address,
          ProjectileSpeed,
          UsesAccuracy,
          CanCrit,
          CanStun,
          SummonCreatures,
          StatusEffects,
          Cooldown,
          CooldownHigh);

    public SkillTypeData WithProjectileSpeed(int? projectileSpeed) =>
      Equals(projectileSpeed, ProjectileSpeed)
        ? this
        : new SkillTypeData(
          Name,
          SkillAnimationType,
          SkillType,
          ImplicitModifiers,
          Address,
          projectileSpeed,
          UsesAccuracy,
          CanCrit,
          CanStun,
          SummonCreatures,
          StatusEffects,
          Cooldown,
          CooldownHigh);

    public SkillTypeData WithUsesAccuracy(bool usesAccuracy) =>
      Equals(usesAccuracy, UsesAccuracy)
        ? this
        : new SkillTypeData(
          Name,
          SkillAnimationType,
          SkillType,
          ImplicitModifiers,
          Address,
          ProjectileSpeed,
          usesAccuracy,
          CanCrit,
          CanStun,
          SummonCreatures,
          StatusEffects,
          Cooldown,
          CooldownHigh);

    public SkillTypeData WithCanCrit(bool canCrit) =>
      Equals(canCrit, CanCrit)
        ? this
        : new SkillTypeData(
          Name,
          SkillAnimationType,
          SkillType,
          ImplicitModifiers,
          Address,
          ProjectileSpeed,
          UsesAccuracy,
          canCrit,
          CanStun,
          SummonCreatures,
          StatusEffects,
          Cooldown,
          CooldownHigh);

    public SkillTypeData WithCanStun(bool canStun) =>
      Equals(canStun, CanStun)
        ? this
        : new SkillTypeData(
          Name,
          SkillAnimationType,
          SkillType,
          ImplicitModifiers,
          Address,
          ProjectileSpeed,
          UsesAccuracy,
          CanCrit,
          canStun,
          SummonCreatures,
          StatusEffects,
          Cooldown,
          CooldownHigh);

    public SkillTypeData WithSummonCreatures(ImmutableList<int> summonCreatures) =>
      Equals(summonCreatures, SummonCreatures)
        ? this
        : new SkillTypeData(
          Name,
          SkillAnimationType,
          SkillType,
          ImplicitModifiers,
          Address,
          ProjectileSpeed,
          UsesAccuracy,
          CanCrit,
          CanStun,
          summonCreatures,
          StatusEffects,
          Cooldown,
          CooldownHigh);

    public SkillTypeData WithStatusEffects(ImmutableList<int> statusEffects) =>
      Equals(statusEffects, StatusEffects)
        ? this
        : new SkillTypeData(
          Name,
          SkillAnimationType,
          SkillType,
          ImplicitModifiers,
          Address,
          ProjectileSpeed,
          UsesAccuracy,
          CanCrit,
          CanStun,
          SummonCreatures,
          statusEffects,
          Cooldown,
          CooldownHigh);

    public SkillTypeData WithCooldown(DurationValue? cooldown) =>
      Equals(cooldown, Cooldown)
        ? this
        : new SkillTypeData(
          Name,
          SkillAnimationType,
          SkillType,
          ImplicitModifiers,
          Address,
          ProjectileSpeed,
          UsesAccuracy,
          CanCrit,
          CanStun,
          SummonCreatures,
          StatusEffects,
          cooldown,
          CooldownHigh);

    public SkillTypeData WithCooldownHigh(DurationValue? cooldownHigh) =>
      Equals(cooldownHigh, CooldownHigh)
        ? this
        : new SkillTypeData(
          Name,
          SkillAnimationType,
          SkillType,
          ImplicitModifiers,
          Address,
          ProjectileSpeed,
          UsesAccuracy,
          CanCrit,
          CanStun,
          SummonCreatures,
          StatusEffects,
          Cooldown,
          cooldownHigh);

  }

  public sealed partial class SkillItemData
  {
    public SkillItemData WithSkillTypeId(int skillTypeId) =>
      Equals(skillTypeId, SkillTypeId)
        ? this
        : new SkillItemData(
          skillTypeId,
          Affixes,
          ImplicitModifiers,
          Name,
          Summons,
          StatusEffects,
          Cooldown);

    public SkillItemData WithAffixes(ImmutableList<AffixData> affixes) =>
      Equals(affixes, Affixes)
        ? this
        : new SkillItemData(
          SkillTypeId,
          affixes,
          ImplicitModifiers,
          Name,
          Summons,
          StatusEffects,
          Cooldown);

    public SkillItemData WithImplicitModifiers(ImmutableList<ModifierData> implicitModifiers) =>
      Equals(implicitModifiers, ImplicitModifiers)
        ? this
        : new SkillItemData(
          SkillTypeId,
          Affixes,
          implicitModifiers,
          Name,
          Summons,
          StatusEffects,
          Cooldown);

    public SkillItemData WithName(string name) =>
      Equals(name, Name)
        ? this
        : new SkillItemData(
          SkillTypeId,
          Affixes,
          ImplicitModifiers,
          name,
          Summons,
          StatusEffects,
          Cooldown);

    public SkillItemData WithSummons(ImmutableList<CreatureItemData> summons) =>
      Equals(summons, Summons)
        ? this
        : new SkillItemData(
          SkillTypeId,
          Affixes,
          ImplicitModifiers,
          Name,
          summons,
          StatusEffects,
          Cooldown);

    public SkillItemData WithStatusEffects(ImmutableList<StatusEffectItemData> statusEffects) =>
      Equals(statusEffects, StatusEffects)
        ? this
        : new SkillItemData(
          SkillTypeId,
          Affixes,
          ImplicitModifiers,
          Name,
          Summons,
          statusEffects,
          Cooldown);

    public SkillItemData WithCooldown(DurationValue? cooldown) =>
      Equals(cooldown, Cooldown)
        ? this
        : new SkillItemData(
          SkillTypeId,
          Affixes,
          ImplicitModifiers,
          Name,
          Summons,
          StatusEffects,
          cooldown);

  }

  public sealed partial class ModifierData
  {
    public ModifierData WithStatId(StatId? statId) =>
      Equals(statId, StatId)
        ? this
        : new ModifierData(
          statId,
          ModifierType,
          DelegateId,
          Value,
          ValueLow,
          ValueHigh,
          Targeted);

    public ModifierData WithModifierType(ModifierType? modifierType) =>
      Equals(modifierType, ModifierType)
        ? this
        : new ModifierData(
          StatId,
          modifierType,
          DelegateId,
          Value,
          ValueLow,
          ValueHigh,
          Targeted);

    public ModifierData WithDelegateId(DelegateId? delegateId) =>
      Equals(delegateId, DelegateId)
        ? this
        : new ModifierData(
          StatId,
          ModifierType,
          delegateId,
          Value,
          ValueLow,
          ValueHigh,
          Targeted);

    public ModifierData WithValue(IValueData? value) =>
      Equals(value, Value)
        ? this
        : new ModifierData(
          StatId,
          ModifierType,
          DelegateId,
          value,
          ValueLow,
          ValueHigh,
          Targeted);

    public ModifierData WithValueLow(IValueData? valueLow) =>
      Equals(valueLow, ValueLow)
        ? this
        : new ModifierData(
          StatId,
          ModifierType,
          DelegateId,
          Value,
          valueLow,
          ValueHigh,
          Targeted);

    public ModifierData WithValueHigh(IValueData? valueHigh) =>
      Equals(valueHigh, ValueHigh)
        ? this
        : new ModifierData(
          StatId,
          ModifierType,
          DelegateId,
          Value,
          ValueLow,
          valueHigh,
          Targeted);

    public ModifierData WithTargeted(bool targeted) =>
      Equals(targeted, Targeted)
        ? this
        : new ModifierData(
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
      Equals(@delegate, Delegate)
        ? this
        : new SkillData(
          @delegate,
          Stats,
          BaseTypeId,
          BaseType,
          ItemData);

    public SkillData WithStats(StatTable stats) =>
      Equals(stats, Stats)
        ? this
        : new SkillData(
          Delegate,
          stats,
          BaseTypeId,
          BaseType,
          ItemData);

    public SkillData WithBaseTypeId(int baseTypeId) =>
      Equals(baseTypeId, BaseTypeId)
        ? this
        : new SkillData(
          Delegate,
          Stats,
          baseTypeId,
          BaseType,
          ItemData);

    public SkillData WithBaseType(SkillTypeData baseType) =>
      Equals(baseType, BaseType)
        ? this
        : new SkillData(
          Delegate,
          Stats,
          BaseTypeId,
          baseType,
          ItemData);

    public SkillData WithItemData(SkillItemData itemData) =>
      Equals(itemData, ItemData)
        ? this
        : new SkillData(
          Delegate,
          Stats,
          BaseTypeId,
          BaseType,
          itemData);

  }

  public sealed partial class CreatureData
  {
    public CreatureData WithDelegate(DelegateList @delegate) =>
      Equals(@delegate, Delegate)
        ? this
        : new CreatureData(
          @delegate,
          Stats,
          Skills,
          BaseType,
          ItemData,
          KeyValueStore);

    public CreatureData WithStats(StatTable stats) =>
      Equals(stats, Stats)
        ? this
        : new CreatureData(
          Delegate,
          stats,
          Skills,
          BaseType,
          ItemData,
          KeyValueStore);

    public CreatureData WithSkills(ImmutableList<SkillData> skills) =>
      Equals(skills, Skills)
        ? this
        : new CreatureData(
          Delegate,
          Stats,
          skills,
          BaseType,
          ItemData,
          KeyValueStore);

    public CreatureData WithBaseType(CreatureTypeData baseType) =>
      Equals(baseType, BaseType)
        ? this
        : new CreatureData(
          Delegate,
          Stats,
          Skills,
          baseType,
          ItemData,
          KeyValueStore);

    public CreatureData WithItemData(CreatureItemData itemData) =>
      Equals(itemData, ItemData)
        ? this
        : new CreatureData(
          Delegate,
          Stats,
          Skills,
          BaseType,
          itemData,
          KeyValueStore);

    public CreatureData WithKeyValueStore(KeyValueStore keyValueStore) =>
      Equals(keyValueStore, KeyValueStore)
        ? this
        : new CreatureData(
          Delegate,
          Stats,
          Skills,
          BaseType,
          ItemData,
          keyValueStore);

  }

  public sealed partial class CreatureState
  {
    public CreatureState WithCreature(Creature creature) =>
      Equals(creature, Creature)
        ? this
        : new CreatureState(
          creature,
          Data,
          Animation,
          RankPosition,
          FilePosition,
          CurrentSkill,
          Owner);

    public CreatureState WithData(CreatureData data) =>
      Equals(data, Data)
        ? this
        : new CreatureState(
          Creature,
          data,
          Animation,
          RankPosition,
          FilePosition,
          CurrentSkill,
          Owner);

    public CreatureState WithAnimation(CreatureAnimation animation) =>
      Equals(animation, Animation)
        ? this
        : new CreatureState(
          Creature,
          Data,
          animation,
          RankPosition,
          FilePosition,
          CurrentSkill,
          Owner);

    public CreatureState WithRankPosition(RankValue? rankPosition) =>
      Equals(rankPosition, RankPosition)
        ? this
        : new CreatureState(
          Creature,
          Data,
          Animation,
          rankPosition,
          FilePosition,
          CurrentSkill,
          Owner);

    public CreatureState WithFilePosition(FileValue? filePosition) =>
      Equals(filePosition, FilePosition)
        ? this
        : new CreatureState(
          Creature,
          Data,
          Animation,
          RankPosition,
          filePosition,
          CurrentSkill,
          Owner);

    public CreatureState WithCurrentSkill(SkillData? currentSkill) =>
      Equals(currentSkill, CurrentSkill)
        ? this
        : new CreatureState(
          Creature,
          Data,
          Animation,
          RankPosition,
          FilePosition,
          currentSkill,
          Owner);

    public CreatureState WithOwner(PlayerName owner) =>
      Equals(owner, Owner)
        ? this
        : new CreatureState(
          Creature,
          Data,
          Animation,
          RankPosition,
          FilePosition,
          CurrentSkill,
          owner);

  }

  public sealed partial class TableMetadata
  {
    public TableMetadata WithNextId(int nextId) =>
      Equals(nextId, NextId)
        ? this
        : new TableMetadata(
          nextId,
          LastAccessedTime);

    public TableMetadata WithLastAccessedTime(long lastAccessedTime) =>
      Equals(lastAccessedTime, LastAccessedTime)
        ? this
        : new TableMetadata(
          NextId,
          lastAccessedTime);

  }

  public sealed partial class EnemyData
  {
    public EnemyData WithEnemies(ImmutableList<CreatureItemData> enemies) =>
      Equals(enemies, Enemies)
        ? this
        : new EnemyData(
          enemies,
          Stats);

    public EnemyData WithStats(StatTable stats) =>
      Equals(stats, Stats)
        ? this
        : new EnemyData(
          Enemies,
          stats);

  }

  public sealed partial class CreatureItemData
  {
    public CreatureItemData WithCreatureTypeId(int creatureTypeId) =>
      Equals(creatureTypeId, CreatureTypeId)
        ? this
        : new CreatureItemData(
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
      Equals(name, Name)
        ? this
        : new CreatureItemData(
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
      Equals(school, School)
        ? this
        : new CreatureItemData(
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
      Equals(skills, Skills)
        ? this
        : new CreatureItemData(
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
      Equals(affixes, Affixes)
        ? this
        : new CreatureItemData(
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
      Equals(implicitModifiers, ImplicitModifiers)
        ? this
        : new CreatureItemData(
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
      Equals(health, Health)
        ? this
        : new CreatureItemData(
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
      Equals(manaCost, ManaCost)
        ? this
        : new CreatureItemData(
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
      Equals(influenceCost, InfluenceCost)
        ? this
        : new CreatureItemData(
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
      Equals(baseDamage, BaseDamage)
        ? this
        : new CreatureItemData(
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

  public sealed partial class UserData
  {
    public UserData WithStats(StatTable stats) =>
      Equals(stats, Stats)
        ? this
        : new UserData(
          stats);

  }
}
