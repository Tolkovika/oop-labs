using SimCore.Units;
using SimCore.Units.Interfaces;
using SimCore.Map;
using SimCore.Terrain;

namespace SimCore.Combat;

/// <summary>
/// Results of a combat encounter.
/// </summary>
public enum CombatResult
{
    AttackerWins,    // Defender dies
    DefenderWins,    // Attacker dies
    AttackerFlees,   // Attacker escapes (pushed back)
    DefenderFlees,   // Defender escapes (pushed back)
    Stalemate        // Both survive, pushed apart
}

/// <summary>
/// Event data for combat results to be logged.
/// </summary>
public record CombatEvent(
    Unit Attacker,
    Unit Defender,
    int AttackerPower,
    int DefenderPower,
    CombatResult Result,
    string TerrainBonus,
    string TimeBonus
);

/// <summary>
/// Enhanced combat resolver with terrain modifiers, day/night effects,
/// and multiple outcome types (death, escape, pushback).
/// </summary>
public class CombatResolver
{
    private readonly Random _random = new();
    
    /// <summary>
    /// Last combat events for logging purposes.
    /// </summary>
    public List<CombatEvent> LastCombatEvents { get; } = [];

    public void Resolve(Tile tile, TimeOfDay time, GameMap map)
    {
        LastCombatEvents.Clear();
        
        if (tile.Units.Count < 2) return;

        // Get hostile pairs (different types)
        var units = tile.Units.OfType<Unit>().ToList();
        
        // Process all hostile unit pairs
        for (int i = 0; i < units.Count - 1; i++)
        {
            for (int j = i + 1; j < units.Count; j++)
            {
                var unitA = units[i];
                var unitB = units[j];
                
                // Skip friendly units (same type)
                if (unitA.GetType() == unitB.GetType()) continue;
                
                // Skip if either was already removed
                if (!tile.Units.Contains(unitA) || !tile.Units.Contains(unitB)) continue;
                
                ResolveSingleCombat(unitA, unitB, tile, time, map);
            }
        }
    }
    
    // Overload for backward compatibility
    public void Resolve(Tile tile, TimeOfDay time)
    {
        Resolve(tile, time, null!);
    }
    
    private void ResolveSingleCombat(Unit unitA, Unit unitB, Tile tile, TimeOfDay time, GameMap map)
    {
        // Calculate power for both units
        var (powerA, terrainBonusA, timeBonusA) = CalculatePowerDetailed(unitA, tile.Terrain, time);
        var (powerB, terrainBonusB, timeBonusB) = CalculatePowerDetailed(unitB, tile.Terrain, time);
        
        // Determine attacker (higher initiative or random)
        bool aIsAttacker = powerA >= powerB;
        Unit attacker = aIsAttacker ? unitA : unitB;
        Unit defender = aIsAttacker ? unitB : unitA;
        int attackerPower = aIsAttacker ? powerA : powerB;
        int defenderPower = aIsAttacker ? powerB : powerA;
        
        // Calculate combat result
        int powerDiff = attackerPower - defenderPower;
        CombatResult result = DetermineResult(powerDiff, defender, tile.Terrain);
        
        // Apply result
        ApplyResult(result, attacker, defender, tile, map);
        
        // Log combat event
        LastCombatEvents.Add(new CombatEvent(
            attacker, defender, attackerPower, defenderPower, result,
            aIsAttacker ? terrainBonusA : terrainBonusB,
            aIsAttacker ? timeBonusA : timeBonusB
        ));
    }
    
    private CombatResult DetermineResult(int powerDiff, Unit defender, TerrainType terrain)
    {
        // Clear victory (power difference >= 3)
        if (powerDiff >= 3) return CombatResult.AttackerWins;
        if (powerDiff <= -3) return CombatResult.DefenderWins;
        
        // Close combat - escape chance
        // Mountain terrain gives escape bonus
        int escapeChance = terrain == TerrainType.Mountain ? 40 : 20;
        
        if (_random.Next(100) < escapeChance)
        {
            return powerDiff >= 0 ? CombatResult.DefenderFlees : CombatResult.AttackerFlees;
        }
        
        // Otherwise, winner determined by power (with small random factor)
        int roll = _random.Next(-1, 2); // -1, 0, or 1
        if (powerDiff + roll >= 0)
            return CombatResult.AttackerWins;
        else
            return CombatResult.DefenderWins;
    }
    
    private void ApplyResult(CombatResult result, Unit attacker, Unit defender, Tile tile, GameMap map)
    {
        switch (result)
        {
            case CombatResult.AttackerWins:
                tile.Units.Remove(defender);
                break;
                
            case CombatResult.DefenderWins:
                tile.Units.Remove(attacker);
                break;
                
            case CombatResult.AttackerFlees:
            case CombatResult.DefenderFlees:
                var fleeing = result == CombatResult.AttackerFlees ? attacker : defender;
                PushBack(fleeing, tile, map);
                break;
                
            case CombatResult.Stalemate:
                // Both get pushed back
                PushBack(defender, tile, map);
                break;
        }

        // === OSTRICH KICK MECHANIC ===
        // 20% chance for Ostrich to push enemy if they didn't already die/flee
        if (tile.Units.Contains(attacker) && tile.Units.Contains(defender))
        {
            CheckOstrichKick(attacker, defender, tile, map);
            CheckOstrichKick(defender, attacker, tile, map);
        }
    }

    private void CheckOstrichKick(Unit kicker, Unit target, Tile tile, GameMap map)
    {
        if (kicker is Bird b && b.IsOstrich && _random.Next(100) < 20)
        {
            // Kick!
            PushBack(target, tile, map);
        }
    }
    
    private void PushBack(Unit unit, Tile tile, GameMap map)
    {
        if (map == null) return;
        
        // Find an adjacent empty tile to push to
        var directions = new[] { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
        foreach (var dir in directions.OrderBy(_ => _random.Next()))
        {
            var newPos = unit.Position.Next(dir);
            if (map.IsValid(newPos))
            {
                var targetTile = map.At(newPos);
                // Can flee to empty or safe terrain
                if (targetTile.Units.Count == 0 && targetTile.Terrain != TerrainType.River)
                {
                    tile.Units.Remove(unit);
                    map.Place(unit, newPos);
                    return;
                }
            }
        }
        // If no escape route - unit dies
        tile.Units.Remove(unit);
    }

    private (int power, string terrainBonus, string timeBonus) CalculatePowerDetailed(Unit unit, TerrainType terrain, TimeOfDay time)
    {
        int basePower = 5;
        string terrainBonus = "";
        string timeBonus = "";
        
        // === RACE BASE POWER ===
        if (unit is Orc)
        {
            basePower = 7;
        }
        else if (unit is Elf)
        {
            basePower = 6;
        }
        else if (unit is Animal)
        {
            basePower = 4;
        }
        else if (unit is Bird)
        {
            basePower = 3;
        }
        
        // === TERRAIN MODIFIERS ===
        if (terrain == TerrainType.Forest)
        {
            if (unit is Elf)
            {
                basePower += 3;
                terrainBonus = "+3 Forest";
            }
            else
            {
                // Reduced visibility for non-elves
                basePower -= 1;
                terrainBonus = "-1 Forest";
            }
        }
        else if (terrain == TerrainType.Mountain)
        {
            // Higher ground advantage
            basePower += 1;
            terrainBonus = "+1 Mountain";
        }
        else if (terrain == TerrainType.Swamp)
        {
            // Swamp slows everyone
            basePower -= 1;
            terrainBonus = "-1 Swamp";
        }
        
        // === TIME OF DAY MODIFIERS ===
        if (unit is Orc)
        {
            if (time == TimeOfDay.Night)
            {
                basePower += 3;
                timeBonus = "+3 Night";
            }
        }
        else if (unit is Elf)
        {
            if (time == TimeOfDay.Day)
            {
                basePower += 2;
                timeBonus = "+2 Day";
            }
            else
            {
                basePower -= 1;
                timeBonus = "-1 Night";
            }
        }
        
        return (Math.Max(1, basePower), terrainBonus, timeBonus);
    }
}
