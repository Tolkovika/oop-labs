namespace Simulator.Maps;

/// <summary>
/// Abstract base class for all rectangular maps.
/// Provides common functionality for size management, boundary checking,
/// and creature tracking.
/// </summary>
public abstract class Map
{
    private readonly Dictionary<Point, List<IMappable>> _fields = new();
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

    #region Mappable Object Management

    /// <summary>
    /// Add a mappable object to the map at the specified position.
    /// Multiple objects can occupy the same position.
    /// </summary>
    /// <param name="mappable">Object to add.</param>
    /// <param name="position">Position where to place the object.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when position is outside map bounds.
    /// </exception>
    public void Add(IMappable mappable, Point position)
    {
        if (!Exist(position))
        {
            throw new ArgumentException(
                $"Position {position} is outside map bounds", 
                nameof(position));
        }

        if (!_fields.ContainsKey(position))
        {
            _fields[position] = new List<IMappable>();
        }

        _fields[position].Add(mappable);
    }

    /// <summary>
    /// Remove a mappable object from the specified position on the map.
    /// </summary>
    /// <param name="mappable">Object to remove.</param>
    /// <param name="position">Position from which to remove the object.</param>
    /// <returns>True if object was found and removed, false otherwise.</returns>
    public bool Remove(IMappable mappable, Point position)
    {
        if (!_fields.ContainsKey(position))
        {
            return false;
        }

        bool removed = _fields[position].Remove(mappable);

        // Clean up empty lists to save memory
        if (_fields[position].Count == 0)
        {
            _fields.Remove(position);
        }

        return removed;
    }

    /// <summary>
    /// Move a mappable object from one position to another on the map.
    /// This is an atomic operation - both remove and add succeed or neither does.
    /// </summary>
    /// <param name="mappable">Object to move.</param>
    /// <param name="oldPosition">Current position of the object.</param>
    /// <param name="newPosition">New position for the object.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when new position is outside map bounds or object not at old position.
    /// </exception>
    public void Move(IMappable mappable, Point oldPosition, Point newPosition)
    {
        if (!Exist(newPosition))
        {
            throw new ArgumentException(
                $"Position {newPosition} is outside map bounds", 
                nameof(newPosition));
        }

        if (!Remove(mappable, oldPosition))
        {
            throw new ArgumentException(
                $"Object not found at position {oldPosition}", 
                nameof(oldPosition));
        }

        Add(mappable, newPosition);
    }

    /// <summary>
    /// Get all mappable objects at the specified position.
    /// Returns an empty read-only list if no objects at that position.
    /// </summary>
    /// <param name="position">Position to query.</param>
    /// <returns>Read-only list of objects at the position.</returns>
    public IReadOnlyList<IMappable> At(Point position)
    {
        if (_fields.ContainsKey(position))
        {
            return _fields[position].AsReadOnly();
        }
        return new List<IMappable>().AsReadOnly();
    }

    /// <summary>
    /// Get all mappable objects at the specified coordinates.
    /// Returns an empty read-only list if no objects at that position.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <returns>Read-only list of objects at the position.</returns>
    public IReadOnlyList<IMappable> At(int x, int y)
    {
        return At(new Point(x, y));
    }

    #endregion
}
