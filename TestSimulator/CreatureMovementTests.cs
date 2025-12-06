using Simulator;
using Simulator.Maps;

namespace TestSimulator;

/// <summary>
/// Tests for creature movement integration with maps.
/// Verifies that creatures can be placed on maps, move correctly,
/// and maintain synchronization between creature state and map state.
/// </summary>
public class CreatureMovementTests
{
    #region Initialization Tests

    [Fact]
    public void Creature_WithoutMap_ShouldHaveNullMapAndPosition()
    {
        // Arrange & Act
        var elf = new Elf("Legolas", 5);

        // Assert
        Assert.Null(elf.Map);
        Assert.Null(elf.Position);
    }

    [Fact]
    public void InitMapAndPosition_ValidPosition_ShouldSetMapAndPosition()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Legolas", 5);
        var position = new Point(5, 5);

        // Act
        elf.InitMapAndPosition(map, position);

        // Assert
        Assert.Equal(map, elf.Map);
        Assert.Equal(position, elf.Position);
    }

    [Fact]
    public void InitMapAndPosition_ShouldAddCreatureToMap()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Legolas", 5);
        var position = new Point(5, 5);

        // Act
        elf.InitMapAndPosition(map, position);

        // Assert
        var creatures = map.At(position);
        Assert.Single(creatures);
        Assert.Equal(elf, creatures[0]);
    }

    [Fact]
    public void InitMapAndPosition_InvalidPosition_ShouldThrowArgumentException()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Legolas", 5);
        var invalidPosition = new Point(-1, 5);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => elf.InitMapAndPosition(map, invalidPosition));
        Assert.Contains("invalid for the map", ex.Message);
    }

    [Fact]
    public void InitMapAndPosition_CalledTwice_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Legolas", 5);
        elf.InitMapAndPosition(map, new Point(5, 5));

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(
            () => elf.InitMapAndPosition(map, new Point(6, 6)));
        Assert.Contains("already on a map", ex.Message);
    }

    #endregion

    #region Go() Tests - Without Map

    [Fact]
    public void Go_WithoutMap_ShouldDoNothing()
    {
        // Arrange
        var elf = new Elf("Legolas", 5);

        // Act - should not throw
        elf.Go(Direction.Up);
        elf.Go(Direction.Right);

        // Assert
        Assert.Null(elf.Map);
        Assert.Null(elf.Position);
    }

    #endregion

    #region Go() Tests - SmallSquareMap (Wall Blocking)

    [Fact]
    public void Go_OnSquareMap_InsideBounds_ShouldMoveCreature()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Legolas", 5);
        elf.InitMapAndPosition(map, new Point(5, 5));

        // Act
        elf.Go(Direction.Up);

        // Assert
        Assert.Equal(new Point(5, 4), elf.Position);
        Assert.Empty(map.At(5, 5)); // Old position empty
        Assert.Single(map.At(5, 4)); // New position has creature
    }

    [Fact]
    public void Go_OnSquareMap_AtWall_ShouldStayInPlace()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Legolas", 5);
        elf.InitMapAndPosition(map, new Point(0, 5));

        // Act
        elf.Go(Direction.Left); // Would move to (-1, 5) which is outside

        // Assert
        Assert.Equal(new Point(0, 5), elf.Position); // Should not move
        Assert.Single(map.At(0, 5)); // Still at original position
    }

    [Fact]
    public void Go_OnSquareMap_MultipleMovements_ShouldTrackCorrectly()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Legolas", 5);
        elf.InitMapAndPosition(map, new Point(5, 5));

        // Act
        elf.Go(Direction.Up);    // (5, 4)
        elf.Go(Direction.Right); // (6, 4)
        elf.Go(Direction.Down);  // (6, 5)
        elf.Go(Direction.Left);  // (5, 5) - back to start

        // Assert
        Assert.Equal(new Point(5, 5), elf.Position);
        Assert.Single(map.At(5, 5));
    }

    #endregion

    #region Go() Tests - SmallTorusMap (Wrapping)

    [Fact]
    public void Go_OnTorusMap_InsideBounds_ShouldMoveCreature()
    {
        // Arrange
        var map = new SmallTorusMap(10);
        var elf = new Elf("Legolas", 5);
        elf.InitMapAndPosition(map, new Point(5, 5));

        // Act
        elf.Go(Direction.Up);

        // Assert
        Assert.Equal(new Point(5, 4), elf.Position);
    }

    [Fact]
    public void Go_OnTorusMap_AtEdge_ShouldWrapAround()
    {
        // Arrange
        var map = new SmallTorusMap(10);
        var elf = new Elf("Legolas", 5);
        elf.InitMapAndPosition(map, new Point(0, 5));

        // Act
        elf.Go(Direction.Left); // Should wrap to (9, 5)

        // Assert
        Assert.Equal(new Point(9, 5), elf.Position);
        Assert.Empty(map.At(0, 5)); // Old position empty
        Assert.Single(map.At(9, 5)); // New wrapped position has creature
    }

    [Fact]
    public void Go_OnTorusMap_VerticalWrapping_ShouldWork()
    {
        // Arrange
        var map = new SmallTorusMap(10);
        var elf = new Elf("Legolas", 5);
        elf.InitMapAndPosition(map, new Point(5, 0));

        // Act
        elf.Go(Direction.Up); // Should wrap to (5, 9)

        // Assert
        Assert.Equal(new Point(5, 9), elf.Position);
    }

    [Fact]
    public void Go_OnRectangularTorus_ShouldRespectDimensions()
    {
        // Arrange
        var map = new SmallTorusMap(10, 15); // 10 wide, 15 tall
        var elf = new Elf("Legolas", 5);
        elf.InitMapAndPosition(map, new Point(9, 14));

        // Act
        elf.Go(Direction.Right); // Should wrap X to 0
        elf.Go(Direction.Down);  // Should wrap Y to 0

        // Assert
        Assert.Equal(new Point(0, 0), elf.Position);
    }

    #endregion

    #region Multiple Creatures Tests

    [Fact]
    public void Go_MultipleCreaturesOnSamePoint_ShouldMaintainBoth()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Legolas", 5);
        var orc = new Orc("Azog", 7);
        elf.InitMapAndPosition(map, new Point(5, 5));
        orc.InitMapAndPosition(map, new Point(5, 5));

        // Act - Move elf away
        elf.Go(Direction.Up);

        // Assert
        Assert.Equal(new Point(5, 4), elf.Position);
        Assert.Equal(new Point(5, 5), orc.Position);
        Assert.Single(map.At(5, 4)); // Elf alone at new position
        Assert.Single(map.At(5, 5)); // Orc alone at original position
    }

    [Fact]
    public void Go_MultipleCreaturesMovingToSamePoint_ShouldAllowOverlap()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Legolas", 5);
        var orc = new Orc("Azog", 7);
        elf.InitMapAndPosition(map, new Point(5, 5));
        orc.InitMapAndPosition(map, new Point(6, 5));

        // Act - Move orc to elf's position
        orc.Go(Direction.Left);

        // Assert
        Assert.Equal(new Point(5, 5), elf.Position);
        Assert.Equal(new Point(5, 5), orc.Position);
        var creatures = map.At(5, 5);
        Assert.Equal(2, creatures.Count);
        Assert.Contains(elf, creatures);
        Assert.Contains(orc, creatures);
    }

    #endregion

    #region Synchronization Tests

    [Fact]
    public void Go_ShouldMaintainSynchronizationBetweenCreatureAndMap()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Legolas", 5);
        elf.InitMapAndPosition(map, new Point(5, 5));

        // Act
        elf.Go(Direction.Right);
        elf.Go(Direction.Up);

        // Assert - Creature position matches map
        var finalPos = elf.Position!.Value;
        var creaturesAtFinalPos = map.At(finalPos);
        Assert.Single(creaturesAtFinalPos);
        Assert.Equal(elf, creaturesAtFinalPos[0]);
        
        // Verify no creatures at old positions
        Assert.Empty(map.At(5, 5));
        Assert.Empty(map.At(6, 5));
    }

    [Fact]
    public void Go_WithDifferentCreatureTypes_ShouldWorkCorrectly()
    {
        // Arrange
        var map = new SmallTorusMap(10);
        var elf = new Elf("Legolas", 5, 8);
        var orc = new Orc("Azog", 7, 9);
        
        elf.InitMapAndPosition(map, new Point(0, 0));
        orc.InitMapAndPosition(map, new Point(9, 9));

        // Act
        elf.Go(Direction.Left);  // Wraps to (9, 0)
        orc.Go(Direction.Right); // Wraps to (0, 9)

        // Assert
        Assert.Equal(new Point(9, 0), elf.Position);
        Assert.Equal(new Point(0, 9), orc.Position);
        Assert.Single(map.At(9, 0));
        Assert.Single(map.At(0, 9));
    }

    #endregion
}
