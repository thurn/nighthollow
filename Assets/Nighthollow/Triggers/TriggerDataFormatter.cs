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

using System.Collections.Immutable;
using System.Linq;
using MessagePack;
using MessagePack.Formatters;

#nullable enable

namespace Nighthollow.Triggers
{
  public sealed class TriggerDataFormatter<TEvent> : IMessagePackFormatter<TriggerData<TEvent>>
    where TEvent : TriggerEvent
  {
    public void Serialize(
      ref MessagePackWriter writer,
      TriggerData<TEvent>? value,
      MessagePackSerializerOptions options)
    {
      if (value == null)
      {
        writer.WriteNil();
        return;
      }

      IFormatterResolver formatterResolver = options.Resolver;
      writer.WriteArrayHeader(4);
      formatterResolver.GetFormatterWithVerify<string>().Serialize(ref writer, value.Name!, options);
      formatterResolver.GetFormatterWithVerify<ImmutableList<ICondition>>()
        .Serialize(ref writer, value.Conditions.Cast<ICondition>().ToImmutableList(), options);
      formatterResolver.GetFormatterWithVerify<ImmutableList<IEffect>>()
        .Serialize(ref writer, value.Effects.Cast<IEffect>().ToImmutableList(), options);
      writer.Write(value.Disabled);
    }

    public TriggerData<TEvent> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
      if (reader.TryReadNil())
      {
        return null!;
      }

      options.Security.DepthStep(ref reader);
      IFormatterResolver formatterResolver = options.Resolver;
      var length = reader.ReadArrayHeader();
      string? name = null;
      ImmutableList<ICondition>? conditions = null;
      ImmutableList<IEffect>? effects = null;
      var disabled = false;

      for (int i = 0; i < length; i++)
      {
        switch (i)
        {
          case 0:
            name = formatterResolver.GetFormatterWithVerify<string>().Deserialize(ref reader, options);
            break;
          case 1:
            conditions = formatterResolver.GetFormatterWithVerify<ImmutableList<ICondition>>()
              .Deserialize(ref reader, options);
            break;
          case 2:
            effects = formatterResolver.GetFormatterWithVerify<ImmutableList<IEffect>>()
              .Deserialize(ref reader, options);
            break;
          case 3:
            disabled = reader.ReadBoolean();
            break;
          default:
            reader.Skip();
            break;
        }
      }

      var result = new TriggerData<TEvent>(
        name,
        conditions?.Cast<ICondition<TEvent>>().ToImmutableList(),
        effects?.Cast<IEffect<TEvent>>().ToImmutableList(),
        disabled);
      reader.Depth--;
      return result;
    }
  }
}