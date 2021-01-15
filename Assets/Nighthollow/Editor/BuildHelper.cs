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

using UnityEditor;

#nullable enable

namespace Nighthollow.Editor
{
  public static class BuildHelper
  {
    [MenuItem("Tools/Run Build Helper &#^b")]
    public static void Run()
    {
      // The default limit on nested generics in IL2CPP is 7, but microsoft loves nesting, so we need to increase it
      // for System.Collections.Immutable to work.
      PlayerSettings.SetAdditionalIl2CppArgs("--maximum-recursive-generic-depth 30");
    }
  }
}
