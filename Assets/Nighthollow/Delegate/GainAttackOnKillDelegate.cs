using Nighthollow.Components;
using Nighthollow.Data;
using UnityEngine;

namespace Nighthollow.Delegate
{
  [CreateAssetMenu(menuName = "Delegate/GainAttackOnKillDelegate")]
  public sealed class GainAttackOnKillDelegate : AbstractCreatureDelegate
  {
    [SerializeField] int _damageAmount;
    [SerializeField] DamageType _damageType;

    public override string Description() => $"Gains {_damageAmount} {_damageAmount} Damage on Kill";

    public override void OnKilledEnemy(Creature self, Creature enemy, int damageAmount)
    {
      Parent.OnKilledEnemy(self, enemy, damageAmount);
      self.Data.BaseAttack.Get(_damageType).AddModifier(Modifier.Create(Operator.Add, _damageAmount));
    }
  }
}