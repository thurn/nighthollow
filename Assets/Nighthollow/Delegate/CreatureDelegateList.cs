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
using System.Linq;

namespace Nighthollow.Delegate
{
  public sealed class CreatureDelegateList : AbstractCreatureDelegate
  {
    public void Initialize(List<AbstractCreatureDelegate> delegates)
    {
      if (delegates != null && delegates.Count > 0)
      {
        delegates[0].SetParent(CreateInstance<DefaultCreatureDelegate>());
        for (var i = delegates.Count - 1; i > 0; i--)
        {
          delegates[i].SetParent(delegates[i - 1]);
        }

        SetParent(delegates.Last());
      }
      else
      {
        SetParent(CreateInstance<DefaultCreatureDelegate>());
      }
    }

    public void AddDelegate(AbstractCreatureDelegate add)
    {
      add.SetParent(Parent);
      SetParent(add);
    }
  }
}