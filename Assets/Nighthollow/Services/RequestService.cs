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

using clojure.clr.api;
using clojure.lang;
using Nighthollow.Model;
using UnityEngine;

namespace Nighthollow.Services
{
  public sealed class RequestService : MonoBehaviour
  {
    const string Namespace = "nighthollow.main";
    IFn _onStartNewGame;
    IFn _onCardDrawn;
    IFn _onPlayedCard;

    void Start()
    {
      Arcadia.Util.require(Namespace);
      _onStartNewGame = Clojure.var(Namespace, "on-start-new-game");
      _onCardDrawn = Clojure.var(Namespace, "on-card-drawn");
      _onPlayedCard = Clojure.var(Namespace, "on-played-card");
    }

    public void OnCardDrawn(CardId cardId)
    {
      _onCardDrawn.invoke(cardId.Value);
    }

    public void OnStartNewGame(CommandService commands)
    {
      _onStartNewGame.invoke(commands);
    }

    public void OnPlayedCard(CardId cardId, RankValue targetRank, FileValue targetFile)
    {
      _onPlayedCard.invoke(cardId, targetRank, targetFile);
    }
  }
}