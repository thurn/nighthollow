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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Magewatch.Data;
using Magewatch.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Magewatch.Services
{
  public sealed class AssetService : MonoBehaviour
  {
    readonly Dictionary<string, Object> _assetCache = new Dictionary<string, Object>();

    public T Get<T>(Asset asset) where T : Object => _assetCache[asset.Address] as T;

    public void FetchAssets(CommandList commandList, Action onComplete)
    {
      var assets = new List<Asset>();
      foreach (var step in commandList.Steps)
      {
        foreach (var command in step.Commands)
        {
          AddAssetsForCommand(command, assets);
        }
      }

      StartCoroutine(PopulateAssets(assets, onComplete));
    }

    void AddAssetsForCommand(Command command, List<Asset> assets)
    {
      if (command.DrawCard != null)
      {
        AddCardAssets(command.DrawCard.Card, assets);
      }

      if (command.PlayCard != null)
      {
        AddCardAssets(command.PlayCard.Card, assets);
      }

      if (command.CreateOrUpdateCreature != null)
      {
        AddCreatureAssets(command.CreateOrUpdateCreature.Creature, assets);
      }

      if (command.Attack != null)
      {
        AddAttackEffectAssets(command.Attack.AttackEffect, assets);
      }
    }

    void AddCardAssets(CardData card, List<Asset> assets)
    {
      assets.Add(card.Prefab);
      assets.Add(card.Image);

      if (card.CreatureData != null)
      {
        AddCreatureAssets(card.CreatureData, assets);
        ;
      }

      if (card.AttachmentData != null)
      {
        AddAttachmentAssets(card.AttachmentData, assets);
      }
    }

    void AddCreatureAssets(CreatureData creature, List<Asset> assets)
    {
      assets.Add(creature.Prefab);

      if (creature.Attachments != null)
      {
        foreach (var attachment in creature.Attachments)
        {
          AddAttachmentAssets(attachment, assets);
        }
      }
    }

    void AddAttachmentAssets(AttachmentData attachmentData, List<Asset> assets)
    {
      assets.Add(attachmentData.Image);
    }

    void AddAttackEffectAssets(AttackEffect attackEffect, List<Asset> assets)
    {
      if (attackEffect.FireProjectile != null)
      {
        assets.Add(attackEffect.FireProjectile.Prefab);
      }
    }

    IEnumerator PopulateAssets(List<Asset> assets, Action onComplete)
    {
      assets.RemoveAll(x => x == null);
      var requests = assets.Select(asset => Resources.LoadAsync(asset.Address, TypeForAsset(asset))).ToList();

      foreach (var request in requests)
      {
        yield return request;
      }

      for (var i = 0; i < requests.Count; ++i)
      {
        _assetCache[assets[i].Address] = requests[i].asset;
      }

      onComplete();
    }

    Type TypeForAsset(Asset asset)
    {
      switch (asset.AssetType)
      {
        case AssetType.Prefab:
          return typeof(GameObject);
        case AssetType.Sprite:
          return typeof(Sprite);
        default:
          throw Errors.UnknownEnumValue(asset.AssetType);
      }
    }
  }
}