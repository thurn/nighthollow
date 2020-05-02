using Magewatch.Data;
using UnityEngine;

namespace Magewatch.Services
{
  public sealed class CommandService : MonoBehaviour
  {
    public void HandleCommand(Command command)
    {
      if (command.RunCombatCommand != null)
      {
        Root.Instance.CombatService.RunCombat(command.RunCombatCommand);
      }
    }
  }
}