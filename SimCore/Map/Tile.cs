using SimCore.Terrain;
using SimCore.Units.Interfaces;

namespace SimCore.Map;

public class Tile
{
    public TerrainType Terrain { get; }
    public List<IMappable> Units { get; } = [];
    
    /// <summary>
    /// Number of carrots dropped on this tile.
    /// </summary>
    public int Carrots { get; set; } = 0;

    /// <summary>
    /// Number of feathers dropped on this tile.
    /// </summary>
    public int Feathers { get; set; } = 0;

    /// <summary>
    /// Dust cloud level on this tile (reduces hit chance or slows).
    /// </summary>
    public int Dust { get; set; } = 0;

    /// <summary>
    /// Orc footprints level on this tile.
    /// </summary>
    public int Footprints { get; set; } = 0;

    public Tile(TerrainType terrain)
    {
        Terrain = terrain;
    }
}
