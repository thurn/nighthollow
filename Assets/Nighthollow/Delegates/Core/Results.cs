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

#nullable enable

using System.Collections.Generic;
using Nighthollow.Delegates.Effects;

namespace Nighthollow.Delegates.Core
{
  public sealed class Results
  {
    readonly List<Effect> _results = new List<Effect>();

    public void Add(Effect effect) => _results.Add(effect);

    public void AddRange(IEnumerable<Effect> effects) => _results.AddRange(effects);

    public IEnumerable<Effect> Values => _results;
  }
}