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
using Nighthollow.Model;
using Nighthollow.Utils;
using UnityEngine;
using Object = UnityEngine.Object;
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault

namespace Nighthollow.Services
{
  public sealed class AssetService : MonoBehaviour
  {
    readonly Dictionary<string, Object> _assetCache = new Dictionary<string, Object>();

    public T Get<T>(AssetData asset) where T : Object => _assetCache[asset.Address] as T;

    public void FetchCardAssets(IEnumerable<CardData> cards, Action onComplete)
    {
      var assets = new List<AssetData>();
      foreach (var card in cards)
      {
        AddCardAssets(card, assets);
      }

      if (assets.Count > 0)
      {
        StartCoroutine(PopulateAssets(assets, onComplete));
      }
    }

    public void FetchCreatureAssets(CreatureData creature, Action onComplete)
    {
      var assets = new List<AssetData>();
      AddCreatureAssets(creature, assets);

      if (assets.Count > 0)
      {
        StartCoroutine(PopulateAssets(assets, onComplete));
      }
    }

    public void FetchProjectileAssets(ProjectileData projectile, Action onComplete)
    {
      var assets = new List<AssetData>();
      AddProjectileAssets(projectile, assets);

      if (assets.Count > 0)
      {
        StartCoroutine(PopulateAssets(assets, onComplete));
      }
    }

    void AddCardAssets(CardData card, List<AssetData> assets)
    {
      assets.Add(card.Prefab);
      assets.Add(card.Image);
      AddCreatureAssets(card.CreatureData, assets);
    }

    void AddCreatureAssets(CreatureData creature, List<AssetData> assets)
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

    void AddAttachmentAssets(AttachmentData attachmentData, ICollection<AssetData> assets)
    {
      assets.Add(attachmentData.Image);
    }

    void AddProjectileAssets(ProjectileData projectile, List<AssetData> assets)
    {
      assets.Add(projectile.Prefab);
    }

    IEnumerator PopulateAssets(IReadOnlyList<AssetData> assets, Action onComplete)
    {
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

    Type TypeForAsset(AssetData asset)
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