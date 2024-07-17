
namespace DalamudSystem.Utilities;

public static class IPCVector3
{
    public static System.Numerics.Vector3 Convert(FFXIVClientStructs.FFXIV.Common.Math.Vector3 Base)
    {
        return new System.Numerics.Vector3(Base.X, Base.Y, Base.Z);
    }
}
