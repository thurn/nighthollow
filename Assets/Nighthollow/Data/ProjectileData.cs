using Nighthollow.Components;
using UnityEngine;

namespace Nighthollow.Data
{
  [CreateAssetMenu(menuName = "Data/Projectile")]
  public class ProjectileData : ScriptableObject
  {
    [SerializeField] Projectile _prefab;
    public Projectile Prefab => _prefab;

    [SerializeField] int _speed;
    public int Speed => _speed;

    [SerializeField] int _hitboxSize;
    public int HitboxSize => _hitboxSize;

    public ProjectileData Clone()
    {
      return Instantiate(this);
    }
  }
}