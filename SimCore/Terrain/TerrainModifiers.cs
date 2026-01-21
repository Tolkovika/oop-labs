namespace SimCore.Terrain;

public static class TerrainModifiers
{
    public static double GetMoveCost(TerrainType terrain)
    {
        return terrain switch
        {
            TerrainType.Plain => 1.0,
            TerrainType.Forest => 1.5,
            TerrainType.Mountain => 2.5,
            TerrainType.River => 999.0, // Impassable for normal units
            TerrainType.Swamp => 2.0,
            _ => 1.0
        };
    }
}
