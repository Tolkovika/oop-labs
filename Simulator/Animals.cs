using Simulator.Maps;

namespace Simulator;

/// <summary>
/// Base class for all animals. Implements IMappable for map placement.
/// </summary>
public class Animals : IMappable
{
    private string _description = string.Empty;
    private bool _descriptionSetOnce = false;

    public string Description
    {
        get => _description;
        set
        {
            if (_descriptionSetOnce) return;
            _description = Normalize(value);
            _descriptionSetOnce = true;
        }
    }

    public uint Size { get; set; } = 3;

    /// <summary>
    /// Map on which the animal is located.
    /// </summary>
    public Map? Map { get; private set; }

    /// <summary>
    /// Current position of the animal on the map.
    /// </summary>
    public Point? Position { get; set; }

    /// <summary>
    /// Symbol representing the animal on map visualization.
    /// </summary>
    public virtual char Symbol => 'A';
    
    /// <summary>
    /// Number of carrots this animal is carrying (rabbits start with 10).
    /// </summary>
    public int Carrots { get; set; } = 10;
    
    /// <summary>
    /// Tracks turns since last spawn for rabbit multiplication.
    /// </summary>
    public int TurnsSinceSpawn { get; set; } = 0;
    
    /// <summary>
    /// Returns true if this animal is a rabbit (based on Description).
    /// </summary>
    public bool IsRabbit => Description?.Contains("Rabbit", StringComparison.OrdinalIgnoreCase) == true;

    /// <summary>
    /// Initialize animal's map and position.
    /// </summary>
    public void InitMapAndPosition(Map map, Point position)
    {
        if (Map != null)
        {
            throw new InvalidOperationException(
                $"Animal is already on a map at position {Position}");
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
    /// Move the animal in the specified direction.
    /// Standard animals move one position.
    /// </summary>
    public virtual void Go(Direction direction)
    {
        if (Map == null || Position == null)
        {
            return;
        }

        Point currentPos = Position.Value;
        Point newPos = Map.Next(currentPos, direction);

        if (!newPos.Equals(currentPos))
        {
            Map.Move(this, currentPos, newPos);
            Position = newPos;
        }
    }

    /// <summary>
    /// Virtual Info property - can be overridden by derived classes
    /// Format: Description <Size>
    /// </summary>
    public virtual string Info => $"{Description} <{Size}>";

    /// <summary>
    /// Returns string representation in format: ANIMALS: Info
    /// </summary>
    public override string ToString()
    {
        return $"{GetType().Name.ToUpper()}: {Info}";
    }

    private static string Normalize(string? input)
    {
        // First trim the input
        string s = (input ?? string.Empty).Trim();

        // Use Validator.Shortener for length validation (min: 3, max: 15, placeholder: '#')
        s = Validator.Shortener(s, 3, 15, '#');

        // Capitalize first letter, rest lowercase
        if (s.Length > 0 && char.IsLetter(s[0]))
        {
            s = char.ToUpperInvariant(s[0]) + s.Substring(1).ToLowerInvariant();
        }

        return s;
    }
}

