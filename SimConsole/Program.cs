using System.Text;
using Simulator;
using Simulator.Maps;
using SimConsole;

// Set console encoding to support Unicode box-drawing characters
Console.OutputEncoding = Encoding.UTF8;

// TIP: For best display, consider using a Unicode-compatible font
// like Lucida Console, Consolas, or Cascadia Code in your terminal.

Console.WriteLine("╔════════════════════════════════╗");
Console.WriteLine("║   SIMULATOR - Console Demo    ║");
Console.WriteLine("╚════════════════════════════════╝");
Console.WriteLine();

// Create simulation setup
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

// Create simulation with IMappable list (cast from Creature list)
Simulation simulation = new(map, creatures.Cast<IMappable>().ToList(), points, moves);
MapVisualizer visualizer = new(simulation.Map);

// Display initial state
Console.WriteLine("═══ Initial State ═══");
Console.WriteLine();
visualizer.Draw();
Console.WriteLine();
Console.WriteLine($"Moves to execute: {moves}");
Console.WriteLine($"Total turns: {moves.Length}");
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
            var current = simulation.CurrentMappable as Creature;
            Console.WriteLine($"Next creature: {current?.Name ?? "Unknown"}");
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
    
    // Show creature positions
    Console.WriteLine("Creature positions:");
    foreach (var creature in creatures)
    {
        if (creature.Position.HasValue)
        {
            Console.WriteLine($"  {creature.Symbol} {creature.Name,-10} at {creature.Position}");
        }
    }
    
    Console.WriteLine();
    
    // Animation delay
    Thread.Sleep(800);
    
    turnNumber++;
}

// Simulation complete
Console.WriteLine();
Console.WriteLine("╔════════════════════════════════╗");
Console.WriteLine("║     Simulation Complete!       ║");
Console.WriteLine("╚════════════════════════════════╝");
Console.WriteLine();
Console.WriteLine("Press any key to exit...");
Console.ReadKey(true);
