using SimCore.Terrain;
using SimCore.Units.Interfaces;

namespace SimCore.Units;

public class Animal : Unit
{
    public override char Symbol => 'A';
    
    /// <summary>
    /// Number of carrots this animal is carrying (rabbits start with 10).
    /// </summary>
    public int Carrots { get; set; } = 10;
    
    /// <summary>
    /// Returns true if this animal is a rabbit (based on Name).
    /// </summary>
    public bool IsRabbit => Name?.Contains("Rabbit", StringComparison.OrdinalIgnoreCase) == true;
}
