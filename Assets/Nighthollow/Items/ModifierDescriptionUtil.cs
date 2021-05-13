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
using System.Collections.Immutable;
using System.Linq;
using Nighthollow.Data;
using Nighthollow.Editing;
using Nighthollow.Utils;

#nullable enable

namespace Nighthollow.Items
{
  public static class ModifierDescriptionUtil
  {
    public static IEnumerable<string> RenderModifiers(GameData gameData, ImmutableList<ModifierData> modifiers)
    {
      if (modifiers.Count == 0)
      {
        return ImmutableList<string>.Empty;
      }

      var entity = modifiers.Aggregate(new ModifierDescriptionProvider(gameData),
        (current, modifier) => current.Insert(modifier));
      return modifiers.Select(m => m.Describe(entity, gameData.StatData)).WhereNotNull();
    }
  }
}