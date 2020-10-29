namespace Nighthollow.Stats
{
  public interface IModifier
  {
    void InsertInto(StatTable table);
  }

  public sealed class Modifier<TOperation, TValue> : IModifier
    where TOperation : IOperation where TValue : struct, IStatValue
  {
    public AbstractStat<TOperation, TValue> Stat { get; }
    public TOperation Operation { get; }
    public ILifetime Lifetime { get; }

    public Modifier(AbstractStat<TOperation, TValue> stat, TOperation operation, ILifetime lifetime)
    {
      Stat = stat;
      Operation = operation;
      Lifetime = lifetime;
    }

    public void InsertInto(StatTable table) => table.InsertModifier(Stat, Operation, Lifetime);
  }
}