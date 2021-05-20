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

using Nighthollow.Interface;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Editing
{
  public sealed class FilterInputTextFieldCellDelegate : TextFieldEditorCellDelegate
  {
    readonly string _preferenceKey;

    public FilterInputTextFieldCellDelegate(string preferenceKey)
    {
      _preferenceKey = preferenceKey;
    }

    public override void Initialize(TextField field, IEditor parent)
    {
      field.RegisterCallback<ChangeEvent<string>>(e =>
      {
        PlayerPrefs.SetString(_preferenceKey, e.newValue);
        parent.OnDataChanged();
      });
    }

    public override void OnActivate(TextField field, Rect worldBound)
    {
      InterfaceUtils.FocusTextField(field);
    }

    public override void OnDeactivate(TextField field)
    {
      field.focusable = false;
    }
  }
}