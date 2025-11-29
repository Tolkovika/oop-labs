namespace Simulator;

/// <summary>
/// Abstract base class for all creatures
/// </summary>
public abstract class Creature
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
    /// Abstract method for creature greeting
    /// </summary>
    public abstract string Greeting();

    /// <summary>
    /// Abstract property for creature's power
    /// </summary>
    public abstract int Power { get; }

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
