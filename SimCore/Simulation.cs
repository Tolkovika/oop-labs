using SimCore.Combat;
using SimCore.Map;
using SimCore.Terrain;
using SimCore.Units;
using SimCore.Units.Interfaces;

namespace SimCore;

/// <summary>
/// Main simulation engine handling turns, movement, terrain effects, and combat.
/// </summary>
public class Simulation
{
    public GameMap Map { get; }
    public List<IMovable> Units { get; }
    public int TurnIndex { get; private set; } = 0;
    public TimeOfDay CurrentTime { get; private set; } = TimeOfDay.Day;
    public List<string> HistoryLog { get; } = [];
    
    /// <summary>
    /// Day/Night cycle period in turns.
    /// </summary>
    public int DayNightCycleTurns { get; } = 5;

    private readonly CombatResolver _combatResolver = new();
    private readonly Random _random = new();

    public Simulation(GameMap map, List<IMovable> units)
    {
        Map = map;
        Units = units;

        // Place units on map
        foreach (var unit in Units)
        {
            if (Map.IsValid(unit.Position))
            {
                Map.Place(unit, unit.Position);
            }
        }
        
        HistoryLog.Add("⚔ Simulation started!");
    }

    public void NextTurn()
    {
        TurnIndex++;
        
        // Cycle day/night every N turns
        if (TurnIndex % DayNightCycleTurns == 0)
        {
            CurrentTime = CurrentTime == TimeOfDay.Day ? TimeOfDay.Night : TimeOfDay.Day;
            string icon = CurrentTime == TimeOfDay.Day ? "☀" : "☽";
            HistoryLog.Add($"{icon} T{TurnIndex}: {CurrentTime} begins!");
        }

        // Move all units and apply terrain effects
        var unitsToRemove = new List<IMovable>();
        
        foreach (var unit in Units.ToList())
        {
            if (unitsToRemove.Contains(unit)) continue;
            
            // AI: Choose random direction
            var direction = (Direction)_random.Next(4);
            var oldPos = unit.Position;

            // === OSTRICH PANIC LOGIC ===
            // Occurs before move
            if (unit is Bird bird && bird.IsOstrich)
            {
                var orcNear = Units.OfType<Orc>().Any(o => o.Position.DistanceTo(unit.Position) <= 2);
                if (orcNear)
                {
                    // Panic sprint: attempt 2-step diagonal movement
                    var sprintDir = (Direction)_random.Next(4);
                    // Point.NextDiagonal followed by another Diagonal step
                    var sprintPos = oldPos.Next(sprintDir).Next(sprintDir); // Fast movement
                    
                    if (Map.IsValid(sprintPos))
                    {
                        Map.Move(unit, oldPos, sprintPos);
                        if (unit is Unit u) u.SetPosition(sprintPos);
                        Map.At(oldPos).Dust += 3; // Leave dust behind
                        HistoryLog.Add($"🌫️ T{TurnIndex}: {unit.Name} panicked and sprinted to ({sprintPos.X},{sprintPos.Y})! Dust cloud created.");
                        continue; // Skip regular move
                    }
                }
            }

            var newPos = oldPos.Next(direction);
            
            // Check if can move there
            if (Map.IsValid(newPos))
            {
                var tile = Map.At(newPos);
                var terrainResult = CheckTerrainEntry(unit, tile.Terrain);
                bool moved = false;
                
                if (terrainResult == TerrainResult.CanEnter)
                {
                    // Move normally
                    Map.Move(unit, oldPos, newPos);
                    if (unit is Unit u) u.SetPosition(newPos);
                    moved = true;
                }
                else if (terrainResult == TerrainResult.Death)
                {
                    // Unit dies trying to enter terrain
                    Map.Remove(unit, oldPos);
                    unitsToRemove.Add(unit);
                    HistoryLog.Add($"💀 T{TurnIndex}: {unit} drowned in the river!");
                }
                else if (terrainResult == TerrainResult.Slowed)
                {
                    // Enters but loses turn (no further action)
                    Map.Move(unit, oldPos, newPos);
                    if (unit is Unit u) u.SetPosition(newPos);
                    moved = true;
                    // Could add "slowed" status effect here
                }
                // TerrainResult.Blocked - stays in place
                
                // === CARROT DROP LOGIC ===
                // After rabbit moves, it drops a carrot on the tile it JUST LEFT
                if (moved && unit is Animal animal && animal.IsRabbit && animal.Carrots > 0)
                {
                    var previousTile = Map.At(oldPos);
                    previousTile.Carrots++;
                    animal.Carrots--;
                    HistoryLog.Add($"🥕 T{TurnIndex}: {animal.Name} left a carrot at ({oldPos.X},{oldPos.Y})");
                }
                
                // === CARROT PICKUP LOGIC ===
                // After elf moves, it picks up all carrots on its tile
                if (moved && unit is Elf elf)
                {
                    var currentTile = Map.At(elf.Position);
                    if (currentTile.Carrots > 0)
                    {
                        int picked = currentTile.Carrots;
                        elf.CarrotsCarried += picked;
                        currentTile.Carrots = 0;
                        HistoryLog.Add($"🥕 T{TurnIndex}: {elf.Name} picked up {picked} carrot(s)! (Total: {elf.CarrotsCarried})");
                    }
                }

                // === EAGLE SWOOP LOGIC ===
                if (moved && unit is Bird eagle && eagle.IsEagle)
                {
                    // Search for rabbits in radius 2
                    var caught = Units.OfType<Animal>()
                        .FirstOrDefault(a => a.IsRabbit && a.Position.DistanceTo(unit.Position) <= 2);
                    
                    if (caught != null)
                    {
                        Map.Remove(caught, caught.Position);
                        unitsToRemove.Add(caught);
                        
                        var eagleTile = Map.At(unit.Position);
                        eagleTile.Feathers++;
                        HistoryLog.Add($"🦅 T{TurnIndex}: {unit.Name} swooped and caught {caught.Name}! Feather dropped at ({unit.Position.X},{unit.Position.Y})");
                    }
                }

                // === ORC RAID LOGIC ===
                if (moved && unit is Orc orc)
                {
                    // Drop footprints
                    Map.At(unit.Position).Footprints++;

                    // Search for adjacent elf
                    var targetElf = Units.OfType<Elf>()
                        .FirstOrDefault(e => e.Position.IsAdjacent(unit.Position));
                    
                    if (targetElf != null && targetElf.CarrotsCarried > 0)
                    {
                        int raidChance = CurrentTime == TimeOfDay.Night ? 70 : 30;
                        if (_random.Next(100) < raidChance)
                        {
                            int stolen = 1 + _random.Next(targetElf.CarrotsCarried / 2 + 1);
                            targetElf.CarrotsCarried -= stolen;
                            HistoryLog.Add($"👹 T{TurnIndex}: {unit.Name} raided {targetElf.Name} and stole {stolen} carrot(s)! (Night bonus: {CurrentTime == TimeOfDay.Night})");
                        }
                    }
                }
            }
        }
        
        // Remove dead units
        foreach (var dead in unitsToRemove)
        {
            Units.Remove(dead);
        }

        // Resolve combat on all tiles
        for (int x = 0; x < Map.SizeX; x++)
        {
            for (int y = 0; y < Map.SizeY; y++)
            {
                var tile = Map.At(x, y);
                if (tile.Units.Count > 1)
                {
                    _combatResolver.Resolve(tile, CurrentTime, Map);
                    
                    // Log combat events (including possible pushes/drownings)
                    foreach (var evt in _combatResolver.LastCombatEvents)
                    {
                        LogCombatEvent(evt);
                    }
                }
            }
        }

        // Cleanup dead units (including those drowned by Ostrich kick)
        Units.RemoveAll(u => 
        {
            var tile = Map.At(u.Position);
            var onMap = tile.Units.Contains(u);
            return !onMap;
        });
    }
    
    private void LogCombatEvent(CombatEvent evt)
    {
        string resultText = evt.Result switch
        {
            CombatResult.AttackerWins => $"⚔ {evt.Attacker.Name} slays {evt.Defender.Name}!",
            CombatResult.DefenderWins => $"⚔ {evt.Defender.Name} slays {evt.Attacker.Name}!",
            CombatResult.AttackerFlees => $"🏃 {evt.Attacker.Name} flees from {evt.Defender.Name}!",
            CombatResult.DefenderFlees => $"🏃 {evt.Defender.Name} escapes from {evt.Attacker.Name}!",
            CombatResult.Stalemate => $"⚔ {evt.Attacker.Name} and {evt.Defender.Name} clash inconclusively!",
            _ => $"⚔ Combat between {evt.Attacker.Name} and {evt.Defender.Name}"
        };
        
        // Add power breakdown
        string powerInfo = $"({evt.AttackerPower} vs {evt.DefenderPower})";
        
        // Add modifiers if present
        var modifiers = new List<string>();
        if (!string.IsNullOrEmpty(evt.TerrainBonus)) modifiers.Add(evt.TerrainBonus);
        if (!string.IsNullOrEmpty(evt.TimeBonus)) modifiers.Add(evt.TimeBonus);
        
        string modifierText = modifiers.Count > 0 ? $" [{string.Join(", ", modifiers)}]" : "";
        
        HistoryLog.Add($"T{TurnIndex}: {resultText} {powerInfo}{modifierText}");
    }
    
    private TerrainResult CheckTerrainEntry(IMovable unit, TerrainType terrain)
    {
        // === RIVER RULES ===
        if (terrain == TerrainType.River)
        {
            // Flyable: no effect (can fly over)
            if (unit is IFlyable) return TerrainResult.CanEnter;
            
            // Swimmable: can enter but slowed
            if (unit is ISwimmable) return TerrainResult.Slowed;
            
            // Others: instant death (drowning)
            return TerrainResult.Death;
        }
        
        // === MOUNTAIN RULES ===
        if (terrain == TerrainType.Mountain)
        {
            // Birds can fly over
            if (unit is IFlyable) return TerrainResult.CanEnter;
            
            // Others: slowed (takes effort to climb)
            return TerrainResult.Slowed;
        }
        
        // === SWAMP RULES ===
        if (terrain == TerrainType.Swamp)
        {
            // Birds can fly over
            if (unit is IFlyable) return TerrainResult.CanEnter;
            
            // Others: slowed
            return TerrainResult.Slowed;
        }

        // === DUST EFFECT ===
        if (Map.At(unit.Position).Dust > 0) // Check if current or target has dust? 
        {
            // Usually dust affects where you ARE or where you are going.
            // Let's say if you are in dust, you move slower.
            if (!(unit is IFlyable)) return TerrainResult.Slowed;
        }
        
        // Forest, Plain - normal entry
        return TerrainResult.CanEnter;
    }
}

/// <summary>
/// Result of attempting to enter terrain.
/// </summary>
public enum TerrainResult
{
    CanEnter,   // Normal movement
    Slowed,     // Can enter but penalty
    Blocked,    // Cannot enter
    Death       // Instant death
}
