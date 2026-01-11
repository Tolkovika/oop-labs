using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimWeb.Services;
using Simulator;
using Simulator.Maps;

namespace SimWeb.Pages;

public record EventLogEntry(int Turn, string Message);

public class SimulationModel : PageModel
{
    private readonly GameService _gameService;

    public SimulationModel(GameService gameService)
    {
        _gameService = gameService;
    }

    public Simulation? Simulation => _gameService.Simulation;

    [BindProperty]
    public int Turn { get; set; } = 1;
    
    public List<EventLogEntry> EventLog { get; set; } = new();

    public IActionResult OnGet(bool reset = false)
    {
        // Reset simulation if requested or not initialized
        if (reset || Simulation == null || Simulation.Finished)
        {
            _gameService.Reset();
            HttpContext.Session.SetInt32("Turn", 1);
            HttpContext.Session.Remove("EventLog");
            Turn = 1;
            EventLog.Clear();
        }
        else
        {
            Turn = HttpContext.Session.GetInt32("Turn") ?? 1;
            LoadEventLog();
        }

        return Page();
    }

    public IActionResult OnPostNext()
    {
        if (Simulation != null && !Simulation.Finished)
        {
            var currentMappable = Simulation.CurrentMappable.ToString();
            var direction = GetDirectionName(Simulation.CurrentMoveName);
            
            Simulation.Turn();
            
            Turn = (HttpContext.Session.GetInt32("Turn") ?? 1) + 1;
            HttpContext.Session.SetInt32("Turn", Turn);
            
            // Add to event log
            AddEventLogEntry(Turn - 1, $"{currentMappable} → {direction}");
        }

        return RedirectToPage();
    }
    
    public IActionResult OnPostReset()
    {
        return RedirectToPage(new { reset = true });
    }
    
    private void LoadEventLog()
    {
        var logJson = HttpContext.Session.GetString("EventLog");
        if (!string.IsNullOrEmpty(logJson))
        {
            EventLog = System.Text.Json.JsonSerializer.Deserialize<List<EventLogEntry>>(logJson) ?? new();
        }
    }
    
    private void AddEventLogEntry(int turn, string message)
    {
        LoadEventLog();
        EventLog.Add(new EventLogEntry(turn, message));
        
        // Keep last 20 entries
        if (EventLog.Count > 20)
            EventLog.RemoveAt(0);
            
        HttpContext.Session.SetString("EventLog", 
            System.Text.Json.JsonSerializer.Serialize(EventLog));
    }
    
    public string GetImageSrc(IMappable mappable)
    {
        var name = mappable.ToString() ?? "";
        if (mappable is Orc) return "/images/orc.png";
        if (mappable is Elf) return "/images/elf.png";
        if (name.Contains("Rabbit")) return "/images/rabbit.png";
        if (name.Contains("Eagle")) return "/images/eagle.png";
        if (name.Contains("Ostrich")) return "/images/ostrich.png";
        
        return "";
    }
    
    public bool CanFly(IMappable mappable)
    {
        // Flying birds (like Eagles) can fly over water
        if (mappable is Birds bird)
        {
            return bird.CanFly;
        }
        return false;
    }
    
    public string GetDirectionName(string move)
    {
        return move.ToLower() switch
        {
            "u" => "Góra",
            "d" => "Dół",
            "l" => "Lewo",
            "r" => "Prawo",
            _ => move
        };
    }
    
    public string GetTerrainClass(int x, int y, int sizeX, int sizeY)
    {
        // River down the middle (column 4)
        if (x == 4) return "terrain-water";
        
        // Forest in corners
        if ((x == 0 && y == sizeY - 1) || (x == 0 && y == sizeY - 2) ||
            (x == 1 && y == sizeY - 1) ||
            (x == sizeX - 1 && y == 0) || (x == sizeX - 2 && y == 0) ||
            (x == sizeX - 1 && y == 1))
            return "terrain-forest";
        
        // Mountains on right edge
        if (x == sizeX - 1 && y >= sizeY - 3)
            return "terrain-mountain";
        
        // Sand near water
        if (x == 3 || x == 5)
            return "terrain-sand";
        
        // Default grass
        return "terrain-grass";
    }
}
