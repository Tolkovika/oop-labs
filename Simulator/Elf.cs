namespace Simulator;

/// <summary>
/// Elf creature with agility attribute
/// </summary>
public class Elf : Creature
{
    private int _agility;
    private int _singCounter = 0;

    /// <summary>
    /// Agility property (0-10), can only be set during initialization
    /// </summary>
    public int Agility
    {
        get => _agility;
        init => _agility = Validator.Limiter(value, 0, 10);
    }

    /// <summary>
    /// Parameterless constructor
    /// </summary>
    public Elf() : base("Unknown Elf", 1)
    {
        _agility = 0;
    }

    /// <summary>
    /// Constructor with optional parameters
    /// </summary>
    /// <param name="name">Name of the elf</param>
    /// <param name="level">Level of the elf (default: 1)</param>
    /// <param name="agility">Agility of the elf (default: 0, clamped 0-10)</param>
    public Elf(string name, int level = 1, int agility = 0) : base(name, level)
    {
        _agility = Validator.Limiter(agility, 0, 10);
    }

    /// <summary>
    /// Override Greeting to present elf with agility
    /// </summary>
    public override string Greeting()
    {
        return $"Hi! I'm {Name}, my level is {Level}, my agility is {Agility}.";
    }

    /// <summary>
    /// Sing method - every 3rd call increases Agility by 1 (max 10)
    /// </summary>
    public void Sing()
    {
        _singCounter++;
        if (_singCounter % 3 == 0)
        {
            _agility = Validator.Limiter(_agility + 1, 0, 10);
        }
    }

    /// <summary>
    /// Power calculation: 8 * Level + 2 * Agility
    /// </summary>
    public override int Power => 8 * Level + 2 * Agility;

    /// <summary>
    /// Symbol for map visualization.
    /// </summary>
    public override char Symbol => 'E';

    /// <summary>
    /// Info string in format: Name [Level][Agility]
    /// </summary>
    public override string Info => $"{Name} [{Level}][{Agility}]";
}
