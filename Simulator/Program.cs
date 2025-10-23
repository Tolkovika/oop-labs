namespace Simulator;

internal static class Program
{
    private static void Main(string[] args)
    {
        Console.Write("Starting Simulator!\n");

        Console.WriteLine($"[DBG] Assembly: {System.Reflection.Assembly.GetExecutingAssembly().Location}");


        Console.WriteLine(">>> Running TestCreatures");
        TestCreatures();

        Console.WriteLine("[DBG] Entering TestDirections()");

        Console.WriteLine(">>> Running TestDirections");
        TestDirections();
    }

    static void TestDirections()
    {
        Creature c = new("Shrek", 7);
        c.SayHi();

        Console.WriteLine("\n* Up");
        c.Go(Direction.Up);

        Console.WriteLine("\n* Right, Left, Left, Down");
        Direction[] directions = {
            Direction.Right, Direction.Left, Direction.Left, Direction.Down
        };
        c.Go(directions);

        Console.WriteLine("\n* LRL");
        c.Go("LRL");

        Console.WriteLine("\n* xxxdR lyyLTyu");
        c.Go("xxxdR lyyLTyu");
    }

    static void TestCreatures()
    {
        Creature c = new() { Name = "Shrek", Level = 20 };
        c.SayHi();
        c.Upgrade();
        Console.WriteLine(c.Info);

        c = new("  ", -5);
        c.SayHi();
        c.Upgrade();
        Console.WriteLine(c.Info);

        c = new("  donkey ") { Level = 7 };
        c.SayHi();
        c.Upgrade();
        Console.WriteLine(c.Info);

        c = new("Puss in Boots – a clever and brave cat.");
        c.SayHi();
        c.Upgrade();
        Console.WriteLine(c.Info);

        c = new("a troll name", 5);
        c.SayHi();
        c.Upgrade();
        Console.WriteLine(c.Info);

        var a = new Animals() { Description = " Cats " };
        Console.WriteLine(a.Info);

        a = new() { Description = "Mice are great", Size = 40 };
        Console.WriteLine(a.Info);
    }
}
