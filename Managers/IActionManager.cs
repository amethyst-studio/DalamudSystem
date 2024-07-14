
using FFXIVClientStructs.FFXIV.Client.Game;

namespace DalamudSystem.Manager;

public unsafe class IActionManager : IManager, IDisposable
{
  private readonly ActionManager* ActionManagerInstance = ActionManager.Instance();

  internal IActionManager() : base(nameof(IActionManager)) {
  }

  public bool CanUse(ActionType actionType, uint aid)
  {
    return ActionManagerInstance->GetActionStatus(actionType, aid) == 0;
  }

  public bool TryToUse(ActionType actionType, uint aid)
  {
    if (CanUse(actionType, aid)) {
      return ActionManagerInstance->UseAction(actionType, aid);
    }
    return false;
  }

  public override void Dispose()
  {
    GC.SuppressFinalize(this);
  }
}
