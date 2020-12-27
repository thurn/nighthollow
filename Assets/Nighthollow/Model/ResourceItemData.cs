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

#nullable enable

namespace Nighthollow.Model
{
  public sealed class ResourceItemData : IItemData
  {
    public string Name { get; }
    public string ImageAddress { get; }
    public string Description { get; }

    public ResourceItemData(string name, string imageAddress, string description)
    {
      Name = name;
      ImageAddress = imageAddress;
      Description = description;
    }

    public T Switch<T>(Func<CreatureItemData, T> onCreature, Func<ResourceItemData, T> onResource) => onResource(this);
  }
}
