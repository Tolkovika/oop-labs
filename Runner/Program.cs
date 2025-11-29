using Simulator;

namespace Runner;

internal static class Program
{
    private static void Main(string[] args)
    {
        Console.Write("Starting Simulator!\n");

        Console.WriteLine(">>> Running TestElfsAndOrcs");
        TestElfsAndOrcs();

        Console.WriteLine("\n>>> Running TestValidators");
        TestValidators();

        Console.WriteLine("\n>>> Running TestObjectsToString");
        TestObjectsToString();
    }

    static void TestElfsAndOrcs()
    {
        Console.WriteLine("HUNT TEST\n");
        var o = new Orc() { Name = "Gorbag", Rage = 7 };
        Console.WriteLine(o.Greeting());
        for (int i = 0; i < 10; i++)
        {
            o.Hunt();
            Console.WriteLine(o.Greeting());
        }

        Console.WriteLine("\nSING TEST\n");
        var e = new Elf("Legolas", agility: 2);
        Console.WriteLine(e.Greeting());
        for (int i = 0; i < 10; i++)
        {
            e.Sing();
            Console.WriteLine(e.Greeting());
        }

        Console.WriteLine("\nPOWER TEST\n");
        Creature[] creatures = {
            o,
            e,
            new Orc("Morgash", 3, 8),
            new Elf("Elandor", 5, 3)
        };
        foreach (Creature creature in creatures)
        {
            Console.WriteLine($"{creature.Name,-15}: {creature.Power}");
        }
    }

    static void TestValidators()
    {
        Console.WriteLine("VALIDATOR TEST - Creatures with Name/Level validation\n");

        // Test Elf with various names and levels (using Validator in Creature base class)
        var e1 = new Elf() { Name = "Shrek", Level = 20 };
        Console.WriteLine(e1.Greeting());
        e1.Upgrade();
        Console.WriteLine(e1.Info);

        var e2 = new Elf("  ", -5);  // Empty name, negative level
        Console.WriteLine(e2.Greeting());
        e2.Upgrade();
        Console.WriteLine(e2.Info);

        var e3 = new Elf("  donkey ") { Level = 7 };  // Trimmed name
        Console.WriteLine(e3.Greeting());
        e3.Upgrade();
        Console.WriteLine(e3.Info);

        var e4 = new Elf("Puss in Boots – a clever and brave cat.");  // Long name
        Console.WriteLine(e4.Greeting());
        e4.Upgrade();
        Console.WriteLine(e4.Info);

        var o1 = new Orc("a troll name", 5);
        Console.WriteLine(o1.Greeting());
        o1.Upgrade();
        Console.WriteLine(o1.Info);

        Console.WriteLine("\nVALIDATOR TEST - Animals with Description validation\n");

        var a = new Animals() { Description = " Cats " };
        Console.WriteLine(a.Info);

        a = new Animals() { Description = "Mice are great", Size = 40 };
        Console.WriteLine(a.Info);

        // Test edge cases
        var a2 = new Animals() { Description = "" };  // Empty description
        Console.WriteLine(a2.Info);

        var a3 = new Animals() { Description = "An incredibly long description that exceeds maximum length" };
        Console.WriteLine(a3.Info);
    }

    static void TestObjectsToString()
    {
        object[] myObjects = {
            new Animals() { Description = "dogs"},
            new Birds { Description = "  eagles ", Size = 10 },
            new Elf("e", 15, -3),
            new Orc("morgash", 6, 4)
        };
        Console.WriteLine("\nMy objects:");
        foreach (var o in myObjects) Console.WriteLine(o);
    }
}
