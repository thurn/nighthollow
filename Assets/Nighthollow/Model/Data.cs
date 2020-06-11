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

using System;
using System.Collections.Generic;

namespace Nighthollow.Model
{
  public readonly struct CreatureId : IEquatable<CreatureId>
  {
    public readonly int Value;

    public CreatureId(int value)
    {
      Value = value;
    }

    public bool Equals(CreatureId other)
    {
      return Value == other.Value;
    }

    public override bool Equals(object obj)
    {
      return obj is CreatureId other && Equals(other);
    }

    public override int GetHashCode()
    {
      return Value;
    }

    public static bool operator ==(CreatureId left, CreatureId right)
    {
      return left.Equals(right);
    }

    public static bool operator !=(CreatureId left, CreatureId right)
    {
      return !left.Equals(right);
    }
  }

  public readonly struct CardId : IEquatable<CardId>
  {
    public readonly int Value;

    public CardId(int value)
    {
      Value = value;
    }

    public bool Equals(CardId other)
    {
      return Value == other.Value;
    }

    public override bool Equals(object obj)
    {
      return obj is CardId other && Equals(other);
    }

    public override int GetHashCode()
    {
      return Value;
    }

    public static bool operator ==(CardId left, CardId right)
    {
      return left.Equals(right);
    }

    public static bool operator !=(CardId left, CardId right)
    {
      return !left.Equals(right);
    }
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
    Rank5,
    Rank6,
    Rank7,
    Rank8
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
    public readonly School School;

    public readonly int Value;

    public Influence(School school, int value)
    {
      School = school;
      Value = value;
    }

    public override string ToString()
    {
      return $"[Influence {nameof(School)}: {School}, {nameof(Value)}: {Value}]";
    }
  }

  public enum AssetType
  {
    Prefab,
    Sprite
  }

  public sealed class Asset
  {
    public readonly string Address;

    public readonly AssetType AssetType;

    public Asset(string address, AssetType assetType)
    {
      Address = address;
      AssetType = assetType;
    }

    public override string ToString()
    {
      return $"[Asset {nameof(Address)}: {Address}, {nameof(AssetType)}: {AssetType}]";
    }
  }

  public sealed class UserData
  {
    public readonly int CurrentLife;
    public readonly int MaximumLife;
    public readonly int CurrentMana;
    public readonly int MaximumMana;
    public readonly IReadOnlyList<Influence> CurrentInfluence;
    public readonly IReadOnlyList<Influence> MaximumInfluence;

    public UserData(int currentLife, int maximumLife, int currentMana, int maximumMana,
      IReadOnlyList<Influence> currentInfluence, IReadOnlyList<Influence> maximumInfluence)
    {
      CurrentLife = currentLife;
      MaximumLife = maximumLife;
      CurrentMana = currentMana;
      MaximumMana = maximumMana;
      CurrentInfluence = currentInfluence;
      MaximumInfluence = maximumInfluence;
    }

    public override string ToString()
    {
      return
        $"[UserData {nameof(CurrentLife)}: {CurrentLife}, " +
        $"{nameof(MaximumLife)}: {MaximumLife}, " +
        $"{nameof(CurrentMana)}: {CurrentMana}, " +
        $"{nameof(MaximumMana)}: {MaximumMana}, " +
        $"{nameof(CurrentInfluence)}: {CurrentInfluence}, " +
        $"{nameof(MaximumInfluence)}: {MaximumInfluence}]";
    }
  }

  public sealed class Cost
  {
    public readonly int ManaCost;
    public readonly IReadOnlyList<Influence> InfluenceCost;

    public Cost(int manaCost, IReadOnlyList<Influence> influenceCost)
    {
      ManaCost = manaCost;
      InfluenceCost = influenceCost;
    }

    public override string ToString()
    {
      return $"[Cost {nameof(ManaCost)}: {ManaCost}, {nameof(InfluenceCost)}: {InfluenceCost}]";
    }
  }

  public sealed class AttachmentData
  {
    public readonly Asset Image;

    public AttachmentData(Asset image)
    {
      Image = image;
    }

    public override string ToString()
    {
      return $"[Attachment {nameof(Image)}: {Image}]";
    }
  }

  public sealed class CardData
  {
    public readonly CardId CardId;

    public readonly Asset Prefab;

    public readonly Cost Cost;

    public readonly Asset Image;

    public readonly CreatureData CreatureData;

    public CardData(CardId cardId, Asset prefab, Cost cost, Asset image, CreatureData creatureData)
    {
      CardId = cardId;
      Prefab = prefab;
      Cost = cost;
      Image = image;
      CreatureData = creatureData;
    }

    public override string ToString()
    {
      return $"[CardData {nameof(CardId)}: {CardId}, " +
             $"{nameof(Prefab)}: {Prefab}, " +
             $"{nameof(Cost)}: {Cost}, " +
             $"{nameof(Image)}: {Image}, " +
             $"{nameof(CreatureData)}: {CreatureData}]";
    }
  }

  public sealed class CreatureData
  {
    public readonly CreatureId CreatureId;

    public readonly Asset Prefab;

    public readonly PlayerName Owner;

    public readonly int Speed;

    public readonly IReadOnlyList<AttachmentData> Attachments;

    public CreatureData(CreatureId creatureId, Asset prefab, PlayerName owner,
      int speed, IReadOnlyList<AttachmentData> attachments)
    {
      CreatureId = creatureId;
      Prefab = prefab;
      Owner = owner;
      Speed = speed;
      Attachments = attachments;
    }

    public override string ToString()
    {
      return $"[CreatureData {nameof(CreatureId)}: {CreatureId}, " +
             $"{nameof(Prefab)}: {Prefab}, " +
             $"{nameof(Owner)}: {Owner}, " +
             $"{nameof(Attachments)}: {Attachments}]";
    }
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