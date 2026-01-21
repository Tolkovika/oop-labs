using SimCore.Map;

namespace SimCore.Units.Interfaces;

public interface IMovable : IMappable
{
    void Move(Direction direction);
}
