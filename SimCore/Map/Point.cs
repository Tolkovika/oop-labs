namespace SimCore.Map;

public readonly struct Point
{
    public int X { get; }
    public int Y { get; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString() => $"({X}, {Y})";

    public Point Next(Direction d)
    {
        return d switch
        {
            Direction.Up => new Point(X, Y + 1),
            Direction.Right => new Point(X + 1, Y),
            Direction.Down => new Point(X, Y - 1),
            Direction.Left => new Point(X - 1, Y),
            _ => this
        };
    }

    public int DistanceTo(Point other) => Math.Max(Math.Abs(X - other.X), Math.Abs(Y - other.Y));
    public bool IsAdjacent(Point other) => DistanceTo(other) == 1;
    
    // Simple equality
    public override bool Equals(object? obj) => obj is Point p && X == p.X && Y == p.Y;
    public override int GetHashCode() => HashCode.Combine(X, Y);
    public static bool operator ==(Point p1, Point p2) => p1.Equals(p2);
    public static bool operator !=(Point p1, Point p2) => !p1.Equals(p2);
}

public enum Direction
{
    Up, Right, Down, Left
}
