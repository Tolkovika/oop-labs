using Simulator;
using Simulator.Maps;

namespace TestSimulator;

/// <summary>
/// Tests for Map base class functionality including size validation,
/// bounds checking, and creature management.
/// Uses SmallSquareMap as a concrete implementation for testing.
/// </summary>
public class MapTests
{
    #region Size and Bounds Tests

    [Fact]
    public void Constructor_ValidSize_ShouldSetSizeProperties()
    {
        // Arrange & Act
        var map = new SmallSquareMap(10);

        // Assert
        Assert.Equal(10, map.SizeX);
        Assert.Equal(10, map.SizeY);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(3)]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_SizeBelowMinimum_ShouldThrowArgumentOutOfRangeException(int size)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new SmallSquareMap(size));
    }

    #endregion

    #region Creature Management - Add Tests

    [Fact]
    public void Add_CreatureToValidPosition_ShouldSucceed()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Legolas", 5);
        var position = new Point(5, 5);

        // Act
        map.Add(elf, position);
        var creatures = map.At(position);

        // Assert
        Assert.Single(creatures);
        Assert.Equal(elf, creatures[0]);
    }

    [Fact]
    public void Add_MultipleCreatures_ToSamePosition_ShouldSucceed()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Legolas", 5);
        var orc = new Orc("Azog", 7);
        var position = new Point(5, 5);

        // Act
        map.Add(elf, position);
        map.Add(orc, position);
        var creatures = map.At(position);

        // Assert
        Assert.Equal(2, creatures.Count);
        Assert.Contains(elf, creatures);
        Assert.Contains(orc, creatures);
    }

    [Fact]
    public void Add_CreatureToInvalidPosition_ShouldThrowArgumentException()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Legolas", 5);
        var invalidPosition = new Point(-1, 5);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => map.Add(elf, invalidPosition));
        Assert.Contains("outside map bounds", ex.Message);
    }

    #endregion

    #region Creature Management - Remove Tests

    [Fact]
    public void Remove_ExistingCreature_ShouldReturnTrue()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Legolas", 5);
        var position = new Point(5, 5);
        map.Add(elf, position);

        // Act
        bool removed = map.Remove(elf, position);

        // Assert
        Assert.True(removed);
        Assert.Empty(map.At(position));
    }

    [Fact]
    public void Remove_NonExistentCreature_ShouldReturnFalse()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Legolas", 5);
        var position = new Point(5, 5);

        // Act
        bool removed = map.Remove(elf, position);

        // Assert
        Assert.False(removed);
    }

    [Fact]
    public void Remove_OneOfMultipleCreatures_ShouldRemoveOnlyThatCreature()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Legolas", 5);
        var orc = new Orc("Azog", 7);
        var position = new Point(5, 5);
        map.Add(elf, position);
        map.Add(orc, position);

        // Act
        map.Remove(elf, position);
        var creatures = map.At(position);

        // Assert
        Assert.Single(creatures);
        Assert.Equal(orc, creatures[0]);
    }

    #endregion

    #region Creature Management - Move Tests

    [Fact]
    public void Move_CreatureToValidPosition_ShouldSucceed()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Legolas", 5);
        var oldPos = new Point(5, 5);
        var newPos = new Point(6, 6);
        map.Add(elf, oldPos);

        // Act
        map.Move(elf, oldPos, newPos);

        // Assert
        Assert.Empty(map.At(oldPos));
        Assert.Single(map.At(newPos));
        Assert.Equal(elf, map.At(newPos)[0]);
    }

    [Fact]
    public void Move_CreatureNotAtOldPosition_ShouldThrowArgumentException()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Legolas", 5);
        var wrongOldPos = new Point(5, 5);
        var newPos = new Point(6, 6);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => map.Move(elf, wrongOldPos, newPos));
        Assert.Contains("not found at position", ex.Message);
    }

    [Fact]
    public void Move_CreatureToInvalidPosition_ShouldThrowArgumentException()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Legolas", 5);
        var oldPos = new Point(5, 5);
        var invalidNewPos = new Point(15, 15);
        map.Add(elf, oldPos);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => map.Move(elf, oldPos, invalidNewPos));
        Assert.Contains("outside map bounds", ex.Message);
        // Verify creature wasn't removed from old position
        Assert.Single(map.At(oldPos));
    }

    #endregion

    #region Creature Management - At Tests

    [Fact]
    public void At_EmptyPosition_ShouldReturnEmptyList()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var position = new Point(5, 5);

        // Act
        var creatures = map.At(position);

        // Assert
        Assert.Empty(creatures);
    }

    [Fact]
    public void At_WithXY_ShouldReturnSameAsAtWithPoint()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Legolas", 5);
        map.Add(elf, new Point(5, 7));

        // Act
        var creaturesFromPoint = map.At(new Point(5, 7));
        var creaturesFromXY = map.At(5, 7);

        // Assert
        Assert.Equal(creaturesFromPoint.Count, creaturesFromXY.Count);
        Assert.Equal(creaturesFromPoint[0], creaturesFromXY[0]);
    }

    [Fact]
    public void At_ShouldReturnReadOnlyList()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var elf = new Elf("Legolas", 5);
        var position = new Point(5, 5);
        map.Add(elf, position);

        // Act
        var creatures = map.At(position);

        // Assert
        Assert.IsAssignableFrom<IReadOnlyList<Creature>>(creatures);
        // Verify it's truly read-only - should not be castable to List<Creature>
        Assert.False(creatures is List<Creature>);
    }

    #endregion

    #region Rectangular Map Tests (using SmallTorusMap)

    [Fact]
    public void RectangularMap_ShouldHaveDifferentSizeXAndSizeY()
    {
        // Arrange & Act
        var map = new SmallTorusMap(10, 15);

        // Assert
        Assert.Equal(10, map.SizeX);
        Assert.Equal(15, map.SizeY);
    }

    [Fact]
    public void RectangularMap_Exist_ShouldRespectBothDimensions()
    {
        // Arrange
        var map = new SmallTorusMap(10, 15);

        // Act & Assert
        Assert.True(map.Exist(new Point(9, 14))); // Max valid coords
        Assert.False(map.Exist(new Point(10, 14))); // X out of bounds
        Assert.False(map.Exist(new Point(9, 15))); // Y out of bounds
    }

    #endregion
}
