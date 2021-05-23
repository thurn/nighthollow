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
using System.Linq;
using System.Reflection;
using System.Text;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Rules
{
  public sealed class Description
  {
    readonly List<string> _tokens;

    public Description(params string[] tokens)
    {
      Errors.CheckArgument(tokens.Length > 0, "Tokens must be non-empty");
      _tokens = tokens.ToList();
    }

    public void Iterate(
      object instance, Action<string> onString,
      Action<PropertyInfo> onProperty,
      string? prefix = null)
    {
      var properties = instance.GetType()
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .ToDictionary(p => p.Name, p => p);
      var addedPrefix = false;

      foreach (var token in _tokens)
      {
        if (properties.ContainsKey(token))
        {
          onProperty(properties[token]);
        }
        else
        {
          if (!addedPrefix && prefix != null)
          {
            onString($"{prefix} {token}");
            addedPrefix = true;
          }
          else
          {
            onString(token);
          }
        }
      }
    }

    public string Render(object instance)
    {
      var result = new StringBuilder();
      Iterate(
        instance,
        s => { result.Append(s).Append(" "); },
        p => { result.Append(p.GetValue(instance)).Append(" "); });

      return result.ToString();
    }

    public static string Describe(object instance) => GetDescription(instance.GetType()).Render(instance);

    public static string Snippet(string prefix, Type type) => $"{prefix} {GetDescription(type)._tokens.First()}";

    public static Description GetDescription(Type type)
    {
      var property = Errors.CheckNotNull(type)
        .GetProperty("Describe", BindingFlags.Static | BindingFlags.Public);
      if (property == null)
      {
        throw new ArgumentException($"Type {type.Name} has not implemented the Describe property!");
      }

      return (Description) property.GetValue(type);
    }
  }
}