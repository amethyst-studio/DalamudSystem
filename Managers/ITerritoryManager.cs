
using Dalamud.Plugin.Services;
using Lumina.Excel.GeneratedSheets;

namespace DalamudSystem.Manager;

public class ITerritoryManager : IManager, IDisposable
{
    private readonly IDataManager DataManager;
    private readonly IEnumerable<TerritoryDetail> TerritoryDetails;

    internal ITerritoryManager() : base(nameof(ITerritoryManager))
    {
        DataManager = ICoreManager.DataManager;
        TerritoryDetails = LoadTerritoryDetails();
    }

    public TerritoryDetail? GetByZoneName(string Zone, bool Partial = true)
    {
        if (!TerritoryDetails.Any()) LoadTerritoryDetails();

        var TerrDetails =
            TerritoryDetails
                .Where(ECH => ECH.Name.Equals(Zone, StringComparison.OrdinalIgnoreCase) ||
                            Partial && ECH.Name.Contains(Zone, StringComparison.CurrentCultureIgnoreCase)).OrderBy(x => x.Name.Length);

        var TerrDetail = TerrDetails.FirstOrDefault();

        return TerrDetail!;
    }

    private IEnumerable<TerritoryDetail> LoadTerritoryDetails()
    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        return (from TerrType in DataManager.GetExcelSheet<TerritoryType>()
                let Type = TerrType.Bg.RawString.Split('/')
                where Type.Length >= 3
                where Type[2] == "twn" || Type[2] == "fld" || Type[2] == "hou"
                where !string.IsNullOrWhiteSpace(TerrType.Map.Value.PlaceName.Value.Name)
                select new TerritoryDetail
                {
                    TerritoryType = TerrType.RowId,
                    MapId = TerrType.Map.Value.RowId,
                    SizeFactor = TerrType.Map.Value.SizeFactor,
                    Name = TerrType.Map.Value.PlaceName.Value.Name
                }).ToList();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }

    public TerritoryDetail? GetByTerritoryType(ushort territoryType)
    {
        return TerritoryDetails.FirstOrDefault(x => x.TerritoryType == territoryType);
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}

public class TerritoryDetail
{
    public string Name { get; set; } = null!;
    public uint TerritoryType { get; set; }
    public uint MapId { get; set; }
    public ushort SizeFactor { get; set; }
}
