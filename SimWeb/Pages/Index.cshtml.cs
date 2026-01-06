using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Simulator;
using Simulator.Maps;
using SimWeb.Services;

namespace SimWeb.Pages;

public class IndexModel : PageModel
{
    private readonly SimulationService _simulationService;

    public Simulation? Simulation => _simulationService.Simulation;
    public int CurrentSimType => _simulationService.CurrentSimType;
    
    public string CurrentMove { get; private set; } = "";
    public string CurrentMappable { get; private set; } = "";

    public IndexModel(SimulationService simulationService)
    {
        _simulationService = simulationService;
    }

    public void OnGet()
    {
        if (Simulation != null)
        {
            UpdateState();
        }
    }

    public IActionResult OnPostSim1()
    {
        _simulationService.SetupSim1();
        return RedirectToPage();
    }

    public IActionResult OnPostSim2()
    {
        _simulationService.SetupSim2();
        return RedirectToPage();
    }

    public IActionResult OnPostNext()
    {
        _simulationService.NextTurn();
        return RedirectToPage();
    }

    public IActionResult OnPostReset()
    {
        _simulationService.Reset();
        return RedirectToPage();
    }
    
    private void UpdateState()
    {
        if (Simulation == null) return;
        
        if (Simulation.Finished)
        {
            CurrentMove = "Finished";
            CurrentMappable = "-";
        }
        else
        {
            CurrentMove = Simulation.CurrentMoveName;
            CurrentMappable = Simulation.CurrentMappable.ToString() ?? "";
        }
    }
    
    public string GetEmojiForMappable(IMappable mappable)
    {
        if (mappable is Elf) return "🧝";
        if (mappable is Orc) return "👹";
        if (mappable is Birds b) return b.CanFly ? "🦅" : "🪶";
        if (mappable is Animals) return "🐰";
        return "❓";
    }
}
