using SimCore.Terrain;

namespace SimCore.Units;

public class Elf : Unit
{
    public override char Symbol => 'E';
    
    /// <summary>
    /// Number of carrots this elf is carrying (collected from tiles).
    /// </summary>
    public int CarrotsCarried { get; set; } = 0;
    
    // Elves might have bonuses in forests, etc. handled by logic later
    protected override bool CanEnter(TerrainType terrain)
    {
        // Elves love forests, hate swamps? standard logic for now
        return base.CanEnter(terrain);
    }
}
