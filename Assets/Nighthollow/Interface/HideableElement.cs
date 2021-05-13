using Nighthollow.Services;
using Nighthollow.Utils;
using UnityEngine.UIElements;

#nullable enable

namespace Nighthollow.Interface
{
  public abstract class AbstractHideableElement : VisualElement
  {
    public ServiceRegistry Registry { get; set; } = null!;
    public ScreenController Controller { get; set; } = null!;
    public bool Visible { get; protected set; }
    public virtual bool ExclusiveFocus { get; } = false;
  }

  public abstract class HideableElement<TShowArgument> : AbstractHideableElement
  {
    bool _initialized;

    public new sealed class UxmlTraits : VisualElement.UxmlTraits
    {
    }

    public void Show(TShowArgument argument, bool animate = false)
    {
      if (!_initialized)
      {
        Initialize();
        _initialized = true;
      }

      Visible = true;
      style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

      if (animate)
      {
        style.opacity = new StyleFloat(v: 0f);
        InterfaceUtils.FadeIn(this, duration: 0.3f);
      }

      OnShow(argument);
    }

    public void Hide(bool animate = false)
    {
      Visible = false;
      style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

      if (animate)
      {
        style.opacity = new StyleFloat(v: 1f);
        InterfaceUtils.FadeOut(this, duration: 0.3f);
      }
    }

    protected virtual void Initialize() {}

    protected abstract void OnShow(TShowArgument argument);

    protected VisualElement FindElement(string elementName) => Find<VisualElement>(elementName);

    protected T Find<T>(string elementName) where T : class => Errors.CheckNotNull(this.Q(elementName) as T);
  }

  public readonly struct NoArguments
  {
  }

  public abstract class DefaultHideableElement : HideableElement<NoArguments>
  {
    public void Show(bool animate = false)
    {
      Show(new NoArguments(), animate);
    }

    protected sealed override void OnShow(NoArguments argument)
    {
      OnShow();
    }

    protected virtual void OnShow()
    {
    }
  }

  public sealed class HideableVisualElement : DefaultHideableElement
  {
    public new sealed class UxmlFactory : UxmlFactory<HideableVisualElement, UxmlTraits>
    {
    }

    protected override void Initialize()
    {
    }
  }
}
