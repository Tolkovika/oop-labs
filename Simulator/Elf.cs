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
        init => _agility = Clamp(value, 0, 10);
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
        _agility = Clamp(agility, 0, 10);
    }

    /// <summary>
    /// Override SayHi to present elf with agility
    /// </summary>
    public override void SayHi()
    {
        Console.WriteLine($"Hi! I'm {Name}, my level is {Level}, my agility is {Agility}.");
    }

    /// <summary>
    /// Sing method - every 3rd call increases Agility by 1 (max 10)
    /// </summary>
    public void Sing()
    {
        _singCounter++;
        if (_singCounter % 3 == 0)
        {
            if (_agility < 10)
            {
                _agility++;
            }
        }
    }

    /// <summary>
    /// Power calculation: 8 * Level + 2 * Agility
    /// </summary>
    public override int Power => 8 * Level + 2 * Agility;

    /// <summary>
    /// Clamps a value between min and max
    /// </summary>
    private static int Clamp(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}
