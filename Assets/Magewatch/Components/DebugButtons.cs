// Copyright The Magewatch Project

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//    https://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.WellKnownTypes;
using Magewatch.Data;
using Magewatch.Services;
using UnityEngine;

// ReSharper disable UnusedMember.Global

namespace Magewatch.Components
{
  public sealed class DebugButtons : MonoBehaviour
  {
    [SerializeField] int _idCounter;

    void Start()
    {
      _idCounter = 100;
    }

    public void Slow()
    {
      Time.timeScale = 0.1f;
    }

    public void Engage()
    {
      RunCommands(new List<Command>
        {
          StockCreature(1),
          StockCreature(2)
        },
        new List<Command>
        {
          MeleeEngage(1, 2),
          MeleeEngage(2, 1)
        });
    }

    public void SetUp()
    {
      RunCommandList(
        StockCreature(1),
        StockCreature(2),
        StockCreature(3),
        StockCreature(4),
        StockCreature(5)
      );
    }

    public void Attack1()
    {
      RunCommands(new List<Command>
        {
          StockCreature(1),
          StockCreature(2)
        },
        new List<Command>
        {
          MeleeEngage(1, 2),
          MeleeEngage(2, 1)
        }, new List<Command>
        {
          Attack(1, 2, 10, Skill.Skill2),
          Attack(2, 1, 10, Skill.Skill3)
        }, new List<Command>
        {
          Attack(1, 2, 10, Skill.Skill2),
          Attack(2, 1, 10, Skill.Skill3)
        },
        new List<Command>
        {
          Attack(1, 2, 10, Skill.Skill2),
          Attack(2, 1, 10, Skill.Skill3)
        }, new List<Command>
        {
          Attack(1, 2, 10, Skill.Skill2),
          Attack(2, 1, 10, Skill.Skill3)
        });
    }

    public void Attack2()
    {
      var actions = new List<List<Command>>
      {
        new List<Command>
        {
          StockCreature(1),
          StockCreature(2)
        },
        new List<Command> {MeleeEngage(1, 2), MeleeEngage(2, 1)},
      };

      for (var i = 0; i < 9; ++i)
      {
        actions.Add(new List<Command>
        {
          i % 2 == 0 ? Attack(1, 2, 20, Skill.Skill2) : Attack(2, 1, 20, Skill.Skill3)
        });
      }

      RunCommands(actions.ToArray());
    }

    public void Attack3()
    {
      var commands = new List<List<Command>>
      {
        new List<Command>
        {
          StockCreature(1),
          StockCreature(2),
          StockCreature(3),
          StockCreature(4),
          StockCreature(5)
        },
        Cmd(Wait(500)),
        new List<Command>
        {
          MeleeEngage(1, 2),
          MeleeEngage(2, 1),
          MeleeEngage(3, 1),
          MeleeEngage(4, 5),
          MeleeEngage(5, 4)
        },
      };

      for (var i = 0; i < 4; ++i)
      {
        commands.Add(new List<Command>
        {
          Attack(1, 2, 20, Skill.Skill2),
          Attack(2, 1, 10, Skill.Skill3),
          Attack(3, 1, 10, Skill.Skill3),
          Attack(4, 5, 20, Skill.Skill2),
          Attack(5, 4, 20, Skill.Skill2)
        });
      }

      commands.Add(new List<Command>
      {
        Attack(1, 2, 20, Skill.Skill2, killsTarget: true),
        Attack(3, 1, 10, Skill.Skill3, killsTarget: true),
        Attack(4, 5, 20, Skill.Skill2, killsTarget: true),
      });

      commands.Add(new List<Command>
      {
        MeleeEngage(3, 4),
        MeleeEngage(4, 3)
      });

      commands.Add(new List<Command>
      {
        Attack(3, 4, 10, Skill.Skill3),
        Attack(4, 3, 20, Skill.Skill2)
      });

      commands.Add(new List<Command>
      {
        Attack(3, 4, 10, Skill.Skill3, killsTarget: true)
      });

      commands.Add(Cmd(CastAtOpponent(3)));

      commands.Add(Cmd(RemoveCreature(3)));

      commands.Add(Cmd(UpdatePlayer(PlayerName.User, 24, 25, 0, 0)));

      commands.Add(Cmd(Wait(1000)));

      commands.Add(new List<Command>
      {
        StockCreature(1),
        StockCreature(2),
        StockCreature(3),
        StockCreature(4),
        StockCreature(5)
      });

      RunCommands(commands.ToArray());
    }

    public void DrawCard()
    {
      RunCommand(new Command
      {
        DrawCard = new DrawCardCommand
        {
          Card = RageCard(_idCounter++)
        }
      });
    }

    public void DrawHands()
    {
      RunCommands(
        DrawCard(MageCard(50, PlayerName.User, new StringValue {Value = "Derek"})),
        DrawCard(OpponentCard(51)),
        DrawCard(BerserkerCard(52)),
        DrawCard(OpponentCard(53)),
        DrawCard(RageCard(54)),
        DrawCard(OpponentCard(55)),
        DrawCard(FlameScrollCard(56)),
        DrawCard(OpponentCard(57)),
        DrawCard(KnowledgeCard(58)),
        DrawCard(OpponentCard(59)),
        DrawCard(FlameScrollCard(60)),
        DrawCard(OpponentCard(61)),
        Cmd(MainButton("End Turn"))
      );
    }

    public void Cast()
    {
      RunCommands(
        Cmd(StockCreature(1)),
        Cmd(StockCreature(2)),
        Cmds(CastSpell1(2, 1, 50), MeleeEngage(1, 2)),
        Cmd(Wait(1000)),
        Cmd(Attack(1, 2, 20, Skill.Skill2)),
        Cmd(Attack(2, 1, 20, Skill.Skill3)),
        Cmd(RemoveCreature(1)),
        Cmd(RemoveCreature(2)));
    }

    public void AddMana()
    {
      RunCommand(new Command
      {
        UpdatePlayer = new UpdatePlayerCommand
        {
          Player = new PlayerData
          {
            PlayerName = PlayerName.User,
            CurrentLife = 25,
            MaximumLife = 25,
            CurrentMana = 3,
            MaximumMana = 5,
            CurrentInfluence = Flame(2),
            MaximumInfluence = Flame(4)
          }
        }
      });
    }

    public void OpponentTurn()
    {
      RunCommands(
        PlayCard(MageCard(51, PlayerName.Enemy), RankValue.Rank2, FileValue.File2),
        CreateOrUpdate(NewCreature("Mage", 51, PlayerName.Enemy, 6, -1)),
        PlayCard(BerserkerCard(53, PlayerName.Enemy), RankValue.Rank3, FileValue.File1),
        CreateOrUpdate(NewCreature("Berserker", 53, PlayerName.Enemy, 5, -2)),
        PlayCard(RageCard(55, PlayerName.Enemy), RankValue.Rank3, FileValue.File1),
        CreateOrUpdate(NewCreature("Berserker", 53, PlayerName.Enemy, 5, -2, hasAttachment: true)),
        PlayCard(FlameScrollCard(57, PlayerName.Enemy), RankValue.Unknown, FileValue.Unknown, 1000)
      );
    }

    static void RunCommand(Command command)
    {
      Root.Instance.CommandService.HandleCommands(new CommandList
      {
        Steps = new List<CommandStep>
        {
          new CommandStep
          {
            Commands = new List<Command>
            {
              command
            }
          }
        }
      });
    }

    static void RunCommandList(params Command[] commands)
    {
      Root.Instance.CommandService.HandleCommands(new CommandList
      {
        Steps = new List<CommandStep>
        {
          new CommandStep
          {
            Commands = commands.ToList()
          }
        }
      });
    }

    static void RunCommands(params List<Command>[] input)
    {
      Root.Instance.CommandService.HandleCommands(new CommandList
      {
        Steps = input.Select(step => new CommandStep {Commands = step}).ToList()
      });
    }

    static List<Command> Cmd(Command command) => new List<Command> {command};

    static List<Command> Cmds(params Command[] commands) => commands.ToList();

    static Command Wait(int milliseconds)
    {
      return new Command
      {
        Wait = new WaitCommand
        {
          WaitTimeMilliseconds = milliseconds
        }
      };
    }

    static Command MainButton(string text)
    {
      return new Command
      {
        UpdateInterface = new UpdateInterfaceCommand
        {
          MainButtonEnabled = text != null,
          MainButtonText = text
        }
      };
    }

    static List<Command> DrawCard(CardData cardData)
    {
      return new List<Command>
      {
        new Command
        {
          DrawCard = new DrawCardCommand
          {
            Card = cardData,
          }
        }
      };
    }

    static List<Command> PlayCard(CardData cardData, RankValue rank, FileValue file, int delayMilliseconds = 2000)
    {
      return new List<Command>
      {
        new Command
        {
          PlayCard = new PlayCardCommand
          {
            Card = cardData,
            RevealDelayMilliseconds = delayMilliseconds,
            RankPosition = rank,
            FilePosition = file
          }
        }
      };
    }


    static Command UpdatePlayer(PlayerName playerName, int currentLife, int maximumLife, int currentMana,
      int maximumMana)
    {
      return new Command
      {
        UpdatePlayer = new UpdatePlayerCommand
        {
          Player = new PlayerData
          {
            PlayerName = playerName,
            CurrentLife = currentLife,
            MaximumLife = maximumLife,
            CurrentMana = currentMana,
            MaximumMana = maximumMana,
            CurrentInfluence = new List<Influence>(),
            MaximumInfluence = new List<Influence>()
          }
        }
      };
    }

    static List<Command> CreateOrUpdate(CreatureData creatureData)
    {
      return new List<Command>
      {
        new Command
        {
          CreateOrUpdateCreature = new CreateOrUpdateCreatureCommand
          {
            Creature = creatureData
          }
        }
      };
    }

    static Command StockCreature(int id)
    {
      var create = new CreateOrUpdateCreatureCommand();
      switch (id)
      {
        case 1:
          create.Creature = NewCreature("Berserker", 1, PlayerName.User, -4, -3, hasAttachment: true);
          break;
        case 2:
          create.Creature = NewCreature("Mage", 2, PlayerName.Enemy, 4, -3);
          break;
        case 3:
          create.Creature = NewCreature("Mage", 3, PlayerName.Enemy, 5, -3);
          break;
        case 4:
          create.Creature = NewCreature("Berserker", 4, PlayerName.User, -3, 2);
          break;
        case 5:
          create.Creature = NewCreature("Berserker", 5, PlayerName.Enemy, 4, 2);
          break;
      }

      return new Command
      {
        CreateOrUpdateCreature = create
      };
    }

    static Command RemoveCreature(int id)
    {
      return new Command
      {
        RemoveCreature = new RemoveCreatureCommand
        {
          CreatureId = id
        }
      };
    }

    static CreatureData NewCreature(string name, int id, PlayerName owner, int? x = null, int? y = null,
      bool hasAttachment = false)
    {
      return new CreatureData
      {
        CreatureId = id,
        Prefab = Prefab($"Creatures/{name}"),
        Owner = owner,
        RankPosition = x.HasValue ? BoardPositions.ClosestRankForXPosition(x.Value, owner) : RankValue.Unknown,
        FilePosition = y.HasValue ? BoardPositions.ClosestFileForYPosition(y.Value) : FileValue.Unknown,
        Attachments = hasAttachment
          ? new List<AttachmentData>
          {
            new AttachmentData
            {
              Image = Sprite("Spells/SpellBook01_01")
            },
            new AttachmentData
            {
              Image = Sprite("Spells/SpellBook01_06")
            }
          }
          : null
      };
    }

    static Command MeleeEngage(int c1, int c2)
    {
      return new Command
      {
        MeleeEngage = new MeleeEngageCommand
        {
          CreatureId = c1,
          TargetCreatureId = c2
        }
      };
    }

    static Command Attack(int c1, int c2, int damage, Skill skill = Skill.Skill1, bool killsTarget = false,
      int hitCount = 1)
    {
      return new Command
      {
        Attack = new AttackCommand
        {
          CreatureId = c1,
          TargetCreatureId = c2,
          SkillNumber = skill,
          HitCount = hitCount,
          AttackEffect = new AttackEffect
          {
            ApplyDamage = new ApplyDamageEffect
            {
              Damage = damage,
              KillsTarget = killsTarget
            }
          }
        }
      };
    }

    static CardData MageCard(int id, PlayerName owner = PlayerName.User, StringValue name = null)
    {
      return new CardData
      {
        CardId = id,
        Prefab = Prefab("Cards/FireCard"),
        Name = name == null ? "Mage" : name.ToString(),
        ManaCost = 2,
        InfluenceCost = Flame(1),
        Owner = owner,
        Image = Sprite("CreatureImages/Mage"),
        Text = "Whiz! Zoom!",
        IsRevealed = true,
        CanBePlayed = owner == PlayerName.User,
        CreatureData = NewCreature("Mage", id, PlayerName.User)
      };
    }

    static CardData BerserkerCard(int id, PlayerName owner = PlayerName.User)
    {
      return new CardData
      {
        CardId = id,
        Prefab = Prefab("Cards/FireCard"),
        Name = "Berserker",
        ManaCost = 4,
        InfluenceCost = Flame(2),
        Owner = owner,
        Image = Sprite("CreatureImages/Berserker"),
        Text = "Anger & Axes",
        IsRevealed = true,
        CanBePlayed = owner == PlayerName.User,
        CreatureData = NewCreature("Berserker", id, PlayerName.User)
      };
    }

    static CardData RageCard(int id, PlayerName owner = PlayerName.User)
    {
      return new CardData
      {
        CardId = id,
        Prefab = Prefab("Cards/FireCard"),
        Name = "Rage",
        ManaCost = 3,
        InfluenceCost = LightAndFlame(1, 1),
        Owner = owner,
        Image = Sprite("Spells/SpellBook01_01"),
        Text = "Adds Bonus Damage on Hits",
        IsRevealed = true,
        CanBePlayed = owner == PlayerName.User,
        AttachmentData = new AttachmentData
        {
          Image = Sprite("Spells/SpellBook01_01")
        }
      };
    }

    static CardData KnowledgeCard(int id, PlayerName owner = PlayerName.User)
    {
      return new CardData
      {
        CardId = id,
        Prefab = Prefab("Cards/FireCard"),
        Name = "Knowledge",
        ManaCost = 3,
        InfluenceCost = Flame(3),
        Owner = owner,
        Image = Sprite("Spells/SpellBook01_06"),
        Text = "Extra Mana",
        IsRevealed = true,
        CanBePlayed = owner == PlayerName.User,
        AttachmentData = new AttachmentData
        {
          Image = Sprite("Spells/SpellBook01_06")
        }
      };
    }

    static CardData FlameScrollCard(int id, PlayerName owner = PlayerName.User)
    {
      return new CardData
      {
        CardId = id,
        Prefab = Prefab("Cards/FireCard"),
        Name = "Flame Scroll",
        NoCost = true,
        Owner = owner,
        Image = Sprite("Scrolls/ScrollsAndBooks_21_t"),
        Text = "Adds 1 mana and 1 flame influence",
        Untargeted = true,
        IsRevealed = true,
        CanBePlayed = owner == PlayerName.User
      };
    }

    static CardData OpponentCard(int id)
    {
      return new CardData
      {
        CardId = id,
        Prefab = Prefab("Cards/FireCard"),
        Owner = PlayerName.Enemy,
        IsRevealed = false,
        CanBePlayed = false
      };
    }

    static List<Influence> LightAndFlame(int light, int flame)
    {
      return new List<Influence>
      {
        new Influence
        {
          Type = InfluenceType.Light,
          Value = light
        },
        new Influence
        {
          Type = InfluenceType.Flame,
          Value = flame
        }
      };
    }

    static List<Influence> Flame(int value)
    {
      return new List<Influence>
      {
        new Influence
        {
          Type = InfluenceType.Flame,
          Value = value
        }
      };
    }

    static Command CastSpell1(int c1, int c2, int damage, bool killsTarget = false, Skill skill = Skill.Skill1)
    {
      return new Command
      {
        Attack = new AttackCommand
        {
          CreatureId = c1,
          TargetCreatureId = c2,
          SkillNumber = skill,
          HitCount = 1,
          AttackEffect = new AttackEffect
          {
            FireProjectile = new FireProjectileEffect
            {
              Prefab = Prefab("Projectiles/Projectile 2"),
              ApplyDamage = new ApplyDamageEffect
              {
                Damage = damage,
                KillsTarget = killsTarget
              },
            }
          }
        }
      };
    }

    static Command CastAtOpponent(int c1, Skill skill = Skill.Skill1)
    {
      return new Command
      {
        Attack = new AttackCommand
        {
          CreatureId = c1,
          TargetCreatureId = c1,
          SkillNumber = skill,
          HitCount = 1,
          AttackEffect = new AttackEffect
          {
            FireProjectile = new FireProjectileEffect
            {
              Prefab = Prefab("Projectiles/Projectile 6"),
              ApplyDamage = new ApplyDamageEffect
              {
                Damage = 0,
                KillsTarget = false
              },
              AtOpponent = true
            }
          }
        }
      };
    }

    static Asset Prefab(string address) =>
      new Asset
      {
        Address = address,
        AssetType = AssetType.Prefab
      };

    static Asset Sprite(string address) =>
      new Asset
      {
        Address = address,
        AssetType = AssetType.Sprite
      };
  }
}