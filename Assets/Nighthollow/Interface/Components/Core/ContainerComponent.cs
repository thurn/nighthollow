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
using Nighthollow.Utils;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface.Components.Core
{
  public abstract record ContainerComponent<TElement> : MountComponent<TElement> where TElement : VisualElement
  {
    readonly IEnumerable<BaseComponent> _children = Enumerable.Empty<BaseComponent>();
    ImmutableList<IMountComponent>? _reduced;

    public IEnumerable<BaseComponent> Children
    {
      init => _children = value;
    }

    public override void ReduceChildren(GlobalKey globalKey)
    {
      Errors.CheckState(_reduced is null, "Error: Already reduced!");
      _reduced = _children.Select((c, i) => c.Reduce(globalKey.Child(c.LocalKey ?? i.ToString()))).ToImmutableList();
    }

    public override ImmutableList<IMountComponent> GetReducedChildren() =>
      Errors.CheckNotNull(_reduced, "You must invoke ReduceChildren() first!");
  }
}