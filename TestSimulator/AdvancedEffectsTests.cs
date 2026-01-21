using Xunit;
using SimCore;
using SimCore.Map;
using SimCore.Units;
using SimCore.Units.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace TestSimulator
{
    public class AdvancedEffectsTests
    {
        [Fact]
        public void Eagle_ShouldSwoopAndCatchRabbit()
        {
            // Arrange
            var map = new GameMap(10, 10);
            var eagle = new Bird { Name = "Eagle" };
            var rabbit = new Animal { Name = "Rabbit" };
            
            // Place in center to guarantee move possibility
            eagle.InitMap(map, new SimCore.Map.Point(5, 5));
            rabbit.InitMap(map, new SimCore.Map.Point(5, 6)); // Adjacent
            
            var units = new List<IMovable> { eagle, rabbit };
            var sim = new Simulation(map, units);

            // Act - Run a few turns if needed to ensure movement
            // Or just check if it caught in 1-2 turns
            for(int i=0; i<5; i++)
            {
                sim.NextTurn();
                if (sim.Units.Count == 1) break;
            }

            // Assert
            Assert.Contains(eagle, sim.Units);
            Assert.DoesNotContain(rabbit, sim.Units);
        }

        [Fact]
        public void Orc_ShouldLeaveFootprints()
        {
            // Arrange
            var map = new GameMap(10, 10);
            var orc = new Orc { Name = "Orc" };
            orc.InitMap(map, new SimCore.Map.Point(5, 5));
            
            var units = new List<IMovable> { orc };
            var sim = new Simulation(map, units);

            // Act
            sim.NextTurn();

            // Assert
            var footprints = 0;
            for(int x=0; x<10; x++)
                for(int y=0; y<10; y++)
                    footprints += map.At(new SimCore.Map.Point(x, y)).Footprints;
            
            Assert.True(footprints > 0);
        }

        [Fact]
        public void Ostrich_ShouldPanicAndLeaveDust()
        {
            // Arrange
            var map = new GameMap(10, 10);
            var orc = new Orc { Name = "Orc" };
            var ostrich = new Bird { Name = "Ostrich" };
            
            ostrich.InitMap(map, new SimCore.Map.Point(5, 5));
            orc.InitMap(map, new SimCore.Map.Point(6, 6)); // Adjacent
            
            var units = new List<IMovable> { ostrich, orc };
            var sim = new Simulation(map, units);

            // Act
            sim.NextTurn();

            // Assert
            var dust = 0;
            for(int x=0; x<10; x++)
                for(int y=0; y<10; y++)
                    dust += map.At(new SimCore.Map.Point(x, y)).Dust;
            
            Assert.True(dust > 0);
        }
    }
}
