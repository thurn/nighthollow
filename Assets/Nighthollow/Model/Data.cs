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

using System.Collections.Generic;

namespace Nighthollow.Model
{
  public struct CreatureId
  {
    public int Value;
  }

  public struct CardId
  {
    public int Value;
  }

  public enum PlayerName
  {
    User,
    Enemy
  }

  public enum RankValue
  {
    Rank1,
    Rank2,
    Rank3,
    Rank4,
    Rank5
  }

  public enum FileValue
  {
    File1,
    File2,
    File3,
    File4,
    File5
  }

  public enum School
  {
    Light,
    Sky,
    Flame,
    Ice,
    Earth,
    Shadow
  }

  public sealed class Influence
  {
    public School School;

    public int Value;
  }

  public enum AssetType
  {
    Prefab,
    Sprite
  }

  public sealed class Asset
  {
    public string Address;

    public AssetType AssetType;
  }

  public sealed class PlayerData
  {
    public PlayerName PlayerName;
    public int CurrentLife;
    public int MaximumLife;
    public int CurrentMana;
    public int MaximumMana;
    public List<Influence> CurrentInfluence;
    public List<Influence> MaximumInfluence;
  }

  public sealed class StandardCost
  {
    public int ManaCost;
    public List<Influence> InfluenceCost;
  }

  public sealed class AttachmentData
  {
    public Asset Image;
  }

  public sealed class CardData
  {
    public CardId CardId;

    public Asset Prefab;

    public StandardCost StandardCost;

    public PlayerName Owner;

    public Asset Image;

    public bool CanBePlayed;

    public CreatureData CreatureData;
  }

  public sealed class CreatureData
  {
    public CreatureId CreatureId;

    public Asset Prefab;

    public PlayerName Owner;

    public RankValue RankPosition;

    public FileValue FilePosition;

    public List<AttachmentData> Attachments;
  }

  public enum SkillAnimationNumber
  {
    Skill1,
    Skill2,
    Skill3,
    Skill4,
    Skill5
  }
}