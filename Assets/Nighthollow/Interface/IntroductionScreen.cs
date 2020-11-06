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

#nullable enable

using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Nighthollow.Interface
{
  public sealed class IntroductionScreen : VisualElement
  {
    public new sealed class UxmlFactory : UxmlFactory<IntroductionScreen, UxmlTraits>
    {
    }

    public new sealed class UxmlTraits : VisualElement.UxmlTraits
    {
    }

    public IntroductionScreen()
    {
      RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    void OnGeometryChange(GeometryChangedEvent evt)
    {
      if (Application.isPlaying)
      {
        Runner.Instance.RunCoroutine(ShowDialog());
        this.Q("ContinueButton").RegisterCallback<ClickEvent>(e =>
        {
          SceneManager.LoadScene("SchoolSelection");
        });
      }
      UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    IEnumerator<YieldInstruction> ShowDialog()
    {
      yield return new WaitForSeconds(0.5f);

      InterfaceUtils.FadeIn(this.Q("Dialog1"));
      yield return new WaitForSeconds(3f);
      InterfaceUtils.FadeIn(this.Q("Dialog2"));
      yield return new WaitForSeconds(3f);
      InterfaceUtils.FadeIn(this.Q("Dialog3"));
      yield return new WaitForSeconds(3f);
      InterfaceUtils.FadeIn(this.Q("Dialog4"));
    }
  }
}