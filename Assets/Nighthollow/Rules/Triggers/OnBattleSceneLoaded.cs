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

#nullable enable

namespace Nighthollow.Rules.Triggers
{
  public interface IOnBattleSceneLoaded : IHandler
  {
    public sealed class Data : ITriggerEvent<IOnBattleSceneLoaded>
    {
      public TriggerName Name => TriggerName.OnBattleSceneLoaded;

      public IEnumerable<IEffect> Handle(Injector injector, int index, IOnBattleSceneLoaded handler) =>
        handler.OnBattleSceneLoaded(injector, index, this);
    }

    /// <summary>Called when the Battle scene has been loaded and all configuration data is ready.</summary>
    IEnumerable<IEffect> OnBattleSceneLoaded(Injector injector, int delegateIndex, Data data);
  }
}