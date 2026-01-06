using Simulator;
using Simulator.Maps;

namespace SimConsole;

/// <summary>
/// Visualizes simulation history turns in console using box-drawing characters.
/// </summary>
internal class LogVisualizer
{
    private readonly SimulationLog _log;

    /// <summary>
    /// Creates a new LogVisualizer for the specified simulation log.
    /// </summary>
    public LogVisualizer(SimulationLog log)
    {
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    /// <summary>
    /// Draws the state of the map at the specified turn index.
    /// </summary>
    /// <param name="turnIndex">Turn index (0 = starting positions).</param>
    public void Draw(int turnIndex)
    {
        if (turnIndex < 0 || turnIndex >= _log.TurnLogs.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(turnIndex),
                $"Turn index must be between 0 and {_log.TurnLogs.Count - 1}");
        }

        var turnLog = _log.TurnLogs[turnIndex];
        
        DrawTopBorder();
        
        for (int y = 0; y < _log.SizeY; y++)
        {
            Console.Write(Box.Vertical);
            
            for (int x = 0; x < _log.SizeX; x++)
            {
                var point = new Point(x, y);
                char symbol = turnLog.Symbols.TryGetValue(point, out char s) ? s : '.';
                Console.Write(symbol);
            }
            
            Console.WriteLine(Box.Vertical);
        }
        
        DrawBottomBorder();
    }

    private void DrawTopBorder()
    {
        Console.Write(Box.TopLeft);
        Console.Write(new string(Box.Horizontal, _log.SizeX));
        Console.WriteLine(Box.TopRight);
    }

    private void DrawBottomBorder()
    {
        Console.Write(Box.BottomLeft);
        Console.Write(new string(Box.Horizontal, _log.SizeX));
        Console.WriteLine(Box.BottomRight);
    }
}
