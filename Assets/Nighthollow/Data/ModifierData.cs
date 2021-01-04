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

using MessagePack;

#nullable enable

namespace Nighthollow.Data
{
  [MessagePackObject]
  public sealed class ModifierData
  {
    public ModifierData(ModifierTypeData baseType, IValueData? argument)
    {
      BaseType = baseType;
      Argument = argument;
    }

    [Key(0)] public ModifierTypeData BaseType { get; }
    [Key(1)] public IValueData? Argument { get; }
  }
}