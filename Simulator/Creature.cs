namespace Simulator;

/// <summary>
/// Abstract base class for all creatures
/// </summary>
public abstract class Creature
{
    public string Name { get; set; }
    public int Level { get; set; }

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
}
