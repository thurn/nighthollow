using System.Collections.Generic;
using Magewatch.Components;
using UnityEngine;

namespace Magewatch.Services
{
  public sealed class CreatureService : MonoBehaviour
  {
    readonly Dictionary<int, Creature> _creatures = new Dictionary<int, Creature>();

    public void Add(Creature creature)
    {
      _creatures[creature.CreatureId] = creature;
    }

    public Creature Get(int creatureId) => _creatures[creatureId];
  }
}