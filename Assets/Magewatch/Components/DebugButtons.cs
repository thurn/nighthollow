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

namespace Magewatch.Components
{
  public sealed class DebugButtons : MonoBehaviour
  {
    public void Slow()
    {
      Time.timeScale = 0.1f;
    }

    public void Engage()
    {
      RunCombat(new List<CombatAction>
      {
        MeleeEngage(1, 2),
        MeleeEngage(2, 1)
      });
    }

    public void Attack1()
    {
      RunCombat(new List<CombatAction>
      {
        MeleeEngage(1, 2),
        MeleeEngage(2, 1)
      }, new List<CombatAction>
      {
        Attack(1, 2, 10, Skill.Skill2),
      }, new List<CombatAction>
      {
        Attack(2, 1, 10, Skill.Skill3)
      });
    }

    public void Attack2()
    {
      var actions = new List<List<CombatAction>>
      {
        new List<CombatAction> {MeleeEngage(1, 2), MeleeEngage(2, 1)},
      };

      for (var i = 0; i < 9; ++i)
      {
        actions.Add(new List<CombatAction>
        {
          i % 2 == 0 ? Attack(1, 2, 20, Skill.Skill2) : Attack(2, 1, 20, Skill.Skill3)
        });
      }

      RunCombat(actions.ToArray());
    }

    public void Attack3()
    {
      var actions = new List<List<CombatAction>>
      {
        new List<CombatAction>
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
           actions.Add(new List<CombatAction>
           {
             Attack(1, 2, 20, Skill.Skill2),
             Attack(4, 5, 20, Skill.Skill2)
           });
        }
        else
        {
          actions.Add(new List<CombatAction>
          {
            Attack(2, 1, 10, Skill.Skill3),
            Attack(3, 1, 10, Skill.Skill3),
            Attack(5, 4, 20, Skill.Skill2)
          });
        }
      }

      actions.Add(new List<CombatAction>
      {
        MeleeEngage(1, 3),
        Attack(3, 1, 10, Skill.Skill3)
      });

      actions.Add(new List<CombatAction>
      {
        Attack(1, 3, 20, Skill.Skill2),
        Attack(3, 1, 10, Skill.Skill3)
      });

      actions.Add(new List<CombatAction>
      {
        MeleeEngage(3, 4),
        MeleeEngage(4, 3)
      });

      RunCombat(actions.ToArray());
    }

    static void RunCombat(params List<CombatAction>[] input)
    {
      Root.Instance.CommandService.HandleCommand(new Command
      {
        RunCombatCommand = new RunCombatCommand
        {
          Steps = input.Select(step => new CombatStep {Actions = step}).ToList()
        }
      });
    }

    static CombatAction MeleeEngage(int c1, int c2)
    {
      return new CombatAction
      {
        MeleeEngage = new MeleeEngage
        {
          CreatureId = c1,
          TargetCreatureId = c2
        }
      };
    }

    static CombatAction Attack(int c1, int c2, int damage, Skill skill = Skill.Skill1, int hitCount = 1)
    {
      return new CombatAction
      {
        Attack = new Attack
        {
          CreatureId = c1,
          TargetCreatureId = c2,
          SkillNumber = skill,
          HitCount = hitCount,
          DamagePercent = damage
        }
      };
    }
  }
}