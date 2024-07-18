using FFXIVClientStructs.FFXIV.Common.Math;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalamudSystem.Utilities.Helpers
{
    public static class MapHelper
    {
        public static (int X, int Y) MapToWorldCoordinates(Vector2 pos, uint mapId)
        {
            ushort scale = ICoreManager.DataManager.GetExcelSheet<Map>()?.GetRow(mapId)?.SizeFactor ?? 100;
            float num = scale / 100f;
            float x = (float)(((pos.X - 1.0) * num / 41.0 * 2048.0) - 1024.0) / num * 1000f;
            float y = (float)(((pos.Y - 1.0) * num / 41.0 * 2048.0) - 1024.0) / num * 1000f;
            x = (int)(MathF.Round(x, 3, MidpointRounding.AwayFromZero) * 1000) * 0.001f / 1000f;
            y = (int)(MathF.Round(y, 3, MidpointRounding.AwayFromZero) * 1000) * 0.001f / 1000f;
            return ((int)x, (int)y);
        }
    }
}
