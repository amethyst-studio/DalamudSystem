
namespace DalamudSystem.Managers.Base;

public abstract class IManager(string? ManagerName) : IDisposable
{
    public readonly string ManagerName = ManagerName!;

    public abstract void Dispose();
}
