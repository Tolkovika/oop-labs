using Simulator.Maps;

namespace Simulator;

/// <summary>
/// Manages simulation of creatures moving on a map.
/// </summary>
public class Simulation
{
    private int _currentMoveIndex = 0;

    /// <summary>
    /// Simulation's map.
    /// </summary>
    public Map Map { get; }

    /// <summary>
    /// Creatures moving on the map.
    /// </summary>
    public List<Creature> Creatures { get; }

    /// <summary>
    /// Starting positions of creatures.
    /// </summary>
    public List<Point> Positions { get; }

    /// <summary>
    /// Cyclic list of creatures moves.
    /// Bad moves are ignored - use DirectionParser.
    /// First move is for first creature, second for second and so on.
    /// When all creatures make moves,
    /// next move is again for first creature and so on.
    /// </summary>
    public string Moves { get; }

    /// <summary>
    /// Has all moves been done?
    /// </summary>
    public bool Finished { get; private set; } = false;

    /// <summary>
    /// Creature which will be moving current turn.
    /// Throws InvalidOperationException if simulation finished.
    /// </summary>
    public Creature CurrentCreature
    {
        get
        {
            if (Finished)
                throw new InvalidOperationException("Simulation has finished");
            
            return Creatures[_currentMoveIndex % Creatures.Count];
        }
    }

    /// <summary>
    /// Lowercase name of direction which will be used in current turn.
    /// Throws InvalidOperationException if simulation finished.
    /// </summary>
    public string CurrentMoveName
    {
        get
        {
            if (Finished)
                throw new InvalidOperationException("Simulation has finished");
            
            return Moves[_currentMoveIndex].ToString().ToLower();
        }
    }

    /// <summary>
    /// Simulation constructor.
    /// Places creatures on map at starting positions.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when creatures list is empty or when number of creatures
    /// differs from number of starting positions.
    /// </exception>
    public Simulation(Map map, List<Creature> creatures, 
                      List<Point> positions, string moves)
    {
        // Validation
        if (creatures == null || creatures.Count == 0)
            throw new ArgumentException(
                "Creatures list cannot be empty", nameof(creatures));
        
        if (positions == null || creatures.Count != positions.Count)
            throw new ArgumentException(
                "Number of creatures must match number of positions");
        
        // Assignment
        Map = map;
        Creatures = creatures;
        Positions = positions;
        Moves = moves ?? "";
        
        // Initialize creatures on map
        for (int i = 0; i < creatures.Count; i++)
        {
            creatures[i].InitMapAndPosition(map, positions[i]);
        }
        
        // Check if no moves - simulation already finished
        if (Moves.Length == 0)
        {
            Finished = true;
        }
    }

    /// <summary>
    /// Makes one move of current creature in current direction.
    /// Invalid moves (unparseable characters) are skipped.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when simulation is already finished.
    /// </exception>
    public void Turn()
    {
        if (Finished)
            throw new InvalidOperationException(
                "Cannot execute turn - simulation has finished");
        
        // Get current move character
        char moveChar = Moves[_currentMoveIndex];
        
        // Parse to Direction (returns empty array for invalid chars)
        Direction[] parsedDirections = DirectionParser.Parse(moveChar.ToString());
        
        // Execute move if valid
        if (parsedDirections.Length > 0)
        {
            Direction direction = parsedDirections[0];
            CurrentCreature.Go(direction);
        }
        // Invalid moves are simply skipped (no else needed)
        
        // Advance to next move
        _currentMoveIndex++;
        
        // Check if simulation finished
        if (_currentMoveIndex >= Moves.Length)
        {
            Finished = true;
        }
    }
}
