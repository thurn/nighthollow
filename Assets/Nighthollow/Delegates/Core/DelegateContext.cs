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

using Nighthollow.Components;
using Nighthollow.Stats;

namespace Nighthollow.Delegates.Core
{
  public abstract class DelegateContext : StatEntity
  {
    public bool Implemented { get; set; }
    public int DelegateIndex { get; set; }
    public Results Results { get; }
    public Creature Self { get; }
    public IDelegate Delegate => Self.Data.Delegate;

    protected DelegateContext(Results results, Creature self)
    {
      Results = results;
      Self = self;
    }

    public T MarkNotImplemented<T>()
    {
      Implemented = false;
      return default!;
    }
  }
}