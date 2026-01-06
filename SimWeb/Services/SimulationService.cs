using Simulator;
using Simulator.Maps;

namespace SimWeb.Services;

public class SimulationService
{
    public Simulation? Simulation { get; private set; }
    public SimulationLog? SimulationLog { get; private set; }
    public int CurrentSimType { get; private set; } = 0; // 0 = none, 1 = Sim1, 2 = Sim2, 3 = Sim3 (history)
    public int CurrentHistoryTurn { get; private set; } = 5; // Current displayed turn in Sim3

    public static readonly int[] HistoryTurns = [5, 10, 15, 20];

    public SimulationService()
    {
        // Don't initialize simulation until user selects one
    }

    public void SetupSim1()
    {
        CurrentSimType = 1;
        SimulationLog = null;
        SmallSquareMap map = new(5);
        List<IMappable> mappables =
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
        
        Simulation = new Simulation(map, mappables, points, moves);
    }

    public void SetupSim2()
    {
        CurrentSimType = 2;
        SimulationLog = null;
        SmallTorusMap map = new(8, 6);
        List<IMappable> mappables =
        [
            new Elf("Legolas"),
            new Orc("Gorbag"),
            new Animals { Description = "Rabbits", Size = 5 },
            new Birds { Description = "Eagles", CanFly = true },
            new Birds { Description = "Ostriches", CanFly = false }
        ];
        
        List<Point> points =
        [
            new(0, 0),
            new(7, 5),
            new(2, 2),
            new(5, 0),
            new(4, 5)
        ];
        
        string moves = "urdlurdlurdlurdlurdl"; // 20 moves
        
        Simulation = new Simulation(map, mappables, points, moves);
    }

    public void SetupSim3()
    {
        CurrentSimType = 3;
        CurrentHistoryTurn = 5; // Start with turn 5
        
        // Same setup as Sim2
        SmallTorusMap map = new(8, 6);
        List<IMappable> mappables =
        [
            new Elf("Legolas"),
            new Orc("Gorbag"),
            new Animals { Description = "Rabbits", Size = 5 },
            new Birds { Description = "Eagles", CanFly = true },
            new Birds { Description = "Ostriches", CanFly = false }
        ];
        
        List<Point> points =
        [
            new(1, 1),
            new(5, 4),
            new(3, 2),
            new(6, 0),
            new(0, 5)
        ];
        
        string moves = "urdlurdlurdlurdlurdl"; // 20 moves
        
        Simulation = new Simulation(map, mappables, points, moves);
        SimulationLog = new SimulationLog(Simulation);
    }

    public void SetHistoryTurn(int turn)
    {
        if (CurrentSimType == 3 && Array.Exists(HistoryTurns, t => t == turn))
        {
            CurrentHistoryTurn = turn;
        }
    }

    public void NextTurn()
    {
        if (Simulation != null && !Simulation.Finished)
        {
            Simulation.Turn();
        }
    }

    public void Reset()
    {
        if (CurrentSimType == 1) SetupSim1();
        else if (CurrentSimType == 2) SetupSim2();
        else if (CurrentSimType == 3) SetupSim3();
    }
}
