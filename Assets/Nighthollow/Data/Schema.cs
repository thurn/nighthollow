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
using Nighthollow.Generated;
using Nighthollow.Stats;

#nullable enable

namespace Nighthollow.Data
{
  public interface IFieldType
  {
    string Print();
  }

  public sealed class FieldType<T> : IFieldType
  {
    public string Print() => typeof(T).Name;
  }

  public sealed class DictionaryFieldType : IFieldType
  {
    public DictionaryFieldType(IFieldType keyType, IFieldType valueType)
    {
      KeyType = keyType;
      ValueType = valueType;
    }

    public IFieldType KeyType { get; }
    public IFieldType ValueType { get; }

    public string Print() => $"ImmutableDictionary<{KeyType.Print()}, {ValueType.Print()}>";
  }

  public sealed class ListFieldType : IFieldType
  {
    public ListFieldType(IFieldType elementType)
    {
      ElementType = elementType;
    }

    public IFieldType ElementType { get; }

    public string Print() => $"ImmutableList<{ElementType.Print()}>";
  }

  public sealed class UnionFieldType : IFieldType
  {
    public UnionFieldType(string name)
    {
      Name = name;
    }

    public string Name { get; }

    public string Print() => Name;
  }

  public sealed class DatatypeFieldType : IFieldType
  {
    public DatatypeFieldType(string name)
    {
      Name = name;
    }

    public string Name { get; }

    public string Print() => Name;
  }

  public sealed class Field
  {
    public Field(string name, IFieldType fieldType)
    {
      Name = name;
      FieldType = fieldType;
    }

    public string Name { get; }
    public IFieldType FieldType { get; }
  }

  public interface ISchemaType
  {
  }

  public sealed class UnionType : ISchemaType
  {
    public UnionType(string name, List<Field> unionTypes)
    {
      Name = name;
      UnionTypes = unionTypes;
    }

    public string Name { get; }
    public List<Field> UnionTypes { get; }
  }

  public sealed class DataType : ISchemaType
  {
    public DataType(string name, List<Field> fields)
    {
      Name = name;
      Fields = fields;
    }

    public string Name { get; }
    public List<Field> Fields { get; }
  }

  public static class Schema
  {
    public static readonly List<ISchemaType> AllTypes = new List<ISchemaType>
    {
      new UnionType("ValueData", new List<Field>
      {
        new Field("Int", new FieldType<int>()),
        new Field("Bool", new FieldType<bool>()),
        new Field("Duration", new FieldType<DurationValue>()),
        new Field("Percentage", new FieldType<PercentageValue>()),
        new Field("IntRange", new FieldType<IntRangeValue>())
      }),

      new DataType("ModifierTypeData", new List<Field>
      {
        new Field("StatId", new FieldType<StatId>()),
        new Field("Operator", new FieldType<Operator>()),
        new Field("DelegateId", new FieldType<DelegateId>()),
        new Field("ValueLow", new UnionFieldType("ValueData")),
        new Field("ValueHigh", new UnionFieldType("ValueData")),
        new Field("IsTargeted", new FieldType<bool>())
      }),

      new DataType("AffixTypeData", new List<Field>
      {
        new Field("MinLevel", new FieldType<int>()),
        new Field("Weight", new FieldType<int>()),
        new Field("ManaCost", new FieldType<IntRangeValue>()),
        new Field("Modifiers", new ListFieldType(new DatatypeFieldType("ModifierTypeData"))),
        new Field("InfluenceType", new FieldType<School>())
      }),

      new DataType("SkillTypeData", new List<Field>
      {
        new Field("Name", new FieldType<string>()),
        new Field("SkillAnimationType", new FieldType<SkillAnimationType>()),
        new Field("SkillType", new FieldType<SkillType>()),
        new Field("ImplicitAffix", new DatatypeFieldType("AffixTypeData")),
        new Field("Address", new FieldType<string>()),
        new Field("ProjectileSpeed", new FieldType<int>()),
        new Field("UsesAccuracy", new FieldType<bool>()),
        new Field("CanCrit", new FieldType<bool>()),
        new Field("CanStun", new FieldType<bool>())
      }),

      new DataType("CreatureTypeData", new List<Field>
      {
        new Field("Name", new FieldType<string>()),
        new Field("PrefabAddress", new FieldType<string>()),
        new Field("Owner", new FieldType<PlayerName>()),
        new Field("Health", new FieldType<IntRangeValue>()),
        new Field("SkillAnimations", new DictionaryFieldType(
          new FieldType<SkillAnimationNumber>(),
          new FieldType<SkillAnimationType>())),
        new Field("ImageAddress", new FieldType<string>()),
        new Field("BaseManaCost", new FieldType<int>()),
        new Field("Speed", new FieldType<int>()),
        new Field("ImplicitAffix", new DatatypeFieldType("AffixTypeData")),
        new Field("ImplicitSkill", new DatatypeFieldType("SkillTypeData")),
        new Field("IsManaCreature", new FieldType<bool>())
      })
    };
  }
}
