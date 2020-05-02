using Magewatch.Utils;
using UnityEngine;

namespace Magewatch.Components
{
  public sealed class OnDeathStateExit : StateMachineBehaviour
  {
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      ComponentUtils.GetComponent<Creature>(animator).OnDeathAnimationCompleted();
    }
  }
}