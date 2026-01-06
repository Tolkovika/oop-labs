namespace Simulator.Maps;

/// <summary>
/// Interface for objects that can be placed and moved on a map.
/// Enables loose coupling between Map/Simulation and actual object types.
/// </summary>
public interface IMappable
{
    /// <summary>
    /// Current position of the object on the map.
    /// </summary>
    Point? Position { get; }

    /// <summary>
    /// Map on which the object is located.
    /// </summary>
    Map? Map { get; }

    /// <summary>
    /// Symbol representing the object on map visualization.
    /// </summary>
    char Symbol { get; }

    /// <summary>
    /// Initialize the object's map and starting position.
    /// </summary>
    /// <param name="map">Map to place object on.</param>
    /// <param name="position">Starting position.</param>
    void InitMapAndPosition(Map map, Point position);

    /// <summary>
    /// Move the object in the specified direction.
    /// </summary>
    /// <param name="direction">Direction to move.</param>
    void Go(Direction direction);

    /// <summary>
    /// String representation of the mappable object.
    /// Suggests implementers override ToString() to return non-null string.
    /// </summary>
    string ToString();
}
