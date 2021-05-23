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
using System.Collections.Immutable;
using System.Linq;
using MessagePack;
using MessagePack.Formatters;

#nullable enable

namespace Nighthollow.Triggers
{
  public sealed class TriggerDataFormatter<TEvent> : IMessagePackFormatter<TriggerData<TEvent>>
    where TEvent : IEvent
  {
    public void Serialize(
      ref MessagePackWriter writer,
      TriggerData<TEvent>? value,
      MessagePackSerializerOptions options)
    {
      if (value == null)
      {
        throw new InvalidOperationException($"Got null while serializing for type {typeof(TEvent)}");
      }

      IFormatterResolver formatterResolver = options.Resolver;
      writer.WriteArrayHeader(6);
      formatterResolver.GetFormatterWithVerify<string>().Serialize(ref writer, value.Name!, options);
      writer.WriteUInt32((uint) value.Category);
      formatterResolver.GetFormatterWithVerify<ImmutableList<ICondition>>()
        .Serialize(ref writer, value.Conditions.Cast<ICondition>().ToImmutableList(), options);
      formatterResolver.GetFormatterWithVerify<ImmutableList<IEffect>>()
        .Serialize(ref writer, value.Effects.Cast<IEffect>().ToImmutableList(), options);
      writer.Write(value.Looping);
      writer.Write(value.Disabled);
    }

    public TriggerData<TEvent> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
      if (reader.TryReadNil())
      {
        throw new InvalidOperationException($"Got null while deserializing for type {typeof(TEvent)}");
      }

      options.Security.DepthStep(ref reader);
      IFormatterResolver formatterResolver = options.Resolver;
      var length = reader.ReadArrayHeader();
      string? name = null;
      var category = TriggerCategory.NoCategory;
      ImmutableList<ICondition>? conditions = null;
      ImmutableList<IEffect>? effects = null;
      var looping = false;
      var disabled = false;

      for (int i = 0; i < length; i++)
      {
        switch (i)
        {
          case 0:
            name = formatterResolver.GetFormatterWithVerify<string>().Deserialize(ref reader, options);
            break;
          case 1:
            category = (TriggerCategory) reader.ReadUInt32();
            break;
          case 2:
            conditions = formatterResolver.GetFormatterWithVerify<ImmutableList<ICondition>>()
              .Deserialize(ref reader, options);
            break;
          case 3:
            effects = formatterResolver.GetFormatterWithVerify<ImmutableList<IEffect>>()
              .Deserialize(ref reader, options);
            break;
          case 4:
            looping = reader.ReadBoolean();
            break;
          case 5:
            disabled = reader.ReadBoolean();
            break;
          default:
            reader.Skip();
            break;
        }
      }

      var result = new TriggerData<TEvent>(
        name,
        category,
        conditions?.Cast<ICondition<TEvent>>().ToImmutableList(),
        effects?.Cast<IEffect<TEvent>>().ToImmutableList(),
        looping,
        disabled);
      reader.Depth--;
      return result;
    }
  }
}