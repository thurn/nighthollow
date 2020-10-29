namespace Nighthollow.Stats
{
  public interface IStatModifier
  {
    void InsertInto(StatTable table);
  }

  public sealed class StatModifier<TOperation, TValue> : IStatModifier
    where TOperation : IOperation where TValue : struct, IStatValue
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

    public void InsertInto(StatTable table) => table.InsertModifier(Stat, Operation, Lifetime);
  }
}