// Copyright © 2020-present Derek Thurn

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//    https://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Magewatch.API;
using Magewatch.Services;
using UnityEngine;

// ReSharper disable UnusedMember.Global

namespace Magewatch.Components
{
  public sealed class DebugButtons : MonoBehaviour
  {
    // [SerializeField] int _idCounter;

    // void Start()
    // {
    //   _idCounter = 100;
    // }

    public void StartGame()
    {
      Root.Instance.NetworkService.MakeRequest(new Request
      {
        StartGame = new StartGameRequest()
      });
    }

    public void Console()
    {
      Root.Instance.NetworkService.MakeRequest(new Request
      {
        RunConsoleCommand = new RunConsoleCommandRequest()
      });
    }

    public void Advance()
    {
      Root.Instance.NetworkService.MakeRequest(new Request
      {
        AdvancePhase = new AdvancePhaseRequest()
      });
    }

    public void Basic()
    {
      Root.Instance.NetworkService.MakeRequest(new Request
      {
        RunRequestSequence = new MDebugRunRequestSequenceRequest
        {
          Requests =
          {
            LoadScenario("basic"),
            DrawHands(),
            PlayCreature(9, RankValue.Rank0, FileValue.File2, PlayerName.User),
            PlayCreature(11, RankValue.Rank0, FileValue.File3, PlayerName.User),
            PlayCreature(15, RankValue.Rank0, FileValue.File1, PlayerName.Enemy),
            PlayCreature(16, RankValue.Rank0, FileValue.File2, PlayerName.Enemy),
            PlayCreature(17, RankValue.Rank0, FileValue.File3, PlayerName.Enemy),
            PlayAttachment(12, 9, PlayerName.User),
            PlayAttachment(18, 15, PlayerName.Enemy),
            PlayScroll(13, PlayerName.User),
            PlayScroll(14, PlayerName.User),
            PlayScroll(19, PlayerName.Enemy),
            PlayCreature(10, RankValue.Rank0, FileValue.File4, PlayerName.User),
            PlayScroll(20, PlayerName.Enemy),
          }
        }
      });
    }

    public void Slow()
    {
      Time.timeScale = 0.1f;
    }

    CardId CardId(int id) => new CardId {Value = id};

    CreatureId CreatureId(int id) => new CreatureId {Value = id};

    Request LoadScenario(string scenario) => new Request
    {
      LoadScenario = new MDebugLoadScenarioRequest
      {
        ScenarioName = scenario,
      }
    };

    Request DrawHands() => new Request
    {
      DrawCards = new MDebugDrawCardsRequest
      {
        DrawUserCards = {0, 0, 1, 2, 3, 3},
        DrawEnemyCards = {0, 0, 1, 2, 3, 3}
      }
    };

    Request PlayCreature(int id, RankValue rank, FileValue file, PlayerName player) =>
      new Request
      {
        PlayCard = new PlayCardRequest
        {
          Player = player,
          CardId = CardId(id),
          PlayCreature = new PlayCreatureCard
          {
            RankPosition = rank,
            FilePosition = file
          }
        }
      };

    Request PlayAttachment(int id, int target, PlayerName player) =>
      new Request
      {
        PlayCard = new PlayCardRequest
        {
          Player = player,
          CardId = CardId(id),
          PlayAttachment = new PlayAttachmentCard
          {
            CreatureId = CreatureId(target)
          }
        }
      };

    Request PlayScroll(int id, PlayerName player) =>
      new Request
      {
        PlayCard = new PlayCardRequest
        {
          Player = player,
          CardId = CardId(id),
          PlayUntargeted = new PlayUntargetedCard()
        }
      };

    // public void Create()
    // {
    //   Run(StockCreatures());
    // }
    //
    // public void Engage()
    // {
    //   Run(StockCreatures(), C(MeleeEngage(1, 2), MeleeEngage(2, 1)));
    // }
    //
    // public void SimpleCombat()
    // {
    //   Run(
    //     StockCreatures(),
    //     C(MeleeEngage(1, 2),
    //       MeleeEngage(2, 1)),
    //     C(Attack(1, 2, 10, SkillAnimationNumber.Skill2),
    //       Attack(2, 1, 10, SkillAnimationNumber.Skill3)));
    // }
    //
    // public void BigCombat()
    // {
    //   Run(
    //     StockCreatures(),
    //     EveryoneEngage(),
    //     EveryoneAttack(),
    //     EveryoneAttack(),
    //     EveryoneAttack(),
    //     EveryoneAttack(),
    //     C(Attack(1, 2, 20, SkillAnimationNumber.Skill2, killsTarget: true),
    //       Attack(3, 1, 10, SkillAnimationNumber.Skill3, killsTarget: true),
    //       Attack(4, 5, 20, SkillAnimationNumber.Skill2, killsTarget: true)),
    //     C(MeleeEngage(3, 4), MeleeEngage(4, 3)),
    //     C(Attack(3, 4, 10, SkillAnimationNumber.Skill3), Attack(4, 3, 20, SkillAnimationNumber.Skill2)),
    //     C(Attack(3, 4, 10, SkillAnimationNumber.Skill3, killsTarget: true)),
    //     C(CastAtOpponent(3)),
    //     C(RemoveCreature(3)),
    //     C(UpdatePlayer(PlayerName.User, 24, 25, 0, 0)),
    //     C(Wait(1000)),
    //     StockCreatures()
    //   );
    // }
    //
    // public void DrawCard()
    // {
    //   Run(C(Draw(RageCard(_idCounter++))));
    // }
    //
    // public void DrawHands()
    // {
    //   Run(
    //     C(Draw(MageCard(50)),
    //       Draw(OpponentCard(51))),
    //     C(Draw(BerserkerCard(52)),
    //       Draw(OpponentCard(53))),
    //     C(Draw(RageCard(54)),
    //       Draw(OpponentCard(55))),
    //     C(Draw(FlameScrollCard(56)),
    //       Draw(OpponentCard(57))),
    //     C(Draw(KnowledgeCard(58)),
    //       Draw(OpponentCard(59))),
    //     C(Draw(FlameScrollCard(60)),
    //       Draw(OpponentCard(61))),
    //     C(MainButton("End Turn"))
    //   );
    // }
    //
    // public void Cast()
    // {
    //   Run(
    //     C(StockCreature(1), StockCreature(2)),
    //     C(CastSpell1(2, 1, 50), MeleeEngage(1, 2)),
    //     C(Attack(1, 2, 20, SkillAnimationNumber.Skill2), Attack(2, 1, 20, SkillAnimationNumber.Skill3)));
    // }
    //
    // public void AddMana()
    // {
    //   var player = new PlayerData
    //   {
    //     PlayerName = PlayerName.User,
    //     CurrentLife = 25,
    //     MaximumLife = 25,
    //     CurrentMana = 3,
    //     MaximumMana = 5
    //   };
    //   player.CurrentInfluence.Add(Influence(2, InfluenceType.Flame));
    //   player.MaximumInfluence.Add(Influence(4, InfluenceType.Flame));
    //
    //   Run(C(new Command
    //   {
    //     UpdatePlayer = new UpdatePlayerCommand
    //     {
    //       Player = player
    //     }
    //   }));
    // }
    //
    // public void OpponentTurn()
    // {
    //   Run(
    //     C(PlayCard(MageCard(51, PlayerName.Enemy), RankValue.Rank2, FileValue.File2)),
    //     C(CreateOrUpdate(NewCreature("Mage", 51, PlayerName.Enemy, 6, -1))),
    //     C(PlayCard(BerserkerCard(53, PlayerName.Enemy), RankValue.Rank3, FileValue.File1)),
    //     C(CreateOrUpdate(NewCreature("Berserker", 53, PlayerName.Enemy, 5, -2))),
    //     C(PlayCard(RageCard(55, PlayerName.Enemy), RankValue.Rank3, FileValue.File1)),
    //     C(CreateOrUpdate(NewCreature("Berserker", 53, PlayerName.Enemy, 5, -2, RageAttachment()))),
    //     C(PlayCard(FlameScrollCard(57, PlayerName.Enemy), RankValue.RankUnspecified, FileValue.FileUnspecified, 1000))
    //   );
    // }
    //
    // static CommandGroup C(params Command[] commands) => C(commands.ToList());
    //
    // static CommandGroup C(IEnumerable<Command> commands)
    // {
    //   var group = new CommandGroup();
    //   group.Commands.AddRange(commands);
    //   return group;
    // }
    //
    // static void Run(params CommandGroup[] groups) => Run(groups.ToList());
    //
    // static void Run(IEnumerable<CommandGroup> groups)
    // {
    //   var list = new CommandList();
    //   list.CommandGroups.AddRange(groups);
    //   Root.Instance.CommandService.HandleCommands(list);
    // }
    //
    // static CreatureId CreatureId(int id) => new CreatureId {Value = id};
    //
    // static CardId CardId(int id) => new CardId {Value = id};
    //
    // static Asset Prefab(string address) =>
    //   new Asset
    //   {
    //     Address = address,
    //     AssetType = AssetType.Prefab
    //   };
    //
    // static Asset Sprite(string address) =>
    //   new Asset
    //   {
    //     Address = address,
    //     AssetType = AssetType.Sprite
    //   };
    //
    // static AttachmentData RageAttachment() =>
    //   new AttachmentData
    //   {
    //     Image = Sprite("Spells/SpellBook01_01"),
    //   };
    //
    // static CommandGroup StockCreatures() =>
    //   C(StockCreature(1),
    //     StockCreature(2),
    //     StockCreature(3),
    //     StockCreature(4),
    //     StockCreature(5));
    //
    // static Command StockCreature(int id)
    // {
    //   var create = new CreateOrUpdateCreatureCommand();
    //   switch (id)
    //   {
    //     case 1:
    //       create.Creature = NewCreature("Berserker", 1, PlayerName.User, -4, -3, RageAttachment());
    //       break;
    //     case 2:
    //       create.Creature = NewCreature("Mage", 2, PlayerName.Enemy, 4, -3);
    //       break;
    //     case 3:
    //       create.Creature = NewCreature("Mage", 3, PlayerName.Enemy, 5, -3);
    //       break;
    //     case 4:
    //       create.Creature = NewCreature("Berserker", 4, PlayerName.User, -3, 2);
    //       break;
    //     case 5:
    //       create.Creature = NewCreature("Berserker", 5, PlayerName.Enemy, 4, 2);
    //       break;
    //   }
    //
    //   return new Command
    //   {
    //     CreateOrUpdateCreature = create
    //   };
    // }
    //
    // static CreatureData NewCreature(string name, int id, PlayerName owner, int? x = null, int? y = null,
    //   AttachmentData attachment = null)
    // {
    //   var result = new CreatureData
    //   {
    //     CreatureId = CreatureId(id),
    //     Prefab = Prefab($"Creatures/{name}"),
    //     Owner = owner,
    //     RankPosition = x.HasValue ? BoardPositions.ClosestRankForXPosition(x.Value, owner) : RankValue.RankUnspecified,
    //     FilePosition = y.HasValue ? BoardPositions.ClosestFileForYPosition(y.Value) : FileValue.FileUnspecified,
    //   };
    //
    //   if (attachment != null)
    //   {
    //     result.Attachments.Add(attachment);
    //   }
    //
    //   return result;
    // }
    //
    // static CommandGroup EveryoneEngage() =>
    //   C(MeleeEngage(1, 2),
    //     MeleeEngage(2, 1),
    //     MeleeEngage(3, 1),
    //     MeleeEngage(4, 5),
    //     MeleeEngage(5, 4));
    //
    // static Command MeleeEngage(int c1, int c2)
    // {
    //   return new Command
    //   {
    //     MeleeEngage = new MeleeEngageCommand
    //     {
    //       CreatureId = CreatureId(c1),
    //       TargetCreatureId = CreatureId(c2)
    //     }
    //   };
    // }
    //
    // static CommandGroup EveryoneAttack() =>
    //   C(Attack(1, 2, 20, SkillAnimationNumber.Skill2),
    //     Attack(2, 1, 10, SkillAnimationNumber.Skill3),
    //     Attack(3, 1, 10, SkillAnimationNumber.Skill3),
    //     Attack(4, 5, 20, SkillAnimationNumber.Skill2),
    //     Attack(5, 4, 20, SkillAnimationNumber.Skill2));
    //
    // static Command Attack(int c1, int c2, int damage, SkillAnimationNumber skill = SkillAnimationNumber.Skill1,
    //   bool killsTarget = false, int hitCount = 1)
    // {
    //   return new Command
    //   {
    //     Attack = new AttackCommand
    //     {
    //       CreatureId = CreatureId(c1),
    //       TargetCreatureId = CreatureId(c2),
    //       Skill = skill,
    //       HitCount = hitCount,
    //       ApplyDamage = new ApplyDamageEffect
    //       {
    //         Damage = damage,
    //         KillsTarget = killsTarget
    //       }
    //     }
    //   };
    // }
    //
    // static Command CastAtOpponent(int c1, SkillAnimationNumber skill = SkillAnimationNumber.Skill1)
    // {
    //   return new Command
    //   {
    //     Attack = new AttackCommand
    //     {
    //       CreatureId = CreatureId(c1),
    //       TargetCreatureId = CreatureId(c1),
    //       Skill = skill,
    //       HitCount = 1,
    //       FireProjectile = new FireProjectileEffect
    //       {
    //         Prefab = Prefab("Projectiles/Projectile 6"),
    //         ApplyDamage = new ApplyDamageEffect
    //         {
    //           Damage = 0,
    //           KillsTarget = false
    //         },
    //         AtOpponent = true
    //       }
    //     }
    //   };
    // }
    //
    // static Command RemoveCreature(int id)
    // {
    //   return new Command
    //   {
    //     RemoveCreature = new RemoveCreatureCommand
    //     {
    //       CreatureId = CreatureId(id)
    //     }
    //   };
    // }
    //
    // static Command UpdatePlayer(PlayerName playerName, int currentLife, int maximumLife, int currentMana,
    //   int maximumMana)
    // {
    //   return new Command
    //   {
    //     UpdatePlayer = new UpdatePlayerCommand
    //     {
    //       Player = new PlayerData
    //       {
    //         PlayerName = playerName,
    //         CurrentLife = currentLife,
    //         MaximumLife = maximumLife,
    //         CurrentMana = currentMana,
    //         MaximumMana = maximumMana
    //       }
    //     }
    //   };
    // }
    //
    // static Command Wait(int milliseconds)
    // {
    //   return new Command
    //   {
    //     Wait = new WaitCommand
    //     {
    //       WaitTimeMilliseconds = milliseconds
    //     }
    //   };
    // }
    //
    // static Command Draw(CardData card) => new Command
    // {
    //   DrawCard = new DrawCardCommand
    //   {
    //     Card = card
    //   }
    // };
    //
    // static Influence Influence(int value, InfluenceType influenceType)
    //   => new Influence
    //   {
    //     InfluenceType = influenceType,
    //     Value = value
    //   };
    //
    // static CardData MageCard(int id, PlayerName owner = PlayerName.User)
    // {
    //   var result = new CardData
    //   {
    //     CardId = CardId(id),
    //     Prefab = Prefab("Cards/FlameCard"),
    //     Name = "Mage",
    //     StandardCost = new StandardCost
    //     {
    //       ManaCost = 2,
    //     },
    //     Owner = owner,
    //     Image = Sprite("CreatureImages/Mage"),
    //     Text = new RichText
    //     {
    //       Text = "Whiz! Zoom!",
    //     },
    //     IsRevealed = true,
    //     CanBePlayed = owner == PlayerName.User,
    //     CreatureCard = NewCreature("Mage", id, PlayerName.User)
    //   };
    //   result.StandardCost.InfluenceCost.Add(Influence(1, InfluenceType.Flame));
    //   return result;
    // }
    //
    // static CardData BerserkerCard(int id, PlayerName owner = PlayerName.User)
    // {
    //   var result = new CardData
    //   {
    //     CardId = CardId(id),
    //     Prefab = Prefab("Cards/FlameCard"),
    //     Name = "Berserker",
    //     StandardCost = new StandardCost
    //     {
    //       ManaCost = 4,
    //     },
    //     Owner = owner,
    //     Image = Sprite("CreatureImages/Berserker"),
    //     Text = new RichText
    //     {
    //       Text = "Anger & Axes",
    //     },
    //     IsRevealed = true,
    //     CanBePlayed = owner == PlayerName.User,
    //     CreatureCard = NewCreature("Berserker", id, PlayerName.User)
    //   };
    //   result.StandardCost.InfluenceCost.Add(Influence(2, InfluenceType.Flame));
    //   return result;
    // }
    //
    // static CardData RageCard(int id, PlayerName owner = PlayerName.User)
    // {
    //   var result = new CardData
    //   {
    //     CardId = CardId(id),
    //     Prefab = Prefab("Cards/FlameCard"),
    //     Name = "Rage",
    //     StandardCost = new StandardCost
    //     {
    //       ManaCost = 3,
    //     },
    //     Owner = owner,
    //     Image = Sprite("Spells/SpellBook01_01"),
    //     Text = new RichText
    //     {
    //       Text = "Adds Bonus Damage on Hits",
    //     },
    //     IsRevealed = true,
    //     CanBePlayed = owner == PlayerName.User,
    //     AttachmentCard = new AttachmentData
    //     {
    //       Image = Sprite("Spells/SpellBook01_01")
    //     }
    //   };
    //   result.StandardCost.InfluenceCost.Add(Influence(1, InfluenceType.Flame));
    //   result.StandardCost.InfluenceCost.Add(Influence(1, InfluenceType.Light));
    //   return result;
    // }
    //
    // static CardData KnowledgeCard(int id, PlayerName owner = PlayerName.User)
    // {
    //   var result = new CardData
    //   {
    //     CardId = CardId(id),
    //     Prefab = Prefab("Cards/FlameCard"),
    //     Name = "Knowledge",
    //     StandardCost = new StandardCost
    //     {
    //       ManaCost = 3,
    //     },
    //     Owner = owner,
    //     Image = Sprite("Spells/SpellBook01_06"),
    //     Text = new RichText
    //     {
    //       Text = "Extra Mana",
    //     },
    //     IsRevealed = true,
    //     CanBePlayed = owner == PlayerName.User,
    //     AttachmentCard = new AttachmentData
    //     {
    //       Image = Sprite("Spells/SpellBook01_06")
    //     }
    //   };
    //   result.StandardCost.InfluenceCost.Add(Influence(2, InfluenceType.Light));
    //   return result;
    // }
    //
    // static CardData FlameScrollCard(int id, PlayerName owner = PlayerName.User)
    // {
    //   return new CardData
    //   {
    //     CardId = CardId(id),
    //     Prefab = Prefab("Cards/FlameCard"),
    //     Name = "Flame Scroll",
    //     NoCost = new NoCost(),
    //     Owner = owner,
    //     Image = Sprite("Scrolls/ScrollsAndBooks_21_t"),
    //     Text = new RichText
    //     {
    //       Text = "Adds 1 mana and 1 flame influence",
    //     },
    //     IsRevealed = true,
    //     CanBePlayed = owner == PlayerName.User,
    //     UntargetedCard = new UntargetedData()
    //   };
    // }
    //
    // static CardData OpponentCard(int id)
    // {
    //   return new CardData
    //   {
    //     CardId = CardId(id),
    //     Prefab = Prefab("Cards/FlameCard"),
    //     Owner = PlayerName.Enemy,
    //     IsRevealed = false,
    //     CanBePlayed = false
    //   };
    // }
    //
    // static Command MainButton(string text)
    // {
    //   return new Command
    //   {
    //     UpdateInterface = new UpdateInterfaceCommand
    //     {
    //       MainButtonEnabled = text != null,
    //       MainButtonText = text
    //     }
    //   };
    // }
    //
    // static Command CastSpell1(int c1, int c2, int damage, bool killsTarget = false,
    //   SkillAnimationNumber skill = SkillAnimationNumber.Skill1)
    // {
    //   return new Command
    //   {
    //     Attack = new AttackCommand
    //     {
    //       CreatureId = CreatureId(c1),
    //       TargetCreatureId = CreatureId(c2),
    //       Skill = skill,
    //       HitCount = 1,
    //       FireProjectile = new FireProjectileEffect
    //       {
    //         Prefab = Prefab("Projectiles/Projectile 2"),
    //         ApplyDamage = new ApplyDamageEffect
    //         {
    //           Damage = damage,
    //           KillsTarget = killsTarget
    //         },
    //       }
    //     }
    //   };
    // }
    //
    // static Command PlayCard(CardData cardData, RankValue rank, FileValue file, int delayMilliseconds = 2000)
    // {
    //   return new Command
    //   {
    //     PlayCard = new PlayCardCommand
    //     {
    //       Card = cardData,
    //       RevealDelayMilliseconds = delayMilliseconds,
    //       RankPosition = rank,
    //       FilePosition = file
    //     }
    //   };
    // }
    //
    // static Command CreateOrUpdate(CreatureData creatureData)
    // {
    //   return new Command
    //   {
    //     CreateOrUpdateCreature = new CreateOrUpdateCreatureCommand
    //     {
    //       Creature = creatureData
    //     }
    //   };
    // }
  }
}