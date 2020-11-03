namespace Nighthollow.Stats
{
  public interface IStatModifier
  {
    void ApplyTo(StatModifierTable table);

    IStatModifier WithLifetime(ILifetime lifetime);
  }

  public sealed class StatModifier<TOperation, TValue> : IStatModifier where TOperation : IOperation
  {
    public AbstractStat<TOperation, TValue> Stat { get; }
    public TOperation Operation { get; }
    public ILifetime Lifetime { get; }

    public StatModifier(AbstractStat<TOperation, TValue> stat, TOperation operation, ILifetime lifetime)
    {
      Stat = stat;
      Operation = operation;
      Lifetime = lifetime;
    }

    public void ApplyTo(StatModifierTable table) => table.InsertModifier(Stat, Operation, Lifetime);

    public IStatModifier WithLifetime(ILifetime lifetime) =>
      new StatModifier<TOperation, TValue>(Stat, Operation, lifetime);
  }
}