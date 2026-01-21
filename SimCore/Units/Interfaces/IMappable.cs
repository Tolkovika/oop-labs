using SimCore.Map;

namespace SimCore.Units.Interfaces;

public interface IMappable
{
    string Name { get; }
    char Symbol { get; }
    Point Position { get; }
    
    void InitMap(GameMap map, Point position);
}
