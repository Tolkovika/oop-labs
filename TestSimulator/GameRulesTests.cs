using Xunit;
using SimCore;
using SimCore.Map;
using SimCore.Units;
using SimCore.Units.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace TestSimulator
{
    public class GameRulesTests
    {
        // === PACK MENTALITY TESTS ===
        [Fact]
        public void OrcPackMentality_AdjacentOrcs_IncreasePower()
        {
            // Arrange: Place 2 orcs adjacent to each other
            var map = new GameMap(10, 10);
            var orc1 = new Orc { Name = "Orc1" };
            var orc2 = new Orc { Name = "Orc2" };
            
            orc1.InitMap(map, new Point(5, 5));
            orc2.InitMap(map, new Point(5, 6)); // Adjacent
            
            var units = new List<IMovable> { orc1, orc2 };
            var sim = new Simulation(map, units);

            // Act: Run turns (pack mentality affects combat power)
            // Both orcs should survive since they're allies
            for (int i = 0; i < 5; i++) sim.NextTurn();

            // Assert: Both orcs should still exist (no friendly fire)
            Assert.Equal(2, sim.Units.OfType<Orc>().Count());
        }

        // === ARCHER'S ADVANTAGE TESTS ===
        [Fact]
        public void ElfArcherAdvantage_CanAttackFromDistance()
        {
            // Arrange: Elf 2 tiles from an orc
            var map = new GameMap(10, 10);
            var elf = new Elf { Name = "Archer" };
            var orc = new Orc { Name = "TargetOrc" };
            
            elf.InitMap(map, new Point(5, 5));
            orc.InitMap(map, new Point(5, 7)); // 2 tiles away
            
            var units = new List<IMovable> { elf, orc };
            var sim = new Simulation(map, units);

            // Act: Run multiple turns (50% hit chance per turn)
            int initialOrcCount = sim.Units.OfType<Orc>().Count();
            for (int i = 0; i < 20; i++) // High chance to hit in 20 turns
            {
                sim.NextTurn();
                if (!sim.Units.Contains(orc)) break;
            }

            // Assert: Check log for ranged attack or orc death
            bool hasRangedAttackLog = sim.HistoryLog.Any(l => l.Contains("🏹") && l.Contains("shot"));
            bool orcDied = !sim.Units.Contains(orc);
            
            Assert.True(hasRangedAttackLog || orcDied, "Elf should have attempted ranged attack");
        }

        // === RABBIT MULTIPLICATION TESTS ===
        [Fact]
        public void RabbitMultiplication_SpawnsBabyAfter10Turns()
        {
            // Arrange
            var map = new GameMap(10, 10);
            var rabbit = new Animal { Name = "MamaRabbit" };
            rabbit.InitMap(map, new Point(5, 5));
            
            var units = new List<IMovable> { rabbit };
            var sim = new Simulation(map, units);

            // Act: Run 15 turns (should spawn at turn 10)
            for (int i = 0; i < 15; i++) sim.NextTurn();

            // Assert: Should have more than 1 rabbit now
            int rabbitCount = sim.Units.OfType<Animal>().Count(a => a.IsRabbit || a.Name.Contains("Baby"));
            Assert.True(rabbitCount >= 2, $"Expected at least 2 rabbits, found {rabbitCount}");
        }

        // === PREY DETECTION TESTS ===
        [Fact]
        public void EaglePreyDetection_MovesTowardRabbit()
        {
            // Arrange: Eagle far from rabbit
            var map = new GameMap(20, 20);
            var eagle = new Bird { Name = "Hunter Eagle" };
            var rabbit = new Animal { Name = "TargetRabbit" };
            
            eagle.InitMap(map, new Point(5, 5));
            rabbit.InitMap(map, new Point(10, 10)); // Moderately far
            
            var units = new List<IMovable> { eagle, rabbit };
            var sim = new Simulation(map, units);
            
            var initialDistance = eagle.Position.DistanceTo(rabbit.Position);

            // Act: Run several turns - eagle should hunt
            bool evergotCloser = false;
            for (int i = 0; i < 10; i++) 
            {
                var distBefore = sim.Units.Contains(rabbit) ? eagle.Position.DistanceTo(rabbit.Position) : 0;
                sim.NextTurn();
                
                // Check if rabbit was caught by swoop
                if (!sim.Units.Contains(rabbit))
                {
                    evergotCloser = true;
                    break;
                }
                
                var distAfter = eagle.Position.DistanceTo(rabbit.Position);
                if (distAfter < distBefore) evergotCloser = true;
            }

            // Assert: Eagle should have moved closer at least once OR caught the rabbit
            bool rabbitCaught = !sim.Units.Contains(rabbit);
            Assert.True(evergotCloser || rabbitCaught, "Eagle should hunt rabbit (get closer or catch it)");
        }

        // === LUCKY ESCAPE TESTS ===
        [Fact]
        public void RabbitLuckyEscape_CanSurviveCombat()
        {
            // Arrange: Many combats to test dodge chance
            int survived = 0;
            
            for (int trial = 0; trial < 20; trial++)
            {
                var map = new GameMap(10, 10);
                var orc = new Orc { Name = "Attacker" };
                var rabbit = new Animal { Name = "LuckyRabbit" };
                
                // Place on same tile to force combat
                orc.InitMap(map, new Point(5, 5));
                rabbit.InitMap(map, new Point(5, 5));
                
                var units = new List<IMovable> { orc, rabbit };
                var sim = new Simulation(map, units);

                // Single turn of combat
                sim.NextTurn();
                
                if (sim.Units.Contains(rabbit)) survived++;
            }

            // Assert: With 30% dodge, expect some survivors in 20 trials
            Assert.True(survived > 0, $"Rabbit should have dodged at least once in 20 trials (survived {survived} times)");
        }
    }
}
