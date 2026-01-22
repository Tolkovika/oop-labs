using Simulator.Maps;

namespace Simulator;

/// <summary>
/// Manages simulation of creatures moving on a map.
/// </summary>
public class Simulation
{
    private int _currentMoveIndex = 0;

    /// <summary>
    /// Simulation's map.
    /// </summary>
    public Map Map { get; }

    /// <summary>
    /// Mappable objects moving on the map.
    /// </summary>
    public List<IMappable> Mappables { get; }

    /// <summary>
    /// Starting positions of creatures.
    /// </summary>
    public List<Point> Positions { get; }

    /// <summary>
    /// Cyclic list of creatures moves.
    /// Bad moves are ignored - use DirectionParser.
    /// First move is for first creature, second for second and so on.
    /// When all creatures make moves,
    /// next move is again for first creature and so on.
    /// </summary>
    public string Moves { get; }

    /// <summary>
    /// Has all moves been done?
    /// </summary>
    public bool Finished { get; private set; } = false;

    /// <summary>
    /// Mappable object which will be moving current turn.
    /// Throws InvalidOperationException if simulation finished.
    /// </summary>
    public IMappable CurrentMappable
    {
        get
        {
            if (Finished)
                throw new InvalidOperationException("Simulation has finished");
            
            return Mappables[_currentMoveIndex % Mappables.Count];
        }
    }

    /// <summary>
    /// Lowercase name of direction which will be used in current turn.
    /// Throws InvalidOperationException if simulation finished.
    /// </summary>
    public string CurrentMoveName
    {
        get
        {
            if (Finished)
                throw new InvalidOperationException("Simulation has finished");
            
            return Moves[_currentMoveIndex].ToString().ToLower();
        }
    }

    /// <summary>
    /// Simulation constructor.
    /// Places mappable objects on map at starting positions.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when mappables list is empty or when number of mappables
    /// differs from number of starting positions.
    /// </exception>
    public Simulation(Map map, List<IMappable> mappables, 
                      List<Point> positions, string moves)
    {
        // Validation
        if (mappables == null || mappables.Count == 0)
            throw new ArgumentException(
                "Mappables list cannot be empty", nameof(mappables));
        
        if (positions == null || mappables.Count != positions.Count)
            throw new ArgumentException(
                "Number of mappables must match number of positions");
        
        // Assignment
        Map = map;
        Mappables = mappables;
        Positions = positions;
        Moves = moves ?? "";
        
        // Initialize mappables on map
        for (int i = 0; i < mappables.Count; i++)
        {
            mappables[i].InitMapAndPosition(map, positions[i]);
        }
        
        // Check if no moves - simulation already finished
        if (Moves.Length == 0)
        {
            Finished = true;
        }
    }

    /// <summary>
    /// Makes one move of current creature in current direction.
    /// Invalid moves (unparseable characters) are skipped.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when simulation is already finished.
    /// </exception>
    public void Turn()
    {
        if (Finished)
            throw new InvalidOperationException(
                "Cannot execute turn - simulation has finished");
        
        // Get current move character
        char moveChar = Moves[_currentMoveIndex];
        
        // Parse to Direction (returns empty array for invalid chars)
        Direction[] parsedDirections = DirectionParser.Parse(moveChar.ToString());
        
        // Track position before move
        var oldPosition = CurrentMappable.Position;
        bool movedAI = false;
        
        // === AI BEHAVIORS ===
        var unit = CurrentMappable;

        // === RABBIT MULTIPLICATION ===
        // Every 5 rabbit turns, spawn a baby on adjacent tile
        if (unit is Animals rabbit && rabbit.IsRabbit && rabbit.Position != null)
        {
            rabbit.TurnsSinceSpawn++;
            if (rabbit.TurnsSinceSpawn >= 3) // Accelerated breeding (3 turns)
            {
                rabbit.TurnsSinceSpawn = 0;
                var emptyTile = FindEmptyAdjacentTile(rabbit.Position.Value);
                if (emptyTile != null)
                {
                    var babyRabbit = new Animals { Description = $"BabyRabbit", Size = 3 };
                    babyRabbit.Carrots = 5;
                    babyRabbit.InitMapAndPosition(Map, emptyTile.Value);
                    Mappables.Add(babyRabbit);
                }
            }
        }

        // 1. OSTRICH PANIC
        if (unit is Birds bird && bird.IsOstrich)
        {
            var orcNear = Mappables.OfType<Orc>().Any(o => o.Position != null && o.Position.Value.DistanceTo(unit.Position.Value) <= 2);
            if (orcNear)
            {
                var randomDir = (Direction)(new Random().Next(4));
                var sprintPos = Map.NextDiagonal(unit.Position.Value, randomDir);
                
                if (!sprintPos.Equals(unit.Position.Value))
                {
                    Map.Move(unit, unit.Position.Value, sprintPos);
                    unit.Position = sprintPos;
                    // Map.AddDust(oldPosition.Value, 3);
                    movedAI = true;
                }
            }
        }

        // 2. ELF SEEK CARROTS (Global Search)
        if (!movedAI && unit is Elf seekingElf && unit.Position != null)
        {
             // Find nearest carrot globally
             Point? bestCarrot = null;
             double bestDist = double.MaxValue;

             for (int x = 0; x < Map.SizeX; x++)
             {
                 for (int y = 0; y < Map.SizeY; y++)
                 {
                     var p = new Point(x, y);
                     if (Map.GetCarrots(p) > 0)
                     {
                         // Use simple coordinate distance for seeking
                        int dx = x - unit.Position.Value.X;
                        int dy = y - unit.Position.Value.Y;
                        double d = Math.Sqrt(dx*dx + dy*dy);
                        
                         if (d < bestDist)
                         {
                             bestDist = d;
                             bestCarrot = p;
                         }
                     }
                 }
             }

             if (bestCarrot != null)
             {
                 var dirToCarrot = GetDirectionToward(unit.Position.Value, bestCarrot.Value);
                 var nextPos = Map.Next(unit.Position.Value, dirToCarrot);
                 
                 // Move if feasible
                 if (!nextPos.Equals(unit.Position.Value))
                 {
                     Map.Move(unit, unit.Position.Value, nextPos);
                     unit.Position = nextPos;
                     movedAI = true;
                 }
             }
        }

        // 3. EAGLE HUNT
        if (!movedAI && unit is Birds eagle && eagle.IsEagle && unit.Position != null)
        {
             var nearestRabbit = Mappables.OfType<Animals>()
                .Where(a => a.IsRabbit && a.Position != null)
                .OrderBy(a => a.Position!.Value.DistanceTo(unit.Position.Value))
                .FirstOrDefault();
            
            if (nearestRabbit != null)
            {
                var dirToRabbit = GetDirectionToward(unit.Position.Value, nearestRabbit.Position!.Value);
                var nextPos = Map.Next(unit.Position.Value, dirToRabbit);
                if (!nextPos.Equals(unit.Position.Value))
                {
                    Map.Move(unit, unit.Position.Value, nextPos);
                    unit.Position = nextPos;
                    movedAI = true;
                }
            }
        }

        // === STANDARD MOVE ===
        if (!movedAI && parsedDirections.Length > 0)
        {
            CurrentMappable.Go(parsedDirections[0]);
        }
        
    // PostMove logic
        var currentUnit = CurrentMappable;
        var currentPos = currentUnit.Position;

        // === CARROT DROP LOGIC ===
        if (currentUnit is Animals animal && animal.IsRabbit && animal.Carrots > 0)
        {
            if (currentPos != null && !currentPos.Equals(oldPosition))
            {
                Map.AddCarrots(oldPosition.Value);
                animal.Carrots--;
            }
        }
        
        // === CARROT PICKUP LOGIC ===
        if (currentUnit is Elf elf && currentPos != null)
        {
            int carrots = Map.GetCarrots(currentPos.Value);
            if (carrots > 0)
            {
                elf.CarrotsCarried += carrots;
                Map.SetCarrots(currentPos.Value, 0);
            }
        }

        // === EAGLE SWOOP LOGIC ===
        if (currentUnit is Birds e && e.IsEagle && currentPos != null)
        {
            var victim = Mappables.OfType<Animals>()
                .FirstOrDefault(a => a.IsRabbit && a.Position != null && a.Position.Value.DistanceTo(currentPos.Value) <= 2);
            
            if (victim != null)
            {
                Map.Remove(victim, victim.Position.Value);
                victim.Position = null; // Mark as removed
                // Map.AddFeathers(currentPos.Value);
            }
        }

        // === ORC RAID LOGIC ===
        if (currentUnit is Orc o && currentPos != null)
        {
             // Footprints removed
            
            var target = Mappables.OfType<Elf>()
                .FirstOrDefault(e => e.Position != null && e.Position.Value.IsAdjacent(currentPos.Value));
            
            if (target != null && target.CarrotsCarried > 0)
            {
                if (new Random().Next(100) < 40)
                {
                    target.CarrotsCarried--;
                }
            }
        }
        
        // Advance to next move
        _currentMoveIndex++;
        
        // Check if simulation finished
        if (_currentMoveIndex >= Moves.Length)
        {
            Finished = true;
        }
    }
    
    /// <summary>
    /// Find an empty adjacent tile for spawning new units.
    /// </summary>
    private Point? FindEmptyAdjacentTile(Point origin)
    {
        var directions = new[] { Direction.Up, Direction.Right, Direction.Down, Direction.Left };
        var rand = new Random();
        foreach (var dir in directions.OrderBy(_ => rand.Next()))
        {
            var pos = Map.Next(origin, dir);
            if (!pos.Equals(origin) && Map.At(pos.X, pos.Y).Count == 0)
            {
                return pos;
            }
        }
        return null;
    }
    
    /// <summary>
    /// Get the best direction to move from 'from' toward 'to'.
    /// </summary>
    private Direction GetDirectionToward(Point from, Point to)
    {
        int dx = to.X - from.X;
        int dy = to.Y - from.Y;
        
        if (Math.Abs(dx) >= Math.Abs(dy))
        {
            return dx > 0 ? Direction.Right : Direction.Left;
        }
        else
        {
            return dy > 0 ? Direction.Up : Direction.Down;
        }
    }
}
