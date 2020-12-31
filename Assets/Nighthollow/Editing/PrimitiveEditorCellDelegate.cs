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
  public sealed class PrimitiveEditorCellDelegate<T> : EditorCellDelegate
  {
    public delegate bool ParsingFunction(string input, out T output);

    readonly ReflectivePath _reflectivePath;
    readonly ParsingFunction _parsingFunction;

    public PrimitiveEditorCellDelegate(ReflectivePath reflectivePath, ParsingFunction parsingFunction)
    {
      _reflectivePath = reflectivePath;
      _parsingFunction = parsingFunction;
    }

    public override string? Initialize(TextField field, IEditor parent)
    {
      field.RegisterCallback<ChangeEvent<string>>(e =>
      {
        if (_parsingFunction(e.newValue, out var output))
        {
          _reflectivePath.Write(output);
        }
        else
        {
          field.value = e.previousValue;
        }
      });

      return _reflectivePath.Read()?.ToString();
    }

    public override void OnActivate(TextField field, Rect worldBound)
    {
      // TextField needs time after setting focusable and calling Focus() before it works, but there is no obvious
      // event which corresponds to these operations completing.
      field.focusable = true;
      InterfaceUtils.After(0.01f, () =>
      {
        field.Focus();
        InterfaceUtils.After(0.01f, () =>
        {
          // For some reason SelectAll() only works if you SelectRange() first?
          field.SelectRange(0, 1);
          field.SelectAll();
        });
      });
    }

    public override void OnDeactivate(TextField field)
    {
      field.focusable = false;
    }
  }
}