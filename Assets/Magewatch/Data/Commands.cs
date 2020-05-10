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

namespace Magewatch.Data
{
  public enum Skill
  {
    Skill1 = 1,
    Skill2 = 2,
    Skill3 = 3,
    Skill4 = 4,
    Skill5 = 5
  }

  public sealed class CommandList
  {
    public List<CommandStep> Steps;
  }

  /// <summary>
  /// Combat takes places over a series of finite rounds called "combat steps". All actions in an
  /// earlier combat step are completed before the next combat step.
  /// </summary>
  public sealed class CommandStep
  {
    public List<Command> Commands;
  }

  public sealed class Command
  {
    public WaitCommand Wait;

    public UpdateInterfaceCommand UpdateInterface;

    public DrawCardCommand DrawCard;

    public PlayCardCommand PlayCard;

    public UpdatePlayerCommand UpdatePlayer;

    public CreateOrUpdateCreatureCommand CreateOrUpdateCreature;

    public RemoveCreatureCommand RemoveCreature;

    public MeleeEngageCommand MeleeEngage;

    public AttackCommand Attack;
  }

  public sealed class WaitCommand
  {
    public int WaitTimeMilliseconds;
  }

  public sealed class UpdateInterfaceCommand
  {
    public bool MainButtonEnabled;

    public string MainButtonText;
  }

  public sealed class DrawCardCommand
  {
    public CardData Card;
  }

  /// <summary>Plays a card which already exists in the scene. The card is revealed and then animated to a display
  /// position, and then animated away, optionally to the provided rank/file position.</summary>
  public sealed class PlayCardCommand
  {
    public CardData Card;

    /// <summary>How long to display the card to the user for before playing it. Defaults to 2000ms.</summary>
    public float RevealDelayMilliseconds;

    public RankValue RankPosition;

    public FileValue FilePosition;
  }

  public sealed class UpdatePlayerCommand
  {
    public PlayerData Player;
  }

  public sealed class CreateOrUpdateCreatureCommand
  {
    public CreatureData Creature;
  }

  public sealed class RemoveCreatureCommand
  {
    public int CreatureId;
  }

  public sealed class MeleeEngageCommand
  {
    /// <summary>The creature which is moving to attack</summary>
    public int CreatureId;

    /// <summary>The creature should move to within melee range of this enemy creature</summary>
    public int TargetCreatureId;
  }

  public sealed class AttackCommand
  {
    /// <summary>The creature performing the attack</summary>
    public int CreatureId;

    /// <summary>The creature being attacked</summary>
    public int TargetCreatureId;

    /// <summary>Which skill animation should be used</summary>
    public Skill SkillNumber;

    /// <summary>How many times this skill is expected to raise the 'AttackStart' event</summary>
    public int HitCount;

    /// <summary>What effect should be applied to the target for *each* 'AttackStart'</summary>
    public AttackEffect AttackEffect;
  }

  public sealed class AttackEffect
  {
    public ApplyDamageEffect ApplyDamage;

    public FireProjectileEffect FireProjectile;
  }

  public sealed class ApplyDamageEffect
  {
    public int Damage;

    public bool KillsTarget;
  }

  public sealed class FireProjectileEffect
  {
    public Asset Prefab;

    public ApplyDamageEffect ApplyDamage;

    public bool AtOpponent;
  }
}