using Simulator;
using Simulator.Maps;

namespace SimWeb.Services;

public class GameService
{
    public Simulation? Simulation { get; private set; }
    
    private bool _initialized = false;

    public void EnsureInitialized()
    {
        if (!_initialized || Simulation == null || Simulation.Finished)
        {
            Reset();
        }
    }
    
    public void Reset()
    {
        var (map, mappables, points, moves) = SimulationScenarios.GetSim2();
        Simulation = new Simulation(map, mappables, points, moves);
        _initialized = true;
    }
}
