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

using System;

namespace Magewatch.Utils
{
  public static class Errors
  {
    public static T CheckNotNull<T>(T value) where T : class
    {
      switch (value)
      {
        case null:
        case UnityEngine.Object c when !c:
          // UnityEngine.Object has weird null behavior
          throw new NullReferenceException($"Expected a non-null object of type {typeof(T).FullName}");
        default:
          return value;
      }
    }

    public static T CheckNotNull<T>(T? value) where T : struct
    {
      if (!value.HasValue)
      {
        throw new ArgumentException($"Expected a non-null value of type {typeof(T).FullName}");
      }

      return value.Value;
    }

    public static T CheckEnum<T>(T value) where T : Enum
    {
      if (Equals(value, default(T)))
      {
        throw new ArgumentException($"Expected enum value of type {typeof(T).FullName} to have a non-default value.");

      }

      return value;
    }

    public static int CheckPositive(int value)
    {
      if (value <= 0)
      {
        throw new ArgumentException($"Expected value {value} to be > 0.");
      }

      return value;
    }

    public static Exception UnknownEnumValue<T>(T value) where T : Enum
      => new ArgumentException($"Unknown '{typeof(T).Name}' value: '{value}'");


    public static Exception UnknownIntEnumValue(int value, int minimum, int maximum)
      => new ArgumentException($"Int value '{value}' must be between '{minimum}' and '{maximum}' (inclusive)");

    public static Exception MustInitialize(string name)
      => new InvalidOperationException($"Must call Initialize() on {name} before Start()!");

    public static void CheckArgument(bool expression, string message)
    {
      if (!expression)
      {
        throw new ArgumentException(message);
      }
    }
  }
}