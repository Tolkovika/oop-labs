using SimCore.Map;
using SimCore.Terrain;
using SimCore.Units.Interfaces;

namespace SimCore.Units;

public class Bird : Unit, IFlyable
{
    public override char Symbol => 'B';

    public bool IsEagle => Name.Contains("Eagle", StringComparison.OrdinalIgnoreCase);
    public bool IsOstrich => Name.Contains("Ostrich", StringComparison.OrdinalIgnoreCase);

    public override void Move(Direction direction)
    {
        // Birds can fly, maybe move 2 squares? Or just fly over rivers.
        // For now, implementing flying over river logic via CanEnter override or IFlyable check
        base.Move(direction);
    }

    protected override bool CanEnter(TerrainType terrain)
    {
        // Birds can fly over anything
        return true; 
    }
}
