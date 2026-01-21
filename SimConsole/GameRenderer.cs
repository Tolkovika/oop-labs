using SimCore;
using SimCore.Map;
using SimCore.Terrain;

namespace SimConsole;

/// <summary>
/// Renders the game UI to console using ASCII characters.
/// Uses in-place rendering without scrolling.
/// </summary>
public class GameRenderer
{
    private const int MAP_START_Y = 3;
    private const int LOG_HEIGHT = 10;
    
    public int Width { get; }
    public int Height { get; }

    public GameRenderer(int width = 80, int height = 28)
    {
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Renders the complete game screen.
    /// </summary>
    public void Render(Simulation sim, string scenarioName, EventLog eventLog, int currentScenario)
    {
        Console.SetCursorPosition(0, 0);
        
        RenderHUD(sim, scenarioName);
        RenderBonusBar(sim);
        RenderMap(sim);
        RenderEventLog(eventLog);
        RenderControls();
    }

    private void RenderHUD(Simulation sim, string scenarioName)
    {
        string timeIcon = sim.CurrentTime == TimeOfDay.Day ? "☀" : "☽";
        string timeName = sim.CurrentTime == TimeOfDay.Day ? "DAY  " : "NIGHT";
        ConsoleColor timeColor = sim.CurrentTime == TimeOfDay.Day ? ConsoleColor.Yellow : ConsoleColor.DarkBlue;
        
        // Count units by type
        int elves = sim.Units.Count(u => u.GetType().Name == "Elf");
        int orcs = sim.Units.Count(u => u.GetType().Name == "Orc");
        int animals = sim.Units.Count(u => u.GetType().Name == "Animal");
        int birds = sim.Units.Count(u => u.GetType().Name == "Bird");
        
        // Get Elf carrot count
        var elf = sim.Units.OfType<SimCore.Units.Elf>().FirstOrDefault();
        int elfCarrots = elf?.CarrotsCarried ?? 0;

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"╔══════════════════════════════════════════════════════════════════════════════╗");
        
        Console.Write("║ ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"{scenarioName,-28}");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(" │ ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"Turn: {sim.TurnIndex,-3}");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(" │ ");
        Console.ForegroundColor = timeColor;
        Console.Write($"{timeIcon} {timeName}");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(" │ ");
        
        // Unit counts with colors
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"E:{elves} ");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"O:{orcs} ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"A:{animals} ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write($"B:{birds} ");
        
        // Elf carrot inventory
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write($" 🥕:{elfCarrots}");
        
        // Advanced resources counts
        int totalFeathers = MapSum(sim.Map, t => t.Feathers);
        int totalFootprints = MapSum(sim.Map, t => t.Footprints);
        int totalDust = MapSum(sim.Map, t => t.Dust);

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($" 🪶:{totalFeathers}");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($" 👣:{totalFootprints}");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write($" 🌫️:{totalDust}");
        
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  ║");
        Console.ResetColor();
    }
    
    private void RenderBonusBar(Simulation sim)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("╠");
        Console.Write(new string('─', 78));
        Console.WriteLine("╣");
        
        // Show active bonuses
        Console.Write("║ ");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("ACTIVE: ");
        
        if (sim.CurrentTime == TimeOfDay.Night)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Orcs +3 ATK ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("Elves -1 ATK ");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Elves +2 ATK ");
        }
        
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("│ ");
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.Write("^Forest: Elves +3 ");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("#Mountain: Escape+ ");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write("~River: Death! ");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write("🌫️Dust: -1 DMG ");
        
        // Pad to end
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(new string(' ', 2));
        Console.WriteLine("║");
        
        Console.Write("╠");
        Console.Write(new string('═', 78));
        Console.WriteLine("╣");
        Console.ResetColor();
    }

    private void RenderMap(Simulation sim)
    {
        var map = sim.Map;
        var elf = sim.Units.OfType<SimCore.Units.Elf>().FirstOrDefault();
        
        // Map area
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("║  ");
        Console.Write(Box.TopLeft);
        Console.Write(new string(Box.Horizontal, map.SizeX));
        Console.Write(Box.TopRight);
        
        // Legend header
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("  LEGEND");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(new string(' ', Width - map.SizeX - 15));
        Console.WriteLine("║");

        // Map rows with legend
        string[] legendLines = {
            "  E = Elf (green)",
            "  O = Orc (red)",
            "  A = Animal",
            "  B = Bird",
            "  ─────────────",
            "  🥕 = Carrots",
            "  🪶 = Feathers",
            "  🌫️ = Dust (Slow)",
            "  👣 = Footprints",
            "  ─────────────",
            "  . = Plain",
            "  ^ = Forest",
            "  # = Mountain"
        };
        
        for (int y = 0; y < map.SizeY; y++)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("║  ");
            Console.Write(Box.Vertical);
            
            for (int x = 0; x < map.SizeX; x++)
            {
                var tile = map.At(x, y);
                RenderTile(tile);
            }
            
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(Box.Vertical);
            
            // Legend text on the right
            if (y < legendLines.Length)
            {
                string legendLine = legendLines[y];
                // Show elf carrot count on a legend line
                if (legendLine.Contains("Elf") && elf != null)
                {
                    legendLine = $"  E = Elf ({elf.CarrotsCarried}🥕)";
                }
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(legendLine.PadRight(Width - map.SizeX - 7));
            }
            else
            {
                Console.Write(new string(' ', Width - map.SizeX - 7));
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("║");
        }

        Console.Write("║  ");
        Console.Write(Box.BottomLeft);
        Console.Write(new string(Box.Horizontal, map.SizeX));
        Console.Write(Box.BottomRight);
        Console.Write(new string(' ', Width - map.SizeX - 7));
        Console.WriteLine("║");
        Console.ResetColor();
    }

    private void RenderTile(Tile tile)
    {
        if (tile.Units.Count > 0)
        {
            // Render unit (highest priority)
            var unit = tile.Units[0];
            ConsoleColor color = unit.GetType().Name switch
            {
                "Elf" => ConsoleColor.Green,
                "Orc" => ConsoleColor.Red,
                "Animal" => ConsoleColor.Yellow,
                "Bird" => ConsoleColor.Cyan,
                _ => ConsoleColor.White
            };
            
            if (tile.Units.Count > 1)
            {
                // Multiple units - COMBAT!
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write('⚔');
            }
            else
            {
                Console.ForegroundColor = color;
                Console.Write(unit.Symbol);
            }
            Console.ResetColor();
        }
        else
        {
            // Check for carrots on this tile
            if (tile.Carrots > 0)
            {
                // Show carrot with count
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write(tile.Carrots > 9 ? 'C' : (char)('0' + tile.Carrots)); // Show count or C for 10+
                Console.ResetColor();
            }
            else
            {
                // Render terrain
                (char symbol, ConsoleColor color) = tile.Terrain switch
                {
                    TerrainType.Plain => ('.', ConsoleColor.DarkGray),
                    TerrainType.Forest => ('^', ConsoleColor.DarkGreen),
                    TerrainType.Mountain => ('#', ConsoleColor.Gray),
                    TerrainType.River => ('~', ConsoleColor.Blue),
                    TerrainType.Swamp => ('%', ConsoleColor.DarkYellow),
                    _ => ('.', ConsoleColor.DarkGray)
                };
                
                Console.ForegroundColor = color;
                Console.Write(symbol);
                Console.ResetColor();
            }
        }
    }

    private void RenderEventLog(EventLog log)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╠══════════════════════════════════════════════════════════════════════════════╣");
        
        Console.Write("║ ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("⚔ EVENT LOG");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(new string(' ', Width - 14) + "║");
        Console.ResetColor();
        
        var events = log.GetRecent(LOG_HEIGHT - 2);
        foreach (var evt in events)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("║ ");
            
            // Color based on event type
            if (evt.Contains("⚔") || evt.Contains("slays"))
                Console.ForegroundColor = ConsoleColor.Red;
            else if (evt.Contains("💀") || evt.Contains("drowned"))
                Console.ForegroundColor = ConsoleColor.DarkRed;
            else if (evt.Contains("🏃") || evt.Contains("flees") || evt.Contains("escapes"))
                Console.ForegroundColor = ConsoleColor.Yellow;
            else if (evt.Contains("☀") || evt.Contains("DAY"))
                Console.ForegroundColor = ConsoleColor.Yellow;
            else if (evt.Contains("☽") || evt.Contains("NIGHT"))
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
            else if (evt.Contains("🏆"))
                Console.ForegroundColor = ConsoleColor.Green;
            else
                Console.ForegroundColor = ConsoleColor.Gray;
            
            string truncated = evt.Length > Width - 4 ? evt[..(Width - 7)] + "..." : evt;
            Console.Write(truncated.PadRight(Width - 4));
            
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("║");
        }
        
        // Pad remaining event lines
        for (int i = events.Count; i < LOG_HEIGHT - 2; i++)
        {
            Console.Write("║");
            Console.Write(new string(' ', Width - 2));
            Console.WriteLine("║");
        }
        
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
    }

    private void RenderControls()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        string controls = "    [N]ext Turn  [Space]Auto  [R]eset  [1-3]Scenario  [Q]uit";
        Console.WriteLine(controls);
        Console.ResetColor();
    }

    /// <summary>
    /// Renders title screen.
    /// </summary>
    public void RenderTitleScreen()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        
        string[] title = {
            @"",
            @"  ╔═══════════════════════════════════════════════════════════════════════╗",
            @"  ║                                                                       ║",
            @"  ║   ███████╗██╗███╗   ███╗██╗   ██╗██╗      █████╗ ████████╗ ██████╗██████╗  ║",
            @"  ║   ██╔════╝██║████╗ ████║██║   ██║██║     ██╔══██╗╚══██╔══╝██╔═══██╗██╔══██╗ ║",
            @"  ║   ███████╗██║██╔████╔██║██║   ██║██║     ███████║   ██║   ██║   ██║██████╔╝ ║",
            @"  ║   ╚════██║██║██║╚██╔╝██║██║   ██║██║     ██╔══██║   ██║   ██║   ██║██╔══██╗ ║",
            @"  ║   ███████║██║██║ ╚═╝ ██║╚██████╔╝███████╗██║  ██║   ██║   ╚██████╔╝██║  ██║ ║",
            @"  ║   ╚══════╝╚═╝╚═╝     ╚═╝ ╚═════╝ ╚══════╝╚═╝  ╚═╝   ╚═╝    ╚═════╝ ╚═╝  ╚═╝ ║",
            @"  ║                                                                       ║",
            @"  ║                    ⚔  ASCII BATTLE ARENA  ⚔                          ║",
            @"  ║                                                                       ║",
            @"  ╠═══════════════════════════════════════════════════════════════════════╣",
            @"  ║                                                                       ║",
            @"  ║   RULES:                                                              ║",
            @"  ║   • River (~) = Death for non-flyers!                                 ║",
            @"  ║   • Forest (^) = Elves get +3 combat bonus                            ║",
            @"  ║   • Mountain (#) = Escape bonus in combat                             ║",
            @"  ║   • Night = Orcs stronger | Day = Elves stronger                      ║",
            @"  ║                                                                       ║",
            @"  ╠═══════════════════════════════════════════════════════════════════════╣",
            @"  ║                                                                       ║",
            @"  ║    [1] ⚔ Battle Arena: Elves vs Orcs                                  ║",
            @"  ║    [2] 🏞 Wild Lands: All Creatures                                   ║",
            @"  ║    [3] 🎭 Advanced Effects Demo                                       ║",
            @"  ║                                                                       ║",
            @"  ║    [Q] Quit                                                           ║",
            @"  ║                                                                       ║",
            @"  ╚═══════════════════════════════════════════════════════════════════════╝",
            @""
        };

        foreach (var line in title)
        {
            Console.WriteLine(line);
        }
        
        Console.ResetColor();
        Console.WriteLine();
        Console.Write("  Select scenario (1-3) or Q to quit: ");
    }
    
    /// <summary>
    /// Displays a brief combat animation.
    /// </summary>
    public void ShowCombatAnimation(int x, int y)
    {
        string[] frames = { "⚔", "✧", "💥", "✧", "⚔" };
        int screenX = x + 3 + 2; // map offset
        int screenY = y + 5;     // HUD + bonus bar offset
        
        foreach (var frame in frames)
        {
            Console.SetCursorPosition(screenX, screenY);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(frame);
            Console.ResetColor();
            Thread.Sleep(50);
        }
    }
    private int MapSum(GameMap map, Func<Tile, int> selector)
    {
        int sum = 0;
        for (int x = 0; x < map.SizeX; x++)
            for (int y = 0; y < map.SizeY; y++)
                sum += selector(map.At(x, y));
        return sum;
    }
}
