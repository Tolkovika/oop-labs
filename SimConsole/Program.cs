using System.Text;
using Simulator;
using Simulator.Maps;
using SimConsole;

// Set console encoding to support Unicode box-drawing characters
Console.OutputEncoding = Encoding.UTF8;

// Display menu
Console.Clear();
Console.WriteLine("╔════════════════════════════════════╗");
Console.WriteLine("║      SIMULATOR - Menu              ║");
Console.WriteLine("╠════════════════════════════════════╣");
Console.WriteLine("║  1. Sim1 - Creatures (Orc & Elf)   ║");
Console.WriteLine("║  2. Sim2 - Animals & Creatures     ║");
Console.WriteLine("╚════════════════════════════════════╝");
Console.WriteLine();
Console.Write("Select simulation (1 or 2): ");

var key = Console.ReadKey();
Console.WriteLine();
Console.WriteLine();

if (key.KeyChar == '1')
{
    Sim1();
}
else if (key.KeyChar == '2')
{
    Sim2();
}
else
{
    Console.WriteLine("Invalid selection. Exiting...");
}

/// <summary>
/// Original simulation with Orc and Elf on SmallSquareMap.
/// </summary>
void Sim1()
{
    SmallSquareMap map = new(5);
    List<Creature> creatures =
    [
        new Orc("Gorbag"),
        new Elf("Elandor")
    ];
    List<Point> points =
    [
        new(2, 2),  // Orc starts at (2,2)
        new(3, 1)   // Elf starts at (3,1)
    ];
    string moves = "dlrludl";

    RunSimulation(map, creatures.Cast<IMappable>().ToList(), points, moves);
}

/// <summary>
/// New simulation with Animals and Creatures on torus map.
/// </summary>
void Sim2()
{
    SmallTorusMap map = new(8, 6);
    
    // Create mappable objects
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

    RunSimulation(map, mappables, points, moves);
}

/// <summary>
/// Common simulation runner for both Sim1 and Sim2.
/// </summary>
void RunSimulation(Map map, List<IMappable> mappables, List<Point> positions, string moves)
{
    Simulation simulation = new(map, mappables, positions, moves);
    MapVisualizer visualizer = new(simulation.Map);
    
    // Hide cursor for smooth animation
    Console.CursorVisible = false;

    // Display initial state
    Console.WriteLine("═══ Initial State ═══");
    Console.WriteLine();
    visualizer.Draw();
    Console.WriteLine();
    Console.WriteLine($"Moves to execute: {moves}");
    Console.WriteLine($"Total turns: {moves.Length}");
    Console.WriteLine();
    
    // Show starting positions
    Console.WriteLine("Starting positions:");
    foreach (var m in mappables)
    {
        if (m.Position.HasValue)
        {
            Console.WriteLine($"  {m.Symbol} {m} at {m.Position}");
        }
    }
    
    Console.WriteLine();
    Console.WriteLine("Press any key to start simulation...");
    Console.ReadKey(true);

    // Run simulation turn by turn
    int turnNumber = 1;
    while (!simulation.Finished)
    {
        // Execute one turn
        simulation.Turn();

        // Clear and display new state
        Console.Clear();
        Console.WriteLine($"╔════════════════════════════════╗");
        Console.WriteLine($"║   Turn {turnNumber,-2}                       ║");
        Console.WriteLine($"╚════════════════════════════════╝");
        Console.WriteLine();

        // Show current mappable info (if not finished)
        if (!simulation.Finished)
        {
            try
            {
                Console.WriteLine($"Next: {simulation.CurrentMappable}");
                Console.WriteLine($"Next move: {simulation.CurrentMoveName}");
            }
            catch (InvalidOperationException)
            {
                // Simulation just finished on this turn
            }
        }

        Console.WriteLine();
        visualizer.Draw();
        Console.WriteLine();

        // Show current positions
        Console.WriteLine("Positions:");
        foreach (var m in mappables)
        {
            if (m.Position.HasValue)
            {
                Console.WriteLine($"  {m.Symbol} {m} at {m.Position}");
            }
        }

        Console.WriteLine();

        // Animation delay (400ms for smooth movement)
        Thread.Sleep(400);

        turnNumber++;
    }

    // Simulation complete - restore cursor
    Console.CursorVisible = true;
    
    Console.WriteLine();
    Console.WriteLine("╔════════════════════════════════╗");
    Console.WriteLine("║     Simulation Complete!       ║");
    Console.WriteLine("╚════════════════════════════════╝");
    Console.WriteLine();
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey(true);
}
