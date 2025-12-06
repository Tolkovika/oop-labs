namespace Simulator.Maps;

/// <summary>
/// Small torus map with coordinate wrapping.
/// Can be rectangular. Map dimensions limited to maximum 20x20.
/// Movement wraps around edges - moving off one edge brings you to the opposite edge.
/// </summary>
public class SmallTorusMap : Map
{
    /// <summary>
    /// Create a square torus map with the specified size.
    /// </summary>
    /// <param name="size">Size of the square map (both width and height).</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when size is less than 5 (enforced by base Map) or greater than 20.
    /// </exception>
    public SmallTorusMap(int size) : base(size, size)
    {
        if (size > 20)
        {
            throw new ArgumentOutOfRangeException(nameof(size),
                "Size must not exceed 20 for SmallTorusMap");
        }
    }

    /// <summary>
    /// Create a rectangular torus map with specified width and height.
    /// </summary>
    /// <param name="sizeX">Width of the map.</param>
    /// <param name="sizeY">Height of the map.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when sizeX or sizeY is less than 5 (enforced by base Map) or greater than 20.
    /// </exception>
    public SmallTorusMap(int sizeX, int sizeY) : base(sizeX, sizeY)
    {
        if (sizeX > 20)
        {
            throw new ArgumentOutOfRangeException(nameof(sizeX),
                "Width must not exceed 20 for SmallTorusMap");
        }
        if (sizeY > 20)
        {
            throw new ArgumentOutOfRangeException(nameof(sizeY),
                "Height must not exceed 20 for SmallTorusMap");
        }
    }

    /// <summary>
    /// Move to next position in given direction with wrapping.
    /// Uses standard Point.Next() convention (Up = Y-1, Down = Y+1).
    /// Coordinates wrap around map edges using modulo arithmetic.
    /// </summary>
    public override Point Next(Point p, Direction d)
    {
        var nextPoint = p.Next(d);
        return new Point(WrapX(nextPoint.X), WrapY(nextPoint.Y));
    }

    /// <summary>
    /// Move to next diagonal position in given direction (45° clockwise rotation)
    /// with wrapping.
    /// Uses standard Point.NextDiagonal() convention.
    /// Coordinates wrap around map edges using modulo arithmetic.
    /// </summary>
    public override Point NextDiagonal(Point p, Direction d)
    {
        var nextPoint = p.NextDiagonal(d);
        return new Point(WrapX(nextPoint.X), WrapY(nextPoint.Y));
    }

    /// <summary>
    /// Wraps an X coordinate value to fit within [0, SizeX) using modulo arithmetic.
    /// Handles negative values correctly.
    /// </summary>
    private int WrapX(int x)
    {
        return (x % SizeX + SizeX) % SizeX;
    }

    /// <summary>
    /// Wraps a Y coordinate value to fit within [0, SizeY) using modulo arithmetic.
    /// Handles negative values correctly.
    /// </summary>
    private int WrapY(int y)
    {
        return (y % SizeY + SizeY) % SizeY;
    }
}

