using Simulator;
using Simulator.Maps;

namespace SimWeb.Services;

/// <summary>
/// Singleton service managing simulation state across requests.
/// </summary>
public class GameService
{
    private Simulation? _simulation;
    private readonly object _lock = new();
    
    public int CurrentTurn { get; private set; } = 0;
    public List<string> EventLog { get; } = new();
    
    public Simulation? Simulation
    {
        get
        {
            lock (_lock)
            {
                return _simulation;
            }
        }
    }
    
    public bool IsInitialized => _simulation != null;

    public void EnsureInitialized()
    {
        lock (_lock)
        {
            if (_simulation == null)
            {
                Reset();
            }
        }
    }
    
    public void Reset()
    {
        lock (_lock)
        {
            var (map, mappables, points, moves) = SimulationScenarios.GetSim2();
            _simulation = new Simulation(map, mappables, points, moves);
            CurrentTurn = 0;
            EventLog.Clear();
            EventLog.Add("🎮 Symulacja rozpoczęta!");
        }
    }
    
    /// <summary>
    /// Advances simulation by one turn. Returns true if successful, false if finished.
    /// </summary>
    public bool NextTurn()
    {
        lock (_lock)
        {
            if (_simulation == null || _simulation.Finished)
                return false;
            
            try
            {
                var mappable = _simulation.CurrentMappable.ToString();
                var direction = _simulation.CurrentMoveName;
                
                _simulation.Turn();
                CurrentTurn++;
                
                // Log the event
                string dirName = direction switch
                {
                    "u" => "↑ Góra",
                    "d" => "↓ Dół",
                    "l" => "← Lewo",
                    "r" => "→ Prawo",
                    _ => direction
                };
                EventLog.Add($"[T{CurrentTurn}] {mappable} → {dirName}");
                
                // Keep last 20 entries
                while (EventLog.Count > 20)
                    EventLog.RemoveAt(0);
                
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
    
    /// <summary>
    /// Gets current map state as a 2D array of symbol lists for JSON serialization.
    /// </summary>
    public object GetMapState()
    {
        lock (_lock)
        {
            if (_simulation == null)
                return new { error = "Not initialized" };
            
            var map = _simulation.Map;
            var cells = new List<object>();
            
            for (int y = map.SizeY - 1; y >= 0; y--)
            {
                for (int x = 0; x < map.SizeX; x++)
                {
                    var items = map.At(x, y);
                    var unitInfos = items.Select(m => new
                    {
                        symbol = m.Symbol.ToString(),
                        name = m.ToString(),
                        type = m.GetType().Name
                    }).ToList();

                    var feathers = map.GetFeathers(new Point(x, y));
                    var dust = map.GetDust(new Point(x, y));
                    var footprints = map.GetFootprints(new Point(x, y));
                    var carrots = map.GetCarrots(new Point(x, y));
                    
                    cells.Add(new { x, y, units = unitInfos, feathers, dust, footprints, carrots });
                }
            }
            
            return new
            {
                turn = CurrentTurn,
                finished = _simulation.Finished,
                sizeX = map.SizeX,
                sizeY = map.SizeY,
                cells,
                events = EventLog.TakeLast(10).Reverse().ToList(),
                isDay = CurrentTurn % 2 == 0,
                elfCarrots = _simulation.Mappables.OfType<Elf>().FirstOrDefault()?.CarrotsCarried ?? 0,
                rabbitCarrots = _simulation.Mappables.OfType<Animals>().Where(a => a.IsRabbit).Sum(a => a.Carrots)
            };
        }
    }
}
