using Simulator.Maps;

namespace Simulator;

/// <summary>
/// State of map after single simulation turn.
/// </summary>
public class TurnLog
{
    /// <summary>
    /// Text representation of moving object in this turn.
    /// CurrentMappable.ToString()
    /// </summary>
    public required string Mappable { get; init; }
    
    /// <summary>
    /// Text representation of move in this turn.
    /// CurrentMoveName.ToString();
    /// </summary>
    public required string Move { get; init; }
    
    /// <summary>
    /// Dictionary of IMappable.Symbol on the map in this turn.
    /// Key is position, value is symbol character.
    /// </summary>
    public required Dictionary<Point, char> Symbols { get; init; }
}
