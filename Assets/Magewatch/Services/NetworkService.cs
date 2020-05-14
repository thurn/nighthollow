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
using Google.Protobuf;
using Magewatch.API;
using UnityEngine;
using UnityEngine.Networking;

namespace Magewatch.Services
{
  public sealed class NetworkService : MonoBehaviour
  {
    public void MakeRequest(Request request)
    {
      StartCoroutine(StartRequest(request));
    }

    IEnumerator<YieldInstruction> StartRequest(IMessage request)
    {
      Debug.Log($"Sending Request {request}");
      var webRequest = new UnityWebRequest("http://localhost:3030/api", UnityWebRequest.kHttpVerbPOST);
      var uploadeHandler = new UploadHandlerRaw(request.ToByteArray())
      {
        contentType = "application/octet-stream"
      };
      var downloadHandler = new DownloadHandlerBuffer();
      webRequest.uploadHandler = uploadeHandler;
      webRequest.downloadHandler = downloadHandler;
      yield return webRequest.SendWebRequest();
      var data = downloadHandler.data;
      if (webRequest.isNetworkError || webRequest.isHttpError || webRequest.error != null || data == null)
      {
        Debug.LogError($"Error making network request: {webRequest.error} for request {request}");
      }
      else
      {
        var response = CommandList.Parser.ParseFrom(webRequest.downloadHandler.data);
        Debug.Log($"Got response: {response}");
        Root.Instance.CommandService.HandleCommands(response);
      }
    }
  }
}