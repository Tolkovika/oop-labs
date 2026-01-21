using SimCore;
using SimCore.Map;
using SimCore.Units;
using SimCore.Units.Interfaces;
using SimCore.Terrain;

namespace SimConsole;

/// <summary>
/// Factory for creating predefined simulation scenarios.
/// </summary>
public static class ScenarioFactory
{
    public static (Simulation sim, string name) CreateScenario(int scenarioNumber)
    {
        return scenarioNumber switch
        {
            1 => CreateScenario1(),
            2 => CreateScenario2(),
            3 => CreateScenario3(),
            _ => CreateScenario1()
        };
    }

    /// <summary>
    /// Scenario 1: Classic Elf vs Orc battle on small map.
    /// </summary>
    private static (Simulation, string) CreateScenario1()
    {
        var map = new GameMap(10, 8);
        
        // Add some terrain
        map.SetTerrain(4, 3, TerrainType.Forest);
        map.SetTerrain(5, 3, TerrainType.Forest);
        map.SetTerrain(4, 4, TerrainType.Forest);
        map.SetTerrain(2, 5, TerrainType.Mountain);
        map.SetTerrain(7, 2, TerrainType.River);
        map.SetTerrain(7, 3, TerrainType.River);

        var units = new List<IMovable>
        {
            new Elf { Name = "Legolas" },
            new Elf { Name = "Elandor" },
            new Orc { Name = "Gorbag" },
            new Orc { Name = "Ugluk" },
            new Orc { Name = "Shagrat" }
        };

        // Place units
        var positions = new List<Point>
        {
            new(1, 1),  // Legolas
            new(2, 3),  // Elandor
            new(8, 6),  // Gorbag
            new(7, 5),  // Ugluk
            new(6, 4)   // Shagrat
        };

        for (int i = 0; i < units.Count; i++)
        {
            units[i].InitMap(map, positions[i]);
        }

        var sim = new Simulation(map, units);
        return (sim, "⚔ Battle Arena: Elves vs Orcs");
    }

    /// <summary>
    /// Scenario 2: Mixed units with varied terrain.
    /// </summary>
    private static (Simulation, string) CreateScenario2()
    {
        var map = new GameMap(12, 8);
        
        // Create diverse terrain
        // Forest patch
        for (int x = 3; x <= 5; x++)
            for (int y = 2; y <= 4; y++)
                map.SetTerrain(x, y, TerrainType.Forest);
        
        // River
        for (int y = 0; y < 8; y++)
            map.SetTerrain(8, y, TerrainType.River);
        
        // Mountain range
        map.SetTerrain(1, 6, TerrainType.Mountain);
        map.SetTerrain(2, 6, TerrainType.Mountain);
        map.SetTerrain(2, 7, TerrainType.Mountain);
        
        // Swamp
        map.SetTerrain(10, 5, TerrainType.Swamp);
        map.SetTerrain(10, 6, TerrainType.Swamp);
        map.SetTerrain(11, 5, TerrainType.Swamp);

        var units = new List<IMovable>
        {
            new Elf { Name = "Arwen" },
            new Orc { Name = "Bolg" },
            new Animal { Name = "Wolf" },
            new Animal { Name = "Bear" },
            new Bird { Name = "Eagle" },
            new Bird { Name = "Raven" }
        };

        var positions = new List<Point>
        {
            new(1, 2),   // Arwen
            new(10, 2),  // Bolg (other side of river)
            new(4, 5),   // Wolf
            new(6, 1),   // Bear
            new(0, 0),   // Eagle
            new(11, 7)   // Raven
        };

        for (int i = 0; i < units.Count; i++)
        {
            units[i].InitMap(map, positions[i]);
        }

        var sim = new Simulation(map, units);
        return (sim, "🏞 Wild Lands: All Creatures");
    }

    /// <summary>
    /// Scenario 3: Intense combat arena with close starting positions.
    /// </summary>
    private static (Simulation, string) CreateScenario3()
    {
        var map = new GameMap(10, 10);
        
        // Add diverse terrain for demo
        for (int x = 0; x < 10; x++) map.SetTerrain(5, x, TerrainType.River); // River in middle
        for (int y = 0; y < 10; y++) map.SetTerrain(y, 5, TerrainType.Forest); // Forest cross
        
        var units = new List<IMovable>
        {
            new Bird { Name = "Fast Eagle" },
            new Animal { Name = "Quick Rabbit" },
            new Orc { Name = "Raid Orc" },
            new Elf { Name = "Rich Elf" },
            new Bird { Name = "Scared Ostrich" }
        };

        // Strategic positions: 
        // Eagle near Rabbit, Orc near Elf, Orc near Ostrich
        var positions = new List<Point>
        {
            new(1, 1),  // Eagle
            new(2, 1),  // Rabbit (Eagle target)
            new(4, 4),  // Orc
            new(3, 4),  // Elf (Orc target)
            new(3, 3)   // Ostrich (Orc target/Panic)
        };

        for (int i = 0; i < units.Count; i++)
        {
            units[i].InitMap(map, positions[i]);
            if (units[i] is Elf e) e.CarrotsCarried = 5; // Give elf some carrots for raiding
        }

        var sim = new Simulation(map, units);
        return (sim, "🎭 Advanced Effects Demo");
    }
}
