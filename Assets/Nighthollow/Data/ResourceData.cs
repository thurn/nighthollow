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
using MessagePack;

#nullable enable

namespace Nighthollow.Data
{
  [MessagePackObject]
  public sealed partial class ResourceTypeData
  {
    public ResourceTypeData(string name = "<unnamed>", string? imageAddress = null, string? description = null)
    {
      Name = name;
      ImageAddress = imageAddress;
      Description = description;
    }

    [Key(0)] public string Name { get; }
    [Key(1)] public string? ImageAddress { get; }
    [Key(2)] public string? Description { get; }

    public string? GetImageAddress(GameData gameData) => ImageAddress;

    public T Switch<T>(
      Func<CreatureItemData, T> onCreature,
      Func<ResourceTypeData, T> onResource) => onResource(this);

    public override string ToString() => Name;
  }

  [MessagePackObject]
  public sealed partial class ResourceItemData : IItemData
  {
    public ResourceItemData(int resourceTypeId, string name, int quantity)
    {
      ResourceTypeId = resourceTypeId;
      Name = name;
      Quantity = quantity;
    }

    [ForeignKey(typeof(ResourceTypeData))]
    [Key(0)] public int ResourceTypeId { get; }
    [Key(1)] public string Name { get; }
    [Key(2)] public int Quantity { get; }

    public int? GetQuantity() => Quantity;

    public string? GetImageAddress(GameData gameData) => gameData.ResourceTypes[ResourceTypeId].ImageAddress;

    public T Switch<T>(Func<CreatureItemData, T> onCreature, Func<ResourceItemData, T> onResource) => onResource(this);

    public override string ToString() => $"{Name} {Quantity}x";
  }
}
