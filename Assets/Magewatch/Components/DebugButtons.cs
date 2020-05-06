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

      commands.Add(new List<Command>
      {
        Attack(3, 4, 10, Skill.Skill3),
      });

      commands.Add(new List<Command>
      {
        UpdatePlayer(PlayerName.User, 24, 25, 1, 1)
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
        DrawCard(MageCard(_idCounter++)),
        DrawCard(MageCard(_idCounter++)),
        DrawCard(MageCard(_idCounter++)),
        DrawCard(MageCard(_idCounter++)),
        DrawCard(MageCard(_idCounter++))
      );
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
            MaximumMana = maximumMana
          }
        }
      };
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
          DamagePercent = damage
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
        Influence = new Influence
        {
          Flame = 1
        },
        Owner = PlayerName.User,
        Image = new Asset<Sprite>("CreatureImages/Mage"),
        Text = "Whiz! Zoom!",
        IsRevealed = true,
        CanBePlayed = true,
        CreatureData = NewCreature("Mage", id, PlayerName.User, 0, 0)
      };
    }
  }
}