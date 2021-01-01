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

using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace Nighthollow.Editing
{
  public abstract class EditorSheetDelegate
  {
    public abstract List<string> Headings { get; }

    public abstract List<List<ICellContent>> Cells { get; }

    public virtual int? ContentHeightOverride => null;

    public virtual string? RenderPreview(object? value) => null;

    public int ColumnCount() => Math.Max(Headings.Count, Cells.Max(row => row.Count));

    public interface ICellContent
    {
      public T Switch<T>(
        Func<ReflectivePath, T> onReflectivePath,
        Func<string?, T> onLabel);
    }

    protected sealed class ReflectivePathCell : ICellContent
    {
      public ReflectivePathCell(ReflectivePath reflectivePath)
      {
        ReflectivePath = reflectivePath;
      }

      public ReflectivePath ReflectivePath { get; }

      public T Switch<T>(Func<ReflectivePath, T> onReflectivePath, Func<string?, T> onLabel) =>
        onReflectivePath(ReflectivePath);
    }

    protected sealed class LabelCell : ICellContent
    {
      public LabelCell(string? text)
      {
        Text = text;
      }

      public string? Text { get; }

      public T Switch<T>(Func<ReflectivePath, T> onReflectivePath, Func<string?, T> onLabel) =>
        onLabel(Text);
    }
  }
}