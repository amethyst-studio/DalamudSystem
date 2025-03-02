
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using DalamudSystem.Utilities;
using ImGuiNET;

namespace DalamudSystem.Module;

public abstract class IModule : Window, IDisposable
{
    public readonly string ModuleName;
    public readonly string ModuleCommand;
    public readonly Every Every = new Every();

    public IModule(string ModuleName, ImGuiWindowFlags ImGuiFlags) : base(ModuleName, ImGuiFlags)
    {
        this.ModuleName = ModuleName;
        this.ModuleCommand = ModuleName.ToLower();
    }

    public IModule(string ModuleName, string ModuleCommand, ImGuiWindowFlags ImGuiFlags) : base(ModuleName, ImGuiFlags)
    {
        this.ModuleName = ModuleName;
        this.ModuleCommand = ModuleCommand.ToLower();
    }

    internal void Load()
    {
        SetWindowOptions();
        ICoreManager.WindowManager.AddWindow(this);
        ICoreManager.Commands.AddHandler($"/{ModuleCommand}", new CommandInfo(
          (command, args) =>
          {
              Toggle();
          }
        ));
        ICoreManager.Framework.Update += Tick;
        ILoad();
    }

    internal void Tick(IFramework Framework)
    {
        ITick(Framework);
        Every.Advance();
    }

    protected abstract void SetWindowOptions();
    protected abstract void ILoad();
    protected abstract void ITick(IFramework Framework);
    protected abstract void IDispose();

    protected string Label(string Label)
    {
        return $"{Label}##{ModuleName}";
    }

    public void Dispose()
    {
        ICoreManager.WindowManager.RemoveWindow(this);
        ICoreManager.Commands.RemoveHandler($"/{ModuleCommand}");
        ICoreManager.Framework.Update -= Tick;
        Every.Clear();
        IDispose();
        GC.SuppressFinalize(this);
    }
}
