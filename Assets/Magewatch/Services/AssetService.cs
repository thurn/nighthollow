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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Magewatch.API;
using Magewatch.Utils;
using UnityEngine;
using Object = UnityEngine.Object;
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault

namespace Magewatch.Services
{
  public sealed class AssetService : MonoBehaviour
  {
    readonly Dictionary<string, Object> _assetCache = new Dictionary<string, Object>();

    public T Get<T>(Asset asset) where T : Object => _assetCache[asset.Address] as T;

    public void FetchAssets(CommandList commandList, Action onComplete)
    {
      var assets = new List<Asset>();
      foreach (var step in commandList.CommandGroups)
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

      if (command.UseCreatureSkill != null)
      {
        AddOnImpactNumberAssets(command.UseCreatureSkill.OnImpact, assets);
      }
    }

    void AddCardAssets(CardData card, List<Asset> assets)
    {
      assets.Add(card.Prefab);
      assets.Add(card.Image);

      switch (card.CardTypeCase)
      {
        case CardData.CardTypeOneofCase.CreatureCard:
          AddCreatureAssets(card.CreatureCard, assets);
          break;
        case CardData.CardTypeOneofCase.AttachmentCard:
          AddAttachmentAssets(card.AttachmentCard, assets);
          break;
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

    void AddOnImpactNumberAssets(IEnumerable<MOnImpactNumber> impactEffects, List<Asset> assets)
    {
      foreach (var onImpact in impactEffects)
      {
        switch (onImpact.Effect.OnImpactCase)
        {
          case MOnImpact.OnImpactOneofCase.FireProjectile:
            assets.Add(onImpact.Effect.FireProjectile.Projectile);
            break;
        }
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
        var asset = requests[i].asset;
        _assetCache[assets[i].Address] = Errors.CheckNotNull(asset, $"For asset address: '{assets[i].Address}'");
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