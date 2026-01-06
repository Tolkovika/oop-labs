using Simulator.Maps;

namespace Simulator;

/// <summary>
/// Abstract base class for all creatures.
/// Implements IMappable to allow placement on maps.
/// </summary>
public abstract class Creature : IMappable
{
    private string _name = "Unknown";
    private int _level = 1;

    public string Name
    {
        get => _name;
        set
        {
            string validated = Validator.Shortener(value?.Trim() ?? "", 3, 25, '#');
            // Capitalize first letter if it's a letter
            if (validated.Length > 0 && char.IsLetter(validated[0]))
            {
                validated = char.ToUpperInvariant(validated[0]) + validated.Substring(1);
            }
            _name = validated;
        }
    }

    public int Level
    {
        get => _level;
        set => _level = Validator.Limiter(value, 1, 10);
    }

    /// <summary>
    /// Map on which the creature is located. Null if creature hasn't been placed on a map yet.
    /// </summary>
    public Map? Map { get; private set; }

    /// <summary>
    /// Current position of the creature on the map. Null if creature hasn't been placed on a map yet.
    /// </summary>
    public Point? Position { get; private set; }

    /// <summary>
    /// Constructor for Creature
    /// </summary>
    /// <param name="name">Name of the creature</param>
    /// <param name="level">Level of the creature</param>
    public Creature(string name, int level)
    {
        Name = name;
        Level = level;
    }

    /// <summary>
    /// Initialize creature's map and position. Can only be called once.
    /// Adds the creature to the map at the specified position.
    /// </summary>
    /// <param name="map">Map to place creature on</param>
    /// <param name="position">Position to place creature at</param>
    /// <exception cref="InvalidOperationException">Thrown if creature already on a map</exception>
    /// <exception cref="ArgumentException">Thrown if position is invalid for the map</exception>
    public void InitMapAndPosition(Map map, Point position)
    {
        if (Map != null)
        {
            throw new InvalidOperationException(
                $"{Name} is already on a map at position {Position}");
        }

        if (!map.Exist(position))
        {
            throw new ArgumentException(
                $"Position {position} is invalid for the map", 
                nameof(position));
        }

        Map = map;
        Position = position;
        map.Add(this, position);
    }

    /// <summary>
    /// Move the creature in the specified direction.
    /// If creature is not on a map, this method does nothing.
    /// Movement follows the rules of the map (e.g., wall blocking, wrapping).
    /// Synchronizes position with the map state.
    /// </summary>
    /// <param name="direction">Direction to move</param>
    public void Go(Direction direction)
    {
        // Can't move if not on a map
        if (Map == null || Position == null)
        {
            return;
        }

        Point currentPos = Position.Value;
        Point newPos = Map.Next(currentPos, direction);

        // Only update if we actually moved
        if (!newPos.Equals(currentPos))
        {
            Map.Move(this, currentPos, newPos);
            Position = newPos;
        }
    }

    /// <summary>
    /// Abstract method for creature greeting
    /// </summary>
    public abstract string Greeting();

    /// <summary>
    /// Abstract property for creature's power
    /// </summary>
    public abstract int Power { get; }

    /// <summary>
    /// Symbol representing the creature on map visualization.
    /// </summary>
    public abstract char Symbol { get; }

    /// <summary>
    /// Abstract Info property - must be implemented by derived classes
    /// </summary>
    public abstract string Info { get; }

    /// <summary>
    /// Returns string representation in format: CLASSNAME: Info
    /// </summary>
    public override string ToString()
    {
        return $"{GetType().Name.ToUpper()}: {Info}";
    }

    /// <summary>
    /// Upgrade creature's level (max 10)
    /// </summary>
    public void Upgrade()
    {
        if (_level < 10) _level++;
    }
}

