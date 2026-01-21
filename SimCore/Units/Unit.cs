using SimCore.Map;
using SimCore.Terrain;
using SimCore.Units.Interfaces;

namespace SimCore.Units;

public abstract class Unit : IMovable
{
    public required string Name { get; init; }
    public abstract char Symbol { get; }
    public Point Position { get; protected set; }
    public GameMap? Map { get; protected set; }

    public void InitMap(GameMap map, Point position)
    {
        Map = map;
        Position = position;
    }
    
    /// <summary>
    /// Sets position externally (used by Simulation for terrain-based movement).
    /// </summary>
    public void SetPosition(Point position)
    {
        Position = position;
    }

    public virtual void Move(Direction direction)
    {
        if (Map == null) return;

        Point newPos = Position.Next(direction);
        
        // Check bounds
        if (!Map.IsValid(newPos)) return;

        // Check if terrain allows entry
        if (CanEnter(Map.At(newPos).Terrain))
        {
            Map.Move(this, Position, newPos);
            Position = newPos;
        }
    }

    protected virtual bool CanEnter(TerrainType terrain)
    {
        // Default: cannot enter river unless swimmable/flyable
        if (terrain == TerrainType.River && !(this is ISwimmable) && !(this is IFlyable)) 
            return false;
        
        return true;
    }

    public override string ToString() => $"{Symbol}{Name}";
}
