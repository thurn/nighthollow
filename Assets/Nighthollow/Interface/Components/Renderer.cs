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

using Nighthollow.Utils;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface.Components
{
  public static class Renderer
  {
    /// <summary>
    /// Runs the tree diff algorithm, updating the Visual Element hierarchy to match the new state.
    /// </summary>
    /// <param name="globalKey">The <see cref="GlobalKey"/> instance corresponding to the tree in <paramref name="newTree"/></param>
    /// <param name="previousElement">The previous <see cref="VisualElement"/> corresponding to the current tree
    /// position, if any</param>
    /// <param name="previousTree">The previous <see cref="IMountComponent"/> corresponding to this tree position,
    /// if any</param>
    /// <param name="newTree">The new <see cref="IMountComponent"/> being rendered at this position</param>
    /// <returns>A newly-created VisualElement for this tree position if one was necessary. If
    /// <paramref name="previousElement"/> was mutated instead, returns null.</returns>
    public static VisualElement? Update(
      GlobalKey globalKey,
      VisualElement? previousElement,
      IMountComponent? previousTree,
      IMountComponent newTree)
    {
      newTree.ReduceChildren(globalKey);
      var newChildren = newTree.GetReducedChildren();
      var addedChildrenCount = 0;
      VisualElement result;
      var createdNewElement = false;

      if (previousElement != null && previousTree != null && previousTree.Type == newTree.Type)
      {
        var prevChildren = previousTree.GetReducedChildren();
        addedChildrenCount = prevChildren.Count;

        for (var i = 0; i < addedChildrenCount; ++i)
        {
          if (i < newChildren.Count)
          {
            var child = newChildren[i];
            // Element exists in new tree, update it
            var updated = Update(
              globalKey.Child(child.Key ?? i.ToString()),
              previousElement[i],
              i < prevChildren?.Count ? prevChildren[i] : null,
              child);
            if (updated != null)
            {
              // New element was created for this position, replace existing element
              previousElement.Insert(i, updated);
              previousElement.RemoveAt(i + 1);
            }
          }
          else
          {
            // Element does not exist in new tree, delete it
            previousElement.RemoveAt(i);
          }
        }

        result = previousElement;
      }
      else
      {
        result = newTree.CreateMountContent();
        createdNewElement = true;
      }

      for (var j = addedChildrenCount; j < newChildren.Count; ++j)
      {
        var child = newChildren[j];
        var updated = Errors.CheckNotNull(Update(globalKey.Child(child.Key ?? j.ToString()), null, null, child),
          $"Expected {nameof(Update)}() to return a value");
        result.Add(updated);
      }

      newTree.Mount(globalKey, result);

      return createdNewElement ? result : null;
    }
  }
}