using Dalamud.Game.ClientState.Objects;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using DalamudSystem.Manager;

namespace DalamudSystem;

#nullable disable
public class ICoreManager
{
  // Plugin Constants
  [PluginService] public static IDalamudPluginInterface PluginInterface { get; private set; }
  [PluginService] public static IFramework Framework { get; private set; }
  [PluginService] public static ICommandManager Commands { get; private set; }
  [PluginService] public static IClientState ClientState { get; private set; }

  [PluginService] public static ICondition Condition { get; private set; }
  [PluginService] public static IKeyState KeyState { get; private set; }
  [PluginService] public static ITargetManager Targets { get; private set; }
  [PluginService] public static IDataManager DataManager { get; private set; }

  [PluginService] public static IChatGui Chat { get; private set; }
  [PluginService] public static IObjectTable Objects { get; private set; }
  [PluginService] public static IAetheryteList AetheryteList { get; private set; }
  [PluginService] public static IFateTable FateTable { get; private set; }
  [PluginService] public static IPluginLog Log { get; private set; }

  // Internal Systems
  public static WindowSystem WindowManager { get; private set; }

  // Logical Initialization
  internal static bool IsInitialized = false;
  public static void Initialize(IDalamudPluginInterface Interface)
  {
    try
    {
      if (IsInitialized)
      {
        Log.Debug("Internal Services Already Initialized.");
        return;
      }
      IsInitialized = true;

      // Create Interface
      Interface.Create<ICoreManager>();

      // Propogate Builtin Modules
      WindowManager = new WindowSystem(Interface.InternalName);

      // Register Managers
      IManagerController.Hook(new IActionManager());
      IManagerController.Hook(new IMovementManager());

      // Register Disposables
      PluginInterface.UiBuilder.Draw += WindowManager.Draw;
    }
    catch (Exception except)
    {
      Console.WriteLine($"Failed to Load Service(s): {except.Message}");
    }
  }

  public static void Dispose()
  {
    // Stop Rendering
    PluginInterface.UiBuilder.Draw -= WindowManager.Draw;

    // Dispose Builtin Modules
    IModuleController.Dispose();
    IManagerController.Dispose();
    WindowManager.RemoveAllWindows();
  }
}
