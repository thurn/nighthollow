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

using UnityEngine;

#nullable enable

namespace Nighthollow.Interface.Components.Library
{
  public static class ColorPalette
  {
    public static readonly Color PrimaryText = RGB(230, 198, 172);
    public static readonly Color WindowBackground = RGB(21, 17, 17);
    public static readonly Color ItemQuantityNumber = Color.white;
    public static readonly Color TextOutlineColor = Color.black;
    public static readonly Color CommonItem = Color.white;

    static Color RGB(int red, int green, int blue, int alpha = 255) =>
      new(red / 255f, green / 255f, blue / 255f, alpha / 255f);
  }
}