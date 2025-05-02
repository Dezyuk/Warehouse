using Warehouse.Models;

public class Zone
{
    public ZoneType ZoneType { get; set; }
    public List<Cell> Cells { get; set; }

    public Zone(ZoneType zoneType, List<Cell> cells)
    {
        ZoneType = zoneType;
        Cells = cells;
    }
}