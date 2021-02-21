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
using Nighthollow.Data;

#nullable enable

namespace Nighthollow.Delegates.Handlers
{
  public interface IOnActivated : IHandler
  {
    public sealed class Data : EventData<IOnActivated>
    {
      public Data(CreatureState self)
      {
        Self = self;
      }

      public override ImmutableList<Effect> Invoke(DelegateContext c, IOnActivated handler) =>
        handler.OnActivated(c, this);

      public CreatureState Self { get; }
    }

    /// <summary>Called when a creature is first placed.</summary>
    ImmutableList<Effect> OnActivated(DelegateContext c, Data d);
  }
}