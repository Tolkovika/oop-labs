using Simulator.Maps;

namespace Simulator;

public static class SimulationScenarios
{
    public static (Map, List<IMappable>, List<Point>, string) GetSim1()
    {
        SmallSquareMap map = new(5);
        List<IMappable> creatures =
        [
            new Orc("Gorbag"),
            new Elf("Elandor")
        ];
        List<Point> points =
        [
            new(2, 2),
            new(3, 1)
        ];
        string moves = "dlrludl";

        return (map, creatures, points, moves);
    }

    public static (Map, List<IMappable>, List<Point>, string) GetSim2()
    {
        SmallTorusMap map = new(8, 6);
        List<IMappable> mappables =
        [
            new Elf("Legolas"),
            new Orc("Gorbag"),
            new Animals { Description = "Rabbits", Size = 5 },
            new Birds { Description = "Eagles", Size = 3, CanFly = true },
            new Birds { Description = "Ostriches", Size = 4, CanFly = false }
        ];
        
        List<Point> points =
        [
            new(1, 1),  // Elf
            new(5, 4),  // Orc
            new(3, 2),  // Rabbits
            new(6, 0),  // Eagles
            new(0, 5)   // Ostriches
        ];
        
        string moves = "urdlurdlurdlurdlurdl"; // 20 moves

        return (map, mappables, points, moves);
    }
}
