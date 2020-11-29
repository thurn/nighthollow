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


using Nighthollow.Data;
using Nighthollow.Generated;
using Nighthollow.Services;

#nullable enable

namespace Nighthollow.Delegates.Effects
{
  public sealed class CreateCreatureEffect : Effect
  {
    public CreateCreatureEffect(CreatureItemData data, RankValue rankPosition, FileValue filePosition, bool isMoving)
    {
      Data = data;
      RankPosition = rankPosition;
      FilePosition = filePosition;
      IsMoving = isMoving;
    }

    public CreatureItemData Data { get; }
    public RankValue RankPosition { get; }
    public FileValue FilePosition { get; }
    public bool IsMoving { get; }

    public override void Execute()
    {
      var data = CreatureUtil.Build(Root.Instance.StatsForPlayer(Data.BaseType.Owner), Data);
      if (IsMoving)
        Root.Instance.CreatureService.CreateMovingCreature(data, FilePosition, RankPosition.ToXPosition());
      else
        Root.Instance.CreatureService.AddUserCreatureAtPosition(
          Root.Instance.CreatureService.CreateUserCreature(data),
          RankPosition,
          FilePosition);
    }
  }
}
