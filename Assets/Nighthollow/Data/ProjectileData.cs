using Nighthollow.Components;
using UnityEngine;

namespace Nighthollow.Data
{
  [CreateAssetMenu(menuName = "Data/Projectile")]
  public class ProjectileData : ScriptableObject
  {
    public Projectile Prefab;
    public int Speed;
    public int HitboxSize;
  }
}