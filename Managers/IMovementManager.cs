

using System.Numerics;

namespace DalamudSystem.Manager;

public class IMovementManager : IManager, IDisposable
{
  public bool IPCPluginLoaded = false;
  public Func<bool> IsReady;
  public Func<float> BuildProgress;
  public Func<bool> PathfindInProgress;
  public Func<bool> IsRunning;

  public Func<Vector3, bool, bool> PathfindAndMove;
  public Func<Vector3, float, float, Vector3> Nearest;
  public Func<Vector3, float, float, Vector3> Floor;
  public Func<bool> Reload;
  public Func<bool> Rebuild;
  internal IMovementManager() : base(nameof(IMovementManager))
  {
    IPCPluginLoaded = ICoreManager.PluginInterface.InstalledPlugins.Where((v) => v.InternalName == "vnavmesh").FirstOrDefault()?.IsLoaded ?? false;

    // Status
    IsReady = ICoreManager.PluginInterface.GetIpcSubscriber<bool>("vnavmesh.Nav.IsReady").InvokeFunc;
    BuildProgress = ICoreManager.PluginInterface.GetIpcSubscriber<float>("vnavmesh.Nav.BuildProgress").InvokeFunc;
    PathfindInProgress = ICoreManager.PluginInterface.GetIpcSubscriber<bool>("vnavmesh.Nav.PathfindInProgress").InvokeFunc;
    IsRunning = ICoreManager.PluginInterface.GetIpcSubscriber<bool>("vnavmesh.Path.IsRunning").InvokeFunc;

    // Functions
    PathfindAndMove = ICoreManager.PluginInterface.GetIpcSubscriber<Vector3, bool, bool>("vnavmesh.SimpleMove.PathfindAndMoveTo").InvokeFunc;
    Nearest = ICoreManager.PluginInterface.GetIpcSubscriber<Vector3, float, float, Vector3>("vnavmesh.Query.Mesh.NearestPoint").InvokeFunc;
    Floor = ICoreManager.PluginInterface.GetIpcSubscriber<Vector3, float, float, Vector3>("vnavmesh.Query.Mesh.PointOnFloor").InvokeFunc;
    Reload = ICoreManager.PluginInterface.GetIpcSubscriber<bool>("vnavmesh.Nav.Reload").InvokeFunc;
    Rebuild = ICoreManager.PluginInterface.GetIpcSubscriber<bool>("vnavmesh.Nav.Rebuild").InvokeFunc;
  }

  public override void Dispose()
  {
    GC.SuppressFinalize(this);
  }
}
