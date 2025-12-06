namespace Simulator.Maps;

/// <summary>
/// Abstract base class for all rectangular maps.
/// Provides common functionality for size management, boundary checking,
/// and creature tracking.
/// </summary>
public abstract class Map
{
    private readonly Dictionary<Point, List<Creature>> _creatures = new();
    private readonly Rectangle area;

    public readonly int SizeX;
    public readonly int SizeY;

    /// <summary>
    /// Protected constructor for derived classes.
    /// Validates that dimensions are at least 5x5.
    /// </summary>
    /// <param name="sizeX">Width of the map (must be >= 5).</param>
    /// <param name="sizeY">Height of the map (must be >= 5).</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when sizeX or sizeY is less than 5.
    /// </exception>
    protected Map(int sizeX, int sizeY)
    {
        if (sizeX < 5)
            throw new ArgumentOutOfRangeException(nameof(sizeX));
        if (sizeY < 5)
            throw new ArgumentOutOfRangeException(nameof(sizeY));

        SizeX = sizeX;
        SizeY = sizeY;
        area = new Rectangle(0, 0, SizeX - 1, SizeY - 1);
    }

    /// <summary>
    /// Check if given point belongs to the map.
    /// </summary>
    /// <param name="p">Point to check.</param>
    /// <returns>True if point is within map bounds.</returns>
    public virtual bool Exist(Point p) => area.Contains(p);

    /// <summary>
    /// Next position to the point in a given direction.
    /// Derived classes define how movement works (e.g., blocking vs wrapping).
    /// </summary>
    /// <param name="p">Starting point.</param>
    /// <param name="d">Direction.</param>
    /// <returns>Next point.</returns>
    public abstract Point Next(Point p, Direction d);

    /// <summary>
    /// Next diagonal position to the point in a given direction
    /// rotated 45 degrees clockwise.
    /// </summary>
    /// <param name="p">Starting point.</param>
    /// <param name="d">Direction.</param>
    /// <returns>Next point.</returns>
    public abstract Point NextDiagonal(Point p, Direction d);

    #region Creature Management

    /// <summary>
    /// Add a creature to the map at the specified position.
    /// Multiple creatures can occupy the same position.
    /// </summary>
    /// <param name="creature">Creature to add.</param>
    /// <param name="position">Position where to place the creature.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when position is outside map bounds.
    /// </exception>
    public void Add(Creature creature, Point position)
    {
        if (!Exist(position))
        {
            throw new ArgumentException(
                $"Position {position} is outside map bounds", 
                nameof(position));
        }

        if (!_creatures.ContainsKey(position))
        {
            _creatures[position] = new List<Creature>();
        }

        _creatures[position].Add(creature);
    }

    /// <summary>
    /// Remove a creature from the specified position on the map.
    /// </summary>
    /// <param name="creature">Creature to remove.</param>
    /// <param name="position">Position from which to remove the creature.</param>
    /// <returns>True if creature was found and removed, false otherwise.</returns>
    public bool Remove(Creature creature, Point position)
    {
        if (!_creatures.ContainsKey(position))
        {
            return false;
        }

        bool removed = _creatures[position].Remove(creature);

        // Clean up empty lists to save memory
        if (_creatures[position].Count == 0)
        {
            _creatures.Remove(position);
        }

        return removed;
    }

    /// <summary>
    /// Move a creature from one position to another on the map.
    /// This is an atomic operation - both remove and add succeed or neither does.
    /// </summary>
    /// <param name="creature">Creature to move.</param>
    /// <param name="oldPosition">Current position of the creature.</param>
    /// <param name="newPosition">New position for the creature.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when new position is outside map bounds or creature not at old position.
    /// </exception>
    public void Move(Creature creature, Point oldPosition, Point newPosition)
    {
        if (!Exist(newPosition))
        {
            throw new ArgumentException(
                $"Position {newPosition} is outside map bounds", 
                nameof(newPosition));
        }

        if (!Remove(creature, oldPosition))
        {
            throw new ArgumentException(
                $"Creature not found at position {oldPosition}", 
                nameof(oldPosition));
        }

        Add(creature, newPosition);
    }

    /// <summary>
    /// Get all creatures at the specified position.
    /// Returns an empty read-only list if no creatures at that position.
    /// </summary>
    /// <param name="position">Position to query.</param>
    /// <returns>Read-only list of creatures at the position.</returns>
    public IReadOnlyList<Creature> At(Point position)
    {
        if (_creatures.ContainsKey(position))
        {
            return _creatures[position].AsReadOnly();
        }
        return new List<Creature>().AsReadOnly();
    }

    /// <summary>
    /// Get all creatures at the specified coordinates.
    /// Returns an empty read-only list if no creatures at that position.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <returns>Read-only list of creatures at the position.</returns>
    public IReadOnlyList<Creature> At(int x, int y)
    {
        return At(new Point(x, y));
    }

    #endregion
}
