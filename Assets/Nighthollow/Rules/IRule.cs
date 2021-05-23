namespace Nighthollow.Rules
{
  public interface IRule
  {
    /// <summary>Returns true if this rule should handle trigger events with the provided TriggerName.</summary>
    bool Matches(TriggerName name);

    TResult Invoke<THandler, TResult>(Injector injector, int index, ITrigger<THandler, TResult> trigger);
  }
}