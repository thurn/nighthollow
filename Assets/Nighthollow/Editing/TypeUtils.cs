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
using System.Linq;

#nullable enable

namespace Nighthollow.Editing
{
  public static class TypeUtils
  {
    public static Type UnboxNullable(Type type) =>
      IsNullable(type) ? type.GetGenericArguments()[0] : type;

    public static bool IsNullable(Type type) =>
      type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

    public static object InstantiateWithDefaults(Type type)
    {
      var constructor = type.GetConstructors()[0];
      var arguments = constructor.GetParameters()
        .Select(parameter =>
          parameter.ParameterType.IsValueType ? Activator.CreateInstance(parameter.ParameterType) : null!);
      return constructor.Invoke(arguments.ToArray());
    }
  }
}