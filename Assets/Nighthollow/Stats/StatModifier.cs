namespace Nighthollow.Stats
{
  public interface IStatModifier
  {
    void InsertInto(StatModifierTable table);
  }

  public sealed class StatModifier<TOperation, TValue> : IStatModifier
    where TOperation : IOperation where TValue : IStatValue
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

    public void InsertInto(StatModifierTable table) => table.InsertModifier(Stat, Operation, Lifetime);
  }
}