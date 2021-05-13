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

using MessagePack;
using Nighthollow.Utils;
using UnityEngine.SceneManagement;

#nullable enable

namespace Nighthollow.Triggers.Effects
{
  public enum SceneName
  {
    Unknown = 0,
    MainMenu = 1,
    Introduction = 2,
    SchoolSelection = 3,
    World = 4,
    Battle = 5
  }

  [MessagePackObject]
  public sealed partial class LoadSceneEffect : IEffect<TriggerEvent>
  {
    public static Description Describe => new Description("load the scene", nameof(SceneName));

    public LoadSceneEffect(SceneName sceneName)
    {
      SceneName = sceneName;
    }

    [Key(0)] public SceneName SceneName { get; }

    public void Execute(TriggerEvent trigger, TriggerOutput? output)
    {
      SceneManager.LoadScene(SceneName switch
      {
        SceneName.MainMenu => "MainMenu",
        SceneName.Introduction => "Introduction",
        SceneName.SchoolSelection => "SchoolSelection",
        SceneName.World => "World",
        SceneName.Battle => "Battle",
        _ => throw Errors.UnknownEnumValue(SceneName)
      });
    }
  }
}