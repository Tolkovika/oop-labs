using SimCore;

namespace SimConsole;

/// <summary>
/// Main game engine handling game loop, input, and state management.
/// </summary>
public class GameEngine
{
    private readonly GameRenderer _renderer;
    private readonly EventLog _eventLog;
    
    private Simulation _simulation = null!;
    private string _scenarioName = "";
    private int _currentScenario = 1;
    private bool _running = true;
    private bool _autoPlay = false;
    private int _previousLogCount = 0;

    private const int AUTO_PLAY_DELAY_MS = 400;

    public GameEngine()
    {
        _renderer = new GameRenderer();
        _eventLog = new EventLog(10);
    }

    /// <summary>
    /// Starts the game engine.
    /// </summary>
    public async Task RunAsync()
    {
        Console.CursorVisible = false;
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        try
        {
            await ShowTitleScreenAsync();
            
            if (!_running) return;
            
            LoadScenario(_currentScenario);
            
            while (_running)
            {
                Render();
                await HandleInputAsync();
            }
        }
        finally
        {
            Console.CursorVisible = true;
            Console.ResetColor();
            Console.Clear();
        }
    }

    private async Task ShowTitleScreenAsync()
    {
        _renderer.RenderTitleScreen();
        
        while (true)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);
                
                switch (key.KeyChar)
                {
                    case '1':
                        _currentScenario = 1;
                        return;
                    case '2':
                        _currentScenario = 2;
                        return;
                    case '3':
                        _currentScenario = 3;
                        return;
                    case 'q':
                    case 'Q':
                        _running = false;
                        return;
                }
            }
            
            await Task.Delay(50);
        }
    }

    private void LoadScenario(int number)
    {
        _currentScenario = number;
        (_simulation, _scenarioName) = ScenarioFactory.CreateScenario(number);
        _eventLog.Clear();
        _eventLog.Add($"🎮 Loaded: {_scenarioName}");
        _eventLog.Add("Press [N] for next turn, [Space] for auto-play");
        _previousLogCount = _simulation.HistoryLog.Count;
    }

    private void Render()
    {
        _renderer.Render(_simulation, _scenarioName, _eventLog, _currentScenario);
    }

    private async Task HandleInputAsync()
    {
        if (_autoPlay)
        {
            // Auto-play mode: advance turn and check for key to stop
            await Task.Delay(AUTO_PLAY_DELAY_MS);
            
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Spacebar || key.KeyChar == ' ')
                {
                    _autoPlay = false;
                    _eventLog.Add("⏸ Auto-play stopped");
                    return;
                }
                ProcessKey(key);
            }
            else
            {
                AdvanceTurn();
            }
        }
        else
        {
            // Manual mode: wait for key
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);
                ProcessKey(key);
            }
            else
            {
                await Task.Delay(50); // Small delay to prevent CPU spin
            }
        }
    }

    private void ProcessKey(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.N:
                AdvanceTurn();
                break;
                
            case ConsoleKey.Spacebar:
                _autoPlay = !_autoPlay;
                _eventLog.Add(_autoPlay ? "▶ Auto-play started" : "⏸ Auto-play stopped");
                break;
                
            case ConsoleKey.R:
                LoadScenario(_currentScenario);
                _eventLog.Add("🔄 Simulation reset");
                break;
                
            case ConsoleKey.Q:
                _running = false;
                break;
                
            case ConsoleKey.D1:
            case ConsoleKey.NumPad1:
                if (_currentScenario != 1)
                {
                    LoadScenario(1);
                }
                break;
                
            case ConsoleKey.D2:
            case ConsoleKey.NumPad2:
                if (_currentScenario != 2)
                {
                    LoadScenario(2);
                }
                break;
                
            case ConsoleKey.D3:
            case ConsoleKey.NumPad3:
                if (_currentScenario != 3)
                {
                    LoadScenario(3);
                }
                break;
        }
    }

    private void AdvanceTurn()
    {
        if (_simulation.Units.Count == 0)
        {
            _eventLog.Add("⚠ No units left! Press [R] to reset.");
            _autoPlay = false;
            return;
        }

        var previousTime = _simulation.CurrentTime;
        _simulation.NextTurn();
        
        // Check for day/night change
        if (_simulation.CurrentTime != previousTime)
        {
            string icon = _simulation.CurrentTime == TimeOfDay.Day ? "☀" : "☽";
            string message = _simulation.CurrentTime == TimeOfDay.Day 
                ? "Dawn breaks... Elves grow stronger!" 
                : "Night falls... Orcs grow stronger!";
            _eventLog.Add($"{icon} {message}");
        }

        // Sync new events from simulation history
        while (_previousLogCount < _simulation.HistoryLog.Count)
        {
            _eventLog.Add(_simulation.HistoryLog[_previousLogCount]);
            _previousLogCount++;
        }

        // Check for game over
        int elves = _simulation.Units.Count(u => u.GetType().Name == "Elf");
        int orcs = _simulation.Units.Count(u => u.GetType().Name == "Orc");
        
        if (_simulation.Units.Count > 0 && (elves == 0 || orcs == 0))
        {
            if (elves == 0 && orcs > 0)
            {
                _eventLog.Add("🏆 ORCS WIN! All elves have fallen!");
                _autoPlay = false;
            }
            else if (orcs == 0 && elves > 0)
            {
                _eventLog.Add("🏆 ELVES WIN! All orcs have fallen!");
                _autoPlay = false;
            }
        }
    }
}
