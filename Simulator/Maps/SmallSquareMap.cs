namespace Simulator.Maps;

/// <summary>
/// Small square map with wall blocking.
/// Map dimensions are square (SizeX == SizeY) and limited to 5-20.
/// Movement is blocked at walls - attempting to move outside returns the same position.
/// </summary>
public class SmallSquareMap : Map
{
    /// <summary>
    /// Create a square map with the specified size.
    /// </summary>
    /// <param name="size">Size of the square map (both width and height).</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when size is less than 5 (enforced by base Map) or greater than 20.
    /// </exception>
    public SmallSquareMap(int size) : base(size, size)
    {
        // Base class validates size >= 5
        // We add the upper limit validation for "Small" maps
        if (size > 20)
        {
            throw new ArgumentOutOfRangeException(nameof(size), 
                "Size must not exceed 20 for SmallSquareMap");
        }
    }

    /// <summary>
    /// Move to next position in given direction, blocking at walls.
    /// If the next position would be outside the map, returns the current position.
    /// </summary>
    public override Point Next(Point p, Direction d)
    {
        var nextPoint = p.Next(d);
        return Exist(nextPoint) ? nextPoint : p;
    }

    /// <summary>
    /// Move to next diagonal position in given direction (45° clockwise rotation),
    /// blocking at walls.
    /// If the next position would be outside the map, returns the current position.
    /// </summary>
    public override Point NextDiagonal(Point p, Direction d)
    {
        var nextPoint = p.NextDiagonal(d);
        return Exist(nextPoint) ? nextPoint : p;
    }
}

