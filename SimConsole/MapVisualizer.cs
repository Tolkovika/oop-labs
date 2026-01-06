using Simulator;
using Simulator.Maps;

namespace SimConsole;

/// <summary>
/// Visualizes the state of a map in console using box-drawing characters.
/// </summary>
public class MapVisualizer
{
    private readonly Map _map;

    /// <summary>
    /// Creates a new MapVisualizer for the specified map.
    /// </summary>
    /// <param name="map">Map to visualize.</param>
    /// <exception cref="ArgumentNullException">Thrown when map is null.</exception>
    public MapVisualizer(Map map)
    {
        _map = map ?? throw new ArgumentNullException(nameof(map));
    }

    /// <summary>
    /// Draws the current state of the map with creatures.
    /// Renders:
    /// - Box border using Unicode characters
    /// - Empty cells as '.'
    /// - Single creature using its Symbol property (E for Elf, O for Orc)
    /// - Multiple creatures on same cell as 'X'
    /// Note: Does NOT clear the console - caller is responsible for clearing/positioning cursor.
    /// </summary>
    public void Draw()
    {
        DrawTopBorder();
        
        // Draw each row of the map
        for (int y = 0; y < _map.SizeY; y++)
        {
            Console.Write(Box.Vertical);
            
            // Draw each column in current row
            for (int x = 0; x < _map.SizeX; x++)
            {
                Console.Write(GetSymbolAt(x, y));
            }
            
            Console.WriteLine(Box.Vertical);
        }
        
        DrawBottomBorder();
    }

    /// <summary>
    /// Draws the top border of the map.
    /// Format: ┌─────┐
    /// </summary>
    private void DrawTopBorder()
    {
        Console.Write(Box.TopLeft);
        Console.Write(new string(Box.Horizontal, _map.SizeX));
        Console.WriteLine(Box.TopRight);
    }

    /// <summary>
    /// Draws the bottom border of the map.
    /// Format: └─────┘
    /// </summary>
    private void DrawBottomBorder()
    {
        Console.Write(Box.BottomLeft);
        Console.Write(new string(Box.Horizontal, _map.SizeX));
        Console.WriteLine(Box.BottomRight);
    }

    /// <summary>
    /// Gets the symbol to display at the specified position.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <returns>
    /// '.' if no creatures,
    /// creature.Symbol if exactly one creature (E or O),
    /// 'X' if multiple creatures.
    /// </returns>
    private char GetSymbolAt(int x, int y)
    {
        var mappables = _map.At(x, y);
        
        if (mappables.Count == 0) 
            return '.';
        
        if (mappables.Count == 1) 
            return mappables[0].Symbol;
        
        return 'X'; // Multiple objects
    }
}
