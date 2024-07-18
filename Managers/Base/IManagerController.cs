
namespace DalamudSystem.Managers.Base;

public static class IManagerController
{
    public static readonly Dictionary<string, IManager> Managers = [];
    public static readonly HashSet<IDisposable> Disposable = [];

    public static IManager Hook(IManager Manager)
    {
        Managers.Add(Manager.ManagerName!, Manager);
        Disposable.Add(Manager);
        return Manager;
    }

    public static void Dispose()
    {
        Managers.Clear();
        foreach (IDisposable Dispose in Disposable)
        {
            Dispose.Dispose();
        }
        Disposable.Clear();
    }
}
