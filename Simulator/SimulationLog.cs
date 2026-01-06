using Simulator.Maps;

namespace Simulator;

/// <summary>
/// Runs a simulation and records all turns for later replay.
/// </summary>
public class SimulationLog
{
    private Simulation _simulation { get; }
    
    /// <summary>
    /// Map width.
    /// </summary>
    public int SizeX { get; }
    
    /// <summary>
    /// Map height.
    /// </summary>
    public int SizeY { get; }
    
    /// <summary>
    /// List of turn logs. Index 0 contains starting positions.
    /// </summary>
    public List<TurnLog> TurnLogs { get; } = [];

    /// <summary>
    /// Creates a simulation log by running the entire simulation
    /// and recording each turn.
    /// </summary>
    public SimulationLog(Simulation simulation)
    {
        _simulation = simulation ??
            throw new ArgumentNullException(nameof(simulation));
        SizeX = _simulation.Map.SizeX;
        SizeY = _simulation.Map.SizeY;
        Run();
    }

    /// <summary>
    /// Runs the simulation and records all turns.
    /// </summary>
    private void Run()
    {
        // Store initial state at index 0
        TurnLogs.Add(new TurnLog
        {
            Mappable = "Start",
            Move = "-",
            Symbols = CaptureSymbols()
        });

        // Run simulation and capture each turn
        while (!_simulation.Finished)
        {
            // Capture info about current turn BEFORE executing it
            string mappable = _simulation.CurrentMappable.ToString() ?? "";
            string move = _simulation.CurrentMoveName;
            
            // Execute the turn
            _simulation.Turn();
            
            // Capture map state AFTER the turn
            TurnLogs.Add(new TurnLog
            {
                Mappable = mappable,
                Move = move,
                Symbols = CaptureSymbols()
            });
        }
    }

    /// <summary>
    /// Captures current state of all symbols on the map.
    /// </summary>
    private Dictionary<Point, char> CaptureSymbols()
    {
        var symbols = new Dictionary<Point, char>();
        
        for (int x = 0; x < SizeX; x++)
        {
            for (int y = 0; y < SizeY; y++)
            {
                var point = new Point(x, y);
                var mappables = _simulation.Map.At(point);
                
                if (mappables.Count == 1)
                {
                    symbols[point] = mappables[0].Symbol;
                }
                else if (mappables.Count > 1)
                {
                    symbols[point] = 'X'; // Multiple objects
                }
            }
        }
        
        return symbols;
    }
}
