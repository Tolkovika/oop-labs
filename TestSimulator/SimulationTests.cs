using Simulator;
using Simulator.Maps;

namespace TestSimulator;

public class SimulationTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_EmptyCreaturesList_ShouldThrowArgumentException()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var emptyList = new List<Creature>();
        var positions = new List<Point>();
        
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(
            () => new Simulation(map, emptyList, positions, "udlr"));
        Assert.Contains("empty", ex.Message);
    }

    [Fact]
    public void Constructor_NullCreaturesList_ShouldThrowArgumentException()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var positions = new List<Point> { new Point(5, 5) };
        
        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => new Simulation(map, null!, positions, "udlr"));
    }

    [Fact]
    public void Constructor_MismatchedCreaturesAndPositions_ShouldThrowArgumentException()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var creatures = new List<Creature> 
        { 
            new Elf("Legolas", 1), 
            new Orc("Azog", 1) 
        };
        var positions = new List<Point> { new Point(5, 5) }; // Only 1 position for 2 creatures
        
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(
            () => new Simulation(map, creatures, positions, "udlr"));
        Assert.Contains("match", ex.Message);
    }

    [Fact]
    public void Constructor_ValidInput_ShouldInitializeCreaturesOnMap()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Legolas", 1);
        var orc = new Orc("Azog", 1);
        var creatures = new List<Creature> { elf, orc };
        var positions = new List<Point> { new Point(3, 3), new Point(7, 7) };
        
        // Act
        var sim = new Simulation(map, creatures, positions, "udlr");
        
        // Assert
        Assert.Equal(new Point(3, 3), elf.Position);
        Assert.Equal(new Point(7, 7), orc.Position);
        Assert.Single(map.At(3, 3));
        Assert.Single(map.At(7, 7));
    }

    [Fact]
    public void Constructor_EmptyMoves_ShouldSetFinishedTrue()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var creatures = new List<Creature> { new Elf("Elf", 1) };
        var positions = new List<Point> { new Point(5, 5) };
        
        // Act
        var sim = new Simulation(map, creatures, positions, "");
        
        // Assert
        Assert.True(sim.Finished);
    }

    [Fact]
    public void Constructor_NullMoves_ShouldTreatAsEmpty()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var creatures = new List<Creature> { new Elf("Elf", 1) };
        var positions = new List<Point> { new Point(5, 5) };
        
        // Act
        var sim = new Simulation(map, creatures, positions, null!);
        
        // Assert
        Assert.True(sim.Finished);
        Assert.Equal("", sim.Moves);
    }

    #endregion

    #region CurrentCreature Tests

    [Fact]
    public void CurrentCreature_FirstTurn_ShouldReturnFirstCreature()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Elf", 1);
        var orc = new Orc("Orc", 1);
        var creatures = new List<Creature> { elf, orc };
        var positions = new List<Point> { new Point(5, 5), new Point(6, 6) };
        
        // Act
        var sim = new Simulation(map, creatures, positions, "udlr");
        
        // Assert
        Assert.Equal(elf, sim.CurrentCreature);
    }

    [Fact]
    public void CurrentCreature_CyclicDistribution_ShouldRotate()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var c1 = new Elf("E1", 1);
        var c2 = new Orc("O1", 1);
        var creatures = new List<Creature> { c1, c2 };
        var positions = new List<Point> { new Point(5, 5), new Point(6, 6) };
        
        // Act
        var sim = new Simulation(map, creatures, positions, "udlr");
        
        // Assert
        Assert.Equal(c1, sim.CurrentCreature); // Move 0 -> creature 0
        sim.Turn();
        Assert.Equal(c2, sim.CurrentCreature); // Move 1 -> creature 1
        sim.Turn();
        Assert.Equal(c1, sim.CurrentCreature); // Move 2 -> creature 0 (cyclic)
    }

    [Fact]
    public void CurrentCreature_AfterFinished_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var creatures = new List<Creature> { new Elf("Elf", 1) };
        var positions = new List<Point> { new Point(5, 5) };
        
        var sim = new Simulation(map, creatures, positions, "u");
        sim.Turn(); // Finishes simulation
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => sim.CurrentCreature);
    }

    #endregion

    #region CurrentMoveName Tests

    [Fact]
    public void CurrentMoveName_ShouldReturnLowercaseChar()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var creatures = new List<Creature> { new Elf("Elf", 1) };
        var positions = new List<Point> { new Point(5, 5) };
        
        // Act
        var sim = new Simulation(map, creatures, positions, "UdLr");
        
        // Assert
        Assert.Equal("u", sim.CurrentMoveName);
        sim.Turn();
        Assert.Equal("d", sim.CurrentMoveName);
    }

    [Fact]
    public void CurrentMoveName_AfterFinished_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var creatures = new List<Creature> { new Elf("Elf", 1) };
        var positions = new List<Point> { new Point(5, 5) };
        
        var sim = new Simulation(map, creatures, positions, "u");
        sim.Turn();
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => sim.CurrentMoveName);
    }

    #endregion

    #region Turn Tests - Valid Moves

    [Fact]
    public void Turn_ValidMove_ShouldMoveCreature()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Elf", 1);
        var creatures = new List<Creature> { elf };
        var positions = new List<Point> { new Point(5, 5) };
        
        var sim = new Simulation(map, creatures, positions, "u");
        
        // Act
        sim.Turn();
        
        // Assert
        Assert.Equal(new Point(5, 4), elf.Position); // Moved up
        Assert.True(sim.Finished);
    }

    [Fact]
    public void Turn_MultipleValidMoves_ShouldExecuteInOrder()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Elf", 1);
        var creatures = new List<Creature> { elf };
        var positions = new List<Point> { new Point(5, 5) };
        
        var sim = new Simulation(map, creatures, positions, "urdl");
        
        // Act & Assert
        sim.Turn(); // u: (5,5) -> (5,4)
        Assert.Equal(new Point(5, 4), elf.Position);
        
        sim.Turn(); // r: (5,4) -> (6,4)
        Assert.Equal(new Point(6, 4), elf.Position);
        
        sim.Turn(); // d: (6,4) -> (6,5)
        Assert.Equal(new Point(6, 5), elf.Position);
        
        sim.Turn(); // l: (6,5) -> (5,5)
        Assert.Equal(new Point(5, 5), elf.Position);
        Assert.True(sim.Finished);
    }

    #endregion

    #region Turn Tests - Invalid Moves

    [Fact]
    public void Turn_InvalidMove_ShouldSkipAndAdvance()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Elf", 1);
        var creatures = new List<Creature> { elf };
        var positions = new List<Point> { new Point(5, 5) };
        
        var sim = new Simulation(map, creatures, positions, "xur"); // 'x' is invalid
        
        // Act
        sim.Turn(); // Skip 'x'
        
        // Assert
        Assert.Equal(new Point(5, 5), elf.Position); // No movement
        Assert.False(sim.Finished);
        
        sim.Turn(); // 'u' - should move
        Assert.Equal(new Point(5, 4), elf.Position);
    }

    [Fact]
    public void Turn_AllInvalidMoves_ShouldSkipAllAndFinish()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Elf", 1);
        var creatures = new List<Creature> { elf };
        var positions = new List<Point> { new Point(5, 5) };
        
        var sim = new Simulation(map, creatures, positions, "xyz123");
        
        // Act
        while (!sim.Finished)
        {
            sim.Turn();
        }
        
        // Assert
        Assert.Equal(new Point(5, 5), elf.Position); // Never moved
        Assert.True(sim.Finished);
    }

    [Fact]
    public void Turn_MixedValidInvalidMoves_ShouldExecuteOnlyValid()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Elf", 1);
        var creatures = new List<Creature> { elf };
        var positions = new List<Point> { new Point(5, 5) };
        
        var sim = new Simulation(map, creatures, positions, "u5xr3l");
        
        // Act & Assert
        sim.Turn(); // u: move
        Assert.Equal(new Point(5, 4), elf.Position);
        
        sim.Turn(); // 5: skip
        Assert.Equal(new Point(5, 4), elf.Position);
        
        sim.Turn(); // x: skip
        Assert.Equal(new Point(5, 4), elf.Position);
        
        sim.Turn(); // r: move
        Assert.Equal(new Point(6, 4), elf.Position);
        
        sim.Turn(); // 3: skip
        Assert.Equal(new Point(6, 4), elf.Position);
        
        sim.Turn(); // l: move
        Assert.Equal(new Point(5, 4), elf.Position);
        Assert.True(sim.Finished);
    }

    #endregion

    #region Turn Tests - Finished State

    [Fact]
    public void Turn_AfterFinished_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var creatures = new List<Creature> { new Elf("Elf", 1) };
        var positions = new List<Point> { new Point(5, 5) };
        
        var sim = new Simulation(map, creatures, positions, "u");
        sim.Turn();
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => sim.Turn());
    }

    [Fact]
    public void Turn_LastMove_ShouldSetFinishedTrue()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var creatures = new List<Creature> { new Elf("Elf", 1) };
        var positions = new List<Point> { new Point(5, 5) };
        
        var sim = new Simulation(map, creatures, positions, "ud");
        
        // Act
        Assert.False(sim.Finished);
        sim.Turn();
        Assert.False(sim.Finished);
        sim.Turn();
        
        // Assert
        Assert.True(sim.Finished);
    }

    #endregion

    #region Turn Tests - Multiple Creatures

    [Fact]
    public void Turn_MultipleCreatures_ShouldDistributeMovesCyclically()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Elf", 1);
        var orc = new Orc("Orc", 1);
        var creatures = new List<Creature> { elf, orc };
        var positions = new List<Point> { new Point(5, 5), new Point(6, 6) };
        
        var sim = new Simulation(map, creatures, positions, "udlr");
        
        // Act & Assert
        // Turn 0: elf moves up
        sim.Turn();
        Assert.Equal(new Point(5, 4), elf.Position);
        Assert.Equal(new Point(6, 6), orc.Position); // unchanged
        
        // Turn 1: orc moves down
        sim.Turn();
        Assert.Equal(new Point(5, 4), elf.Position); // unchanged
        Assert.Equal(new Point(6, 7), orc.Position);
        
        // Turn 2: elf moves left
        sim.Turn();
        Assert.Equal(new Point(4, 4), elf.Position);
        Assert.Equal(new Point(6, 7), orc.Position); // unchanged
        
        // Turn 3: orc moves right
        sim.Turn();
        Assert.Equal(new Point(4, 4), elf.Position); // unchanged
        Assert.Equal(new Point(7, 7), orc.Position);
        Assert.True(sim.Finished);
    }

    [Fact]
    public void Turn_ThreeCreatures_ShouldCycleCorrectly()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var c1 = new Elf("E1", 1);
        var c2 = new Orc("O1", 1);
        var c3 = new Elf("E2", 1);
        var creatures = new List<Creature> { c1, c2, c3 };
        var positions = new List<Point> 
        { 
            new Point(3, 3), 
            new Point(5, 5), 
            new Point(7, 7) 
        };
        
        var sim = new Simulation(map, creatures, positions, "uuuuuu"); // 6 moves, 2 per creature
        
        // Act & Assert
        sim.Turn(); // c1
        Assert.Equal(new Point(3, 2), c1.Position);
        
        sim.Turn(); // c2
        Assert.Equal(new Point(5, 4), c2.Position);
        
        sim.Turn(); // c3
        Assert.Equal(new Point(7, 6), c3.Position);
        
        sim.Turn(); // c1 again (cyclic)
        Assert.Equal(new Point(3, 1), c1.Position);
        
        sim.Turn(); // c2 again
        Assert.Equal(new Point(5, 3), c2.Position);
        
        sim.Turn(); // c3 again
        Assert.Equal(new Point(7, 5), c3.Position);
        
        Assert.True(sim.Finished);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Simulation_CompleteRun_ShouldWorkCorrectly()
    {
        // Arrange
        var map = new SmallTorusMap(5);
        var elf = new Elf("Elf", 1);
        var creatures = new List<Creature> { elf };
        var positions = new List<Point> { new Point(0, 0) };
        
        // Move left 3 times (wrapping torus)
        var sim = new Simulation(map, creatures, positions, "lll");
        
        // Act & Assert
        Assert.False(sim.Finished);
        
        sim.Turn(); // (0,0) -> (4,0) wrapped
        Assert.Equal(new Point(4, 0), elf.Position);
        
        sim.Turn(); // (4,0) -> (3,0)
        Assert.Equal(new Point(3, 0), elf.Position);
        
        sim.Turn(); // (3,0) -> (2,0)
        Assert.Equal(new Point(2, 0), elf.Position);
        Assert.True(sim.Finished);
    }

    [Fact]
    public void Simulation_WallBlocking_ShouldNotMove()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Elf", 1);
        var creatures = new List<Creature> { elf };
        var positions = new List<Point> { new Point(0, 0) }; // Corner
        
        var sim = new Simulation(map, creatures, positions, "ulu"); // Try to go left+up
        
        // Act & Assert
        sim.Turn(); // u: blocked at wall (0,0) stays
        Assert.Equal(new Point(0, 0), elf.Position);
        
        sim.Turn(); // l: blocked at wall (0,0) stays
        Assert.Equal(new Point(0, 0), elf.Position);
        
        sim.Turn(); // u: blocked at wall (0,0) stays
        Assert.Equal(new Point(0, 0), elf.Position);
        
        Assert.True(sim.Finished);
    }

    #endregion
}
