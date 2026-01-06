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
    /// Mappable objects moving on the map.
    /// </summary>
    public List<IMappable> Mappables { get; }

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
    /// Mappable object which will be moving current turn.
    /// Throws InvalidOperationException if simulation finished.
    /// </summary>
    public IMappable CurrentMappable
    {
        get
        {
            if (Finished)
                throw new InvalidOperationException("Simulation has finished");
            
            return Mappables[_currentMoveIndex % Mappables.Count];
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
    /// Places mappable objects on map at starting positions.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when mappables list is empty or when number of mappables
    /// differs from number of starting positions.
    /// </exception>
    public Simulation(Map map, List<IMappable> mappables, 
                      List<Point> positions, string moves)
    {
        // Validation
        if (mappables == null || mappables.Count == 0)
            throw new ArgumentException(
                "Mappables list cannot be empty", nameof(mappables));
        
        if (positions == null || mappables.Count != positions.Count)
            throw new ArgumentException(
                "Number of mappables must match number of positions");
        
        // Assignment
        Map = map;
        Mappables = mappables;
        Positions = positions;
        Moves = moves ?? "";
        
        // Initialize mappables on map
        for (int i = 0; i < mappables.Count; i++)
        {
            mappables[i].InitMapAndPosition(map, positions[i]);
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
            CurrentMappable.Go(direction);
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
