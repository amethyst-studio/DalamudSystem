
namespace DalamudSystem.Module;

public static class IModuleController
{
    public static readonly Dictionary<string, IModule> Modules = [];
    public static readonly HashSet<IDisposable> Disposable = [];

    public static IModule Hook(IModule Module)
    {
        Modules.Add(Module.ModuleName, Module);
        Disposable.Add(Module);
        Module.Load();
        return Module;
    }

    public static void Dispose()
    {
        Modules.Clear();
        foreach (IDisposable Dispose in Disposable)
        {
            Dispose.Dispose();
        }
        Disposable.Clear();
    }
}
