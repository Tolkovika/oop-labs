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
        set => _name = Validator.Shortener(value?.Trim() ?? "", 3, 25, '#');
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
    /// Abstract method for creature to say hi
    /// </summary>
    public abstract void SayHi();

    /// <summary>
    /// Abstract property for creature's power
    /// </summary>
    public abstract int Power { get; }

    /// <summary>
    /// Info string for the creature
    /// </summary>
    public string Info => $"{Name} <{Level}>";

    /// <summary>
    /// Upgrade creature's level (max 10)
    /// </summary>
    public void Upgrade()
    {
        if (_level < 10) _level++;
    }
}
