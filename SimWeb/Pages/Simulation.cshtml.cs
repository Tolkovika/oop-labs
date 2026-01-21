using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimWeb.Services;
using Simulator;
using Simulator.Maps;

namespace SimWeb.Pages;

public class SimulationModel : PageModel
{
    private readonly GameService _gameService;

    public SimulationModel(GameService gameService)
    {
        _gameService = gameService;
    }

    public Simulation? Simulation => _gameService.Simulation;
    public int Turn => _gameService.CurrentTurn;
    public List<string> EventLog => _gameService.EventLog;
    public bool IsDay => Turn % 2 == 0;

    public IActionResult OnGet(bool reset = false)
    {
        if (reset)
        {
            _gameService.Reset();
        }
        else
        {
            _gameService.EnsureInitialized();
        }

        return Page();
    }

    public IActionResult OnPostNext()
    {
        _gameService.EnsureInitialized();
        _gameService.NextTurn();
        return RedirectToPage();
    }
    
    public IActionResult OnPostReset()
    {
        return RedirectToPage(new { reset = true });
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
