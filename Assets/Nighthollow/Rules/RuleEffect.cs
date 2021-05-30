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

using System.Collections.Immutable;
using MessagePack;
using Nighthollow.Rules.Effects;

#nullable enable

namespace Nighthollow.Rules
{
  public interface IHasEffects
  {
    public ImmutableList<RuleEffect> Effects { get; }

    public IHasEffects WithNewEffects(ImmutableList<RuleEffect> effects);
  }

  [Union(0, typeof(DisplayHelpTextEffect))]
  [Union(1, typeof(CharacterDialogueEffect))]
  [Union(2, typeof(PreventDefaultEffect))]
  [Union(3, typeof(LoadSceneEffect))]
  [Union(4, typeof(InitializeWorldMapEffect))]
  [Union(5, typeof(CenterCameraOnHexEffect))]
  [Union(6, typeof(EnableAllRulesEffect))]
  [Union(7, typeof(PlaceCreaturesFromListEffect))]
  [Union(8, typeof(ClearScenariosEffect))]
  [Union(9, typeof(ResetToNewGameStateEffect))]
  public abstract class RuleEffect : IHasDescription
  {
    public abstract Description Describe();

    public abstract ImmutableHashSet<IKey> GetDependencies();

    public abstract void Execute(IEffectScope scope, RuleOutput? output);

    public override string ToString()
    {
      var description = Description.Describe(this);
      return description.Length < 100 ? description : $"{description.Substring(0, 100)}...";
    }
  }
}
