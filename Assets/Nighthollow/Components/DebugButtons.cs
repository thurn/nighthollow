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

using System.Collections.Generic;
using Nighthollow.Services;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Nighthollow.Components
{
  public sealed class DebugButtons : MonoBehaviour
  {
    public void HideButtons()
    {
      gameObject.SetActive(false);
    }

    public void Slow()
    {
      Time.timeScale = 0.05f;
    }

    public void Fast()
    {
      Time.timeScale = 1.0f;
    }

    public void LoadData()
    {
      StartCoroutine(LoadDataAsync());
    }

    IEnumerator<YieldInstruction> LoadDataAsync()
    {
      const string sheetId = "1tCHDRkul9IjF-16JVgOX7qUS_VTIVemuk9Pu_iONdwI";
      const string apiKey = "AIzaSyAh7HQu6r9J55bMAqW60IvX_oZ5RIIwO7g";
      var request = UnityWebRequest.Get(
        $"https://sheets.googleapis.com/v4/spreadsheets/{sheetId}/values:batchGet?ranges=Sheet1&ranges=Sheet2&key={apiKey}");
      yield return request.SendWebRequest();

      if (request.isNetworkError || request.isHttpError)
      {
        Debug.LogError(request.error);
      }
      else
      {
        var parsed = JSON.Parse(request.downloadHandler.text);
        Debug.Log(parsed["valueRanges"][0]["values"][0][0]);
        Debug.Log(parsed["valueRanges"][0]["values"][0][1]);
        Debug.Log(request.downloadHandler.text);
      }
    }

    public void StartGame()
    {
      Root.Instance.User.OnStartGame();
      Root.Instance.Enemy.StartSpawningEnemies();
    }

    public void ResetGame()
    {
      SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    public void Draft()
    {
      var rewardSelector = FindObjectOfType<RewardSelector>();
      if (rewardSelector)
      {
        Destroy(rewardSelector.gameObject);
      }
      else
      {
        Root.Instance.RewardService.DraftRewards();
      }
    }

    public void DeckBuilder()
    {
      var deckBuilder = FindObjectOfType<DeckBuilder>();
      if (deckBuilder)
      {
        Destroy(deckBuilder.gameObject);
      }
      else
      {
        Root.Instance.Prefabs.CreateDeckBuilder();
      }
    }
  }
}