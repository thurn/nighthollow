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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DuloGames.UI;
using Magewatch.Data;
using Magewatch.Services;
using UnityEngine;
using UnityEngine.Rendering;

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
        MeleeEngage(1, 2),
        MeleeEngage(2, 1)
      });
    }

    public void SetUp()
    {
      RunCommandList(
        CreateCreature(1),
        CreateCreature(2),
        CreateCreature(3),
        CreateCreature(4),
        CreateCreature(5)
      );
    }

    public void Attack1()
    {
      RunCommands(new List<Command>
        {
          CreateCreature(1),
          CreateCreature(2)
        },
        new List<Command>
        {
          MeleeEngage(1, 2),
          MeleeEngage(2, 1)
        }, new List<Command>
        {
          Attack(1, 2, 10, Skill.Skill2),
        }, new List<Command>
        {
          Attack(2, 1, 10, Skill.Skill3)
        });
    }

    public void Attack2()
    {
      var actions = new List<List<Command>>
      {
        new List<Command>
        {
          CreateCreature(1),
          CreateCreature(2)
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
          CreateCreature(1),
          CreateCreature(2),
          CreateCreature(3),
          CreateCreature(4),
          CreateCreature(5)
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

      for (var i = 0; i < 9; ++i)
      {
        if (i % 2 == 0)
        {
          commands.Add(new List<Command>
          {
            Attack(1, 2, 20, Skill.Skill2),
            Attack(4, 5, 20, Skill.Skill2)
          });
        }
        else
        {
          commands.Add(new List<Command>
          {
            Attack(2, 1, 10, Skill.Skill3),
            Attack(3, 1, 10, Skill.Skill3),
            Attack(5, 4, 20, Skill.Skill2)
          });
        }
      }

      commands.Add(new List<Command>
      {
        MeleeEngage(1, 3),
        Attack(3, 1, 10, Skill.Skill3)
      });

      commands.Add(new List<Command>
      {
        Attack(1, 3, 20, Skill.Skill2),
      });

      commands.Add(new List<Command>
      {
        Attack(3, 1, 10, Skill.Skill3)
      });

      commands.Add(new List<Command>
      {
        MeleeEngage(3, 4),
        MeleeEngage(4, 3)
      });

      commands.Add(new List<Command>
      {
        Attack(3, 4, 10, Skill.Skill3),
      });

      commands.Add(new List<Command>
      {
        Attack(4, 3, 20, Skill.Skill2)
      });

      commands.Add(new List<Command>
      {
        Attack(3, 4, 10, Skill.Skill3),
      });

      commands.Add(Cmd(CastAtOpponent(3)));

      commands.Add(Cmd(RemoveCreature(3)));

      commands.Add(Cmd(UpdatePlayer(PlayerName.User, 24, 25, 1, 1)));

      commands.Add(Cmd(Wait(1000)));

      commands.Add(new List<Command>
      {
        CreateCreature(1),
        CreateCreature(2),
        CreateCreature(3),
        CreateCreature(4),
        CreateCreature(5)
      });

      RunCommands(commands.ToArray());
    }

    public void DrawCard()
    {
      RunCommand(new Command
      {
        DrawCard = new DrawCardCommand
        {
          Card = MageCard(_idCounter++)
        }
      });
    }

    public void DrawHand()
    {
      RunCommands(
        DrawCard(MageCard(_idCounter++)),
        DrawCard(BerserkerCard(_idCounter++)),
        DrawCard(RageCard(_idCounter++)),
        DrawCard(BerserkerCard(_idCounter++)),
        DrawCard(RageCard(_idCounter++)),
        DrawCard(BerserkerCard(_idCounter++))
      );
    }

    public void Cast()
    {
      RunCommands(
        Cmd(CreateCreature(1)),
        Cmd(CreateCreature(2)),
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

    static void RunSequentially(params Command[] commands)
    {
      Root.Instance.CommandService.HandleCommands(new CommandList
      {
        Steps = commands.Select(c => new CommandStep
        {
          Commands = new List<Command> {c}
        }).ToList()
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

    static List<Command> DrawCard(CardData cardData)
    {
      return new List<Command>
      {
        new Command
        {
          DrawCard = new DrawCardCommand
          {
            Card = cardData
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

    static Command CreateCreature(int id)
    {
      var create = new CreateCreatureCommand();
      switch (id)
      {
        case 1:
          create.Creature = NewCreature("Berserker", 1, PlayerName.User, -4, -3);
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
        CreateCreature = create
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

    static CreatureData NewCreature(string name, int id, PlayerName owner, int x, int y)
    {
      return new CreatureData
      {
        CreatureId = id,
        Prefab = new Asset<GameObject>($"Creatures/{name}"),
        Owner = owner,
        RankPosition = BoardPositions.ClosestRankForXPosition(x, owner),
        FilePosition = BoardPositions.ClosestFileForYPosition(y)
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

    static Command Attack(int c1, int c2, int damage, Skill skill = Skill.Skill1, int hitCount = 1)
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
              Damage = damage
            }
          }
        }
      };
    }

    static CardData MageCard(int id)
    {
      return new CardData
      {
        CardId = id,
        Prefab = new Asset<GameObject>("Cards/FireCard"),
        Name = "Mage",
        ManaCost = 2,
        InfluenceCost = Flame(1),
        Owner = PlayerName.User,
        Image = new Asset<Sprite>("CreatureImages/Mage"),
        Text = "Whiz! Zoom!",
        IsRevealed = true,
        CanBePlayed = true,
        CreatureData = NewCreature("Mage", id, PlayerName.User, 0, 0)
      };
    }

    static CardData BerserkerCard(int id)
    {
      return new CardData
      {
        CardId = id,
        Prefab = new Asset<GameObject>("Cards/FireCard"),
        Name = "Berserker",
        ManaCost = 4,
        InfluenceCost = Flame(2),
        Owner = PlayerName.User,
        Image = new Asset<Sprite>("CreatureImages/Berserker"),
        Text = "Anger & Axes",
        IsRevealed = true,
        CanBePlayed = true,
        CreatureData = NewCreature("Berserker", id, PlayerName.User, 0, 0)
      };
    }

    static CardData RageCard(int id)
    {
      return new CardData
      {
        CardId = id,
        Prefab = new Asset<GameObject>("Cards/FireCard"),
        Name = "Rage",
        ManaCost = 3,
        InfluenceCost = LightAndFlame(1, 1),
        Owner = PlayerName.User,
        Image = new Asset<Sprite>("Spells/SpellBook01_01"),
        Text = "Adds Bonus Damage on Hits",
        IsRevealed = true,
        CanBePlayed = true
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

    static Command CastSpell1(int c1, int c2, int damage, Skill skill = Skill.Skill1)
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
              Prefab = new Asset<GameObject>("Projectiles/Projectile 3"),
              Damage = damage,
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
              Prefab = new Asset<GameObject>("Projectiles/Projectile 6"),
              Damage = 0,
              AtOpponent = true
            }
          }
        }
      };
    }
  }
}