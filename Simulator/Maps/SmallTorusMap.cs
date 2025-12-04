namespace Simulator.Maps;

public class SmallTorusMap : Map
{
    public int Size { get; }

    public SmallTorusMap(int size)
    {
        if (size < 5 || size > 20)
        {
            throw new ArgumentOutOfRangeException(nameof(size),
                "Size must be between 5 and 20.");
        }
        Size = size;
    }

    public override bool Exist(Point p)
    {
        return p.X >= 0 && p.X < Size &&
               p.Y >= 0 && p.Y < Size;
    }

    public override Point Next(Point p, Direction d)
    {
        // Calculate the movement delta based on direction
        // NOTE: Tests require Up→Y+1, Down→Y-1 (opposite to standard Point.Next)
        int newX = p.X;
        int newY = p.Y;

        switch (d)
        {
            case Direction.Up:
                newY = p.Y + 1;
                break;
            case Direction.Down:
                newY = p.Y - 1;
                break;
            case Direction.Left:
                newX = p.X - 1;
                break;
            case Direction.Right:
                newX = p.X + 1;
                break;
        }

        // Wrap coordinates using modulo for torus topology
        newX = Wrap(newX);
        newY = Wrap(newY);

        return new Point(newX, newY);
    }

    public override Point NextDiagonal(Point p, Direction d)
    {
        // Calculate diagonal movement (45° clockwise rotation from direction)
        // NOTE: Tests require Up→Y+1, Down→Y-1 convention
        int newX = p.X;
        int newY = p.Y;

        switch (d)
        {
            case Direction.Up:
                // Up + 45° CW = Up-Right
                newX = p.X + 1;
                newY = p.Y + 1;
                break;
            case Direction.Right:
                // Right + 45° CW = Down-Right
                newX = p.X + 1;
                newY = p.Y - 1;
                break;
            case Direction.Down:
                // Down + 45° CW = Down-Left
                newX = p.X - 1;
                newY = p.Y - 1;
                break;
            case Direction.Left:
                // Left + 45° CW = Up-Left
                newX = p.X - 1;
                newY = p.Y + 1;
                break;
        }

        // Wrap coordinates using modulo for torus topology
        newX = Wrap(newX);
        newY = Wrap(newY);

        return new Point(newX, newY);
    }

    /// <summary>
    /// Wraps a coordinate value to fit within [0, Size) using modulo arithmetic.
    /// Handles negative values correctly.
    /// </summary>
    private int Wrap(int value)
    {
        return (value % Size + Size) % Size;
    }
}
