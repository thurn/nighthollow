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

#nullable enable

using Nighthollow.Generated;
using UnityEngine;

namespace Nighthollow.Utils
{
  public static class Constants
  {
    public const int IndicatorBottomY = -10;
    public const int IndicatorRightX = 14;
    public const float EnemyCreatureYOffset = 0;
    public const float EnemyCreatureStartingX = 20.0f;
    public const float EnemyCreatureEndingX = -15.0f;
    public const float CreatureDespawnLeftX = -20.0f;
    public const float CreatureDespawnRightX = 25.0f;
    public const float EnemyCreatureRemoveLifeX = -11.0f;

    public const int UserCreaturesLayer = 8;
    public const int EnemyCreaturesLayer = 9;
    public const int UserProjectilesLayer = 10;
    public const int EnemyProjectilesLayer = 11;

    static readonly LayerMask UserCreaturesLayerMask = LayerMask.GetMask("UserCreatures");
    static readonly LayerMask EnemyCreaturesLayerMask = LayerMask.GetMask("EnemyCreatures");

    public static float MultiplierBasisPoints(int basisPoints) =>
      basisPoints / 10_000.0f;

    public static int FractionBasisPoints(int input, int basisPoints) =>
      Mathf.RoundToInt((input * basisPoints) / 10_000.0f);

    public static string PercentageStringBasisPoints(int basisPoints) =>
      $"{Mathf.RoundToInt(basisPoints / 100.0f)}%";

    public static string MultiplierStringBasisPoints(int basisPoints) =>
      $"{MultiplierBasisPoints(basisPoints):0.##}x";

    public static readonly Vector2 OffScreen = new Vector2(-9999, -9999);

    public static int LayerForCreatures(PlayerName playerName)
    {
      switch (playerName)
      {
        case PlayerName.User:
          return UserCreaturesLayer;
        case PlayerName.Enemy:
          return EnemyCreaturesLayer;
        default:
          throw Errors.UnknownEnumValue(playerName);
      }
    }

    public static LayerMask LayerMaskForCreatures(PlayerName playerName)
    {
      switch (playerName)
      {
        case PlayerName.User:
          return UserCreaturesLayerMask;
        case PlayerName.Enemy:
          return EnemyCreaturesLayerMask;
        default:
          throw Errors.UnknownEnumValue(playerName);
      }
    }

    public static int LayerForProjectiles(PlayerName playerName)
    {
      switch (playerName)
      {
        case PlayerName.User:
          return UserProjectilesLayer;
        case PlayerName.Enemy:
          return EnemyProjectilesLayer;
        default:
          throw Errors.UnknownEnumValue(playerName);
      }
    }

    public static Vector2 ForwardDirectionForPlayer(PlayerName owner)
    {
      switch (owner)
      {
        case PlayerName.User:
          return Vector2.right;
        case PlayerName.Enemy:
          return Vector2.left;
        default:
          throw Errors.UnknownEnumValue(owner);
      }
    }
  }
}