namespace Simulator;

/// <summary>
/// Orc creature with rage attribute
/// </summary>
public class Orc : Creature
{
    private int _rage;
    private int _huntCounter = 0;

    /// <summary>
    /// Rage property (0-10), can only be set during initialization
    /// </summary>
    public int Rage
    {
        get => _rage;
        init => _rage = Validator.Limiter(value, 0, 10);
    }

    /// <summary>
    /// Parameterless constructor
    /// </summary>
    public Orc() : base("Unknown Orc", 1)
    {
        _rage = 0;
    }

    /// <summary>
    /// Constructor with optional parameters
    /// </summary>
    /// <param name="name">Name of the orc</param>
    /// <param name="level">Level of the orc (default: 1)</param>
    /// <param name="rage">Rage of the orc (default: 0, clamped 0-10)</param>
    public Orc(string name, int level = 1, int rage = 0) : base(name, level)
    {
        _rage = Validator.Limiter(rage, 0, 10);
    }

    /// <summary>
    /// Override Greeting to present orc with rage
    /// </summary>
    public override string Greeting()
    {
        return $"Hi! I'm {Name}, my level is {Level}, my rage is {Rage}.";
    }

    /// <summary>
    /// Hunt method - every 2nd call increases Rage by 1 (max 10)
    /// </summary>
    public void Hunt()
    {
        _huntCounter++;
        if (_huntCounter % 2 == 0)
        {
            _rage = Validator.Limiter(_rage + 1, 0, 10);
        }
    }

    /// <summary>
    /// Power calculation: 7 * Level + 3 * Rage
    /// </summary>
    public override int Power => 7 * Level + 3 * Rage;

    /// <summary>
    /// Info string in format: Name [Level][Rage]
    /// </summary>
    public override string Info => $"{Name} [{Level}][{Rage}]";
}
