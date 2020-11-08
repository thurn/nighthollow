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
using UnityEngine;
using UnityEngine.UIElements;

namespace Nighthollow.Interface
{
  [RequireComponent(typeof(UIDocument))]
  public sealed class Dialog : MonoBehaviour
  {
    Action _onClose;

    public void Initialize(string text, Action onClose = null)
    {
      _onClose = onClose;
      StartCoroutine(InitializeAsync(text));
    }

    IEnumerator<YieldInstruction> InitializeAsync(string text)
    {
      yield return new WaitForSeconds(0.01f);
      var document = GetComponent<UIDocument>();
      var overlay = (DialogOverlay) document.rootVisualElement.Children().First();
      overlay.Initialize(text, this);
    }

    public void OnClose()
    {
      Destroy(gameObject);
      _onClose?.Invoke();
    }
  }

  public sealed class DialogOverlay : VisualElement
  {
    Label _text;
    Dialog _parent;

    public new sealed class UxmlFactory : UxmlFactory<DialogOverlay, UxmlTraits>
    {
    }

    public new sealed class UxmlTraits : VisualElement.UxmlTraits
    {
    }

    public DialogOverlay()
    {
      RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    public void Initialize(string text, Dialog dialog)
    {
      _text.text = text;
      _parent = dialog;
    }

    void OnGeometryChange(GeometryChangedEvent evt)
    {
      var dialogBox = this.Q("DialogBox");
      InterfaceUtils.FadeIn(dialogBox, 0.3f);
      _text = (Label) this.Q("DialogText");
      this.Q("DialogClose").RegisterCallback<MouseEnterEvent>(e => Debug.Log("Mouse Enter"));
      this.Q("DialogText").RegisterCallback<MouseEnterEvent>(e => Debug.Log("DT Mouse Enter"));
      this.Q("DialogClose").RegisterCallback<ClickEvent>(e =>
      {
        Debug.Log("On Close");
        InterfaceUtils.FadeOut(dialogBox, 0.3f, () =>
        {
          _parent.OnClose();
        });
      });
      UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }
  }
}
