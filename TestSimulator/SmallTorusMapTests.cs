using Simulator;
using Simulator.Maps;

namespace TestSimulator;

public class SmallTorusMapTests
{
    [Fact]
    public void Constructor_ValidSize_ShouldSetSize()
    {
        // Arrange
        int size = 10;
        // Act
        var map = new SmallTorusMap(size);
        // Assert
        Assert.Equal(size, map.SizeX);
        Assert.Equal(size, map.SizeY);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(21)]
    public void Constructor_InvalidSize_ShouldThrowArgumentOutOfRangeException(int size)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new SmallTorusMap(size));
    }

    [Theory]
    [InlineData(3, 4, 5, true)]
    [InlineData(6, 1, 5, false)]
    [InlineData(19, 19, 20, true)]
    [InlineData(20, 20, 20, false)]
    public void Exist_ShouldReturnCorrectValue(int x, int y, int size, bool expected)
    {
        // Arrange
        var map = new SmallTorusMap(size);
        var point = new Point(x, y);
        // Act
        var result = map.Exist(point);
        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(5, 10, Direction.Up, 5, 9)]    // Up: Y-1, wraps from 0→19
    [InlineData(5, 0, Direction.Up, 5, 19)]    // Up at Y=0 wraps to Y=19
    [InlineData(5, 10, Direction.Down, 5, 11)] // Down: Y+1
    [InlineData(5, 19, Direction.Down, 5, 0)]  // Down at Y=19 wraps to Y=0
    [InlineData(0, 8, Direction.Left, 19, 8)]  // Left: X-1, wraps from 0→19
    [InlineData(19, 10, Direction.Right, 0, 10)] // Right: X+1, wraps from 19→0
    public void Next_ShouldReturnCorrectNextPoint(int x, int y,
        Direction direction, int expectedX, int expectedY)
    {
        // Arrange
        var map = new SmallTorusMap(20);
        var point = new Point(x, y);
        // Act
        var nextPoint = map.Next(point, direction);
        // Assert
        Assert.Equal(new Point(expectedX, expectedY), nextPoint);
    }

    [Theory]
    [InlineData(5, 10, Direction.Up, 6, 9)]     // Up diagonal: X+1, Y-1
    [InlineData(5, 0, Direction.Up, 6, 19)]     // Up diagonal at Y=0 wraps Y to 19
    [InlineData(5, 10, Direction.Down, 4, 11)]  // Down diagonal: X-1, Y+1
    [InlineData(0, 8, Direction.Left, 19, 7)]   // Left diagonal: X-1 (wraps), Y-1
    [InlineData(19, 10, Direction.Right, 0, 11)] // Right diagonal: X+1 (wraps), Y+1
    public void NextDiagonal_ShouldReturnCorrectNextPoint(int x, int y,
        Direction direction, int expectedX, int expectedY)
    {
        // Arrange
        var map = new SmallTorusMap(20);
        var point = new Point(x, y);
        // Act
        var nextPoint = map.NextDiagonal(point, direction);
        // Assert
        Assert.Equal(new Point(expectedX, expectedY), nextPoint);
    }
}
