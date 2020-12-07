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


using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public sealed class BlackoutWindow : HideableElement<float>
  {
    public new sealed class UxmlFactory : UxmlFactory<BlackoutWindow, UxmlTraits>
    {
    }

    protected override void Initialize()
    {
    }

    protected override void OnShow(float argument)
    {
      style.opacity = new StyleFloat(argument);
    }
  }
}
