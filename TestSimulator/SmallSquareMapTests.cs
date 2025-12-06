using Simulator;
using Simulator.Maps;

namespace TestSimulator;

public class SmallSquareMapTests
{
    #region Constructor Tests - Valid Sizes

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(15)]
    [InlineData(20)]
    public void Constructor_WithValidSize_ShouldCreateMap(int size)
    {
        // Arrange & Act
        var map = new SmallSquareMap(size);

        // Assert
        Assert.Equal(size, map.SizeX);
        Assert.Equal(size, map.SizeY);
    }

    #endregion

    #region Constructor Tests - Invalid Sizes

    [Theory]
    [InlineData(4)]
    [InlineData(3)]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithSizeBelowMin_ShouldThrowArgumentOutOfRangeException(int size)
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new SmallSquareMap(size));
    }

    [Theory]
    [InlineData(21)]
    [InlineData(25)]
    [InlineData(100)]
    public void Constructor_WithSizeAboveMax_ShouldThrowArgumentOutOfRangeException(int size)
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new SmallSquareMap(size));
    }

    #endregion

    #region Exist Tests - Inside Map

    [Fact]
    public void Exist_PointInsideMap_ShouldReturnTrue()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var point = new Point(5, 5);

        // Act
        var result = map.Exist(point);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(5, 5)]
    [InlineData(9, 9)]
    [InlineData(0, 9)]
    [InlineData(9, 0)]
    public void Exist_VariousPointsInside_ShouldReturnTrue(int x, int y)
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var point = new Point(x, y);

        // Act
        var result = map.Exist(point);

        // Assert
        Assert.True(result);
    }

    #endregion

    #region Exist Tests - On Boundaries

    [Fact]
    public void Exist_PointAtOrigin_ShouldReturnTrue()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var point = new Point(0, 0);

        // Act
        var result = map.Exist(point);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Exist_PointAtMaxCorner_ShouldReturnTrue()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var point = new Point(9, 9); // Size-1, Size-1

        // Act
        var result = map.Exist(point);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(0, 5)]  // Left edge
    [InlineData(9, 5)]  // Right edge
    [InlineData(5, 0)]  // Top edge
    [InlineData(5, 9)]  // Bottom edge
    public void Exist_PointOnEdge_ShouldReturnTrue(int x, int y)
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var point = new Point(x, y);

        // Act
        var result = map.Exist(point);

        // Assert
        Assert.True(result);
    }

    #endregion

    #region Exist Tests - Outside Map

    [Theory]
    [InlineData(-1, 0)]   // Left of map
    [InlineData(0, -1)]   // Above map
    [InlineData(10, 5)]   // Right of map (Size == 10)
    [InlineData(5, 10)]   // Below map
    [InlineData(-1, -1)]  // Outside corner
    [InlineData(10, 10)]  // Outside corner
    [InlineData(15, 15)]  // Far outside
    public void Exist_PointOutsideMap_ShouldReturnFalse(int x, int y)
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var point = new Point(x, y);

        // Act
        var result = map.Exist(point);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region Next Tests - Movement Inside Map

    [Fact]
    public void Next_InsideMap_ShouldMoveUp()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var point = new Point(5, 5);

        // Act
        var result = map.Next(point, Direction.Up);

        // Assert
        Assert.Equal(new Point(5, 4), result);
    }

    [Fact]
    public void Next_InsideMap_ShouldMoveDown()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var point = new Point(5, 5);

        // Act
        var result = map.Next(point, Direction.Down);

        // Assert
        Assert.Equal(new Point(5, 6), result);
    }

    [Fact]
    public void Next_InsideMap_ShouldMoveLeft()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var point = new Point(5, 5);

        // Act
        var result = map.Next(point, Direction.Left);

        // Assert
        Assert.Equal(new Point(4, 5), result);
    }

    [Fact]
    public void Next_InsideMap_ShouldMoveRight()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var point = new Point(5, 5);

        // Act
        var result = map.Next(point, Direction.Right);

        // Assert
        Assert.Equal(new Point(6, 5), result);
    }

    #endregion

    #region Next Tests - At Walls (Should Return Same Point)

    [Fact]
    public void Next_AtTopWall_MovingUp_ShouldReturnSamePoint()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var point = new Point(5, 0);

        // Act
        var result = map.Next(point, Direction.Up);

        // Assert
        Assert.Equal(point, result);
    }

    [Fact]
    public void Next_AtBottomWall_MovingDown_ShouldReturnSamePoint()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var point = new Point(5, 9); // Size-1

        // Act
        var result = map.Next(point, Direction.Down);

        // Assert
        Assert.Equal(point, result);
    }

    [Fact]
    public void Next_AtLeftWall_MovingLeft_ShouldReturnSamePoint()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var point = new Point(0, 5);

        // Act
        var result = map.Next(point, Direction.Left);

        // Assert
        Assert.Equal(point, result);
    }

    [Fact]
    public void Next_AtRightWall_MovingRight_ShouldReturnSamePoint()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var point = new Point(9, 5); // Size-1

        // Act
        var result = map.Next(point, Direction.Right);

        // Assert
        Assert.Equal(point, result);
    }

    #endregion

    #region Next Tests - Corner Cases

    [Fact]
    public void Next_AtOrigin_MovingUpOrLeft_ShouldReturnSamePoint()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var origin = new Point(0, 0);

        // Act & Assert
        Assert.Equal(origin, map.Next(origin, Direction.Up));
        Assert.Equal(origin, map.Next(origin, Direction.Left));
    }

    [Fact]
    public void Next_AtOrigin_MovingDownOrRight_ShouldMove()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var origin = new Point(0, 0);

        // Act & Assert
        Assert.Equal(new Point(0, 1), map.Next(origin, Direction.Down));
        Assert.Equal(new Point(1, 0), map.Next(origin, Direction.Right));
    }

    #endregion

    #region Next Tests - Point Outside Map (Should Not Throw)

    [Fact]
    public void Next_PointOutsideMap_ShouldNotThrowException()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var outsidePoint = new Point(-5, -5);

        // Act & Assert - should not throw
        var result = map.Next(outsidePoint, Direction.Right);
        Assert.Equal(outsidePoint, result); // Should return same point as it's outside
    }

    #endregion

    #region NextDiagonal Tests - Movement Inside Map

    [Fact]
    public void NextDiagonal_InsideMap_ShouldMoveUpRight()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var point = new Point(5, 5);

        // Act
        var result = map.NextDiagonal(point, Direction.Up);

        // Assert
        Assert.Equal(new Point(6, 4), result);
    }

    [Fact]
    public void NextDiagonal_InsideMap_ShouldMoveDownRight()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var point = new Point(5, 5);

        // Act
        var result = map.NextDiagonal(point, Direction.Right);

        // Assert
        Assert.Equal(new Point(6, 6), result);
    }

    [Fact]
    public void NextDiagonal_InsideMap_ShouldMoveDownLeft()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var point = new Point(5, 5);

        // Act
        var result = map.NextDiagonal(point, Direction.Down);

        // Assert
        Assert.Equal(new Point(4, 6), result);
    }

    [Fact]
    public void NextDiagonal_InsideMap_ShouldMoveUpLeft()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var point = new Point(5, 5);

        // Act
        var result = map.NextDiagonal(point, Direction.Left);

        // Assert
        Assert.Equal(new Point(4, 4), result);
    }

    #endregion

    #region NextDiagonal Tests - At Walls/Corners

    [Fact]
    public void NextDiagonal_AtTopLeftCorner_MovingUpOrLeft_ShouldReturnSamePoint()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var corner = new Point(0, 0);

        // Act & Assert
        Assert.Equal(corner, map.NextDiagonal(corner, Direction.Up));
        Assert.Equal(corner, map.NextDiagonal(corner, Direction.Left));
    }

    [Fact]
    public void NextDiagonal_AtTopRightCorner_MovingUp_ShouldReturnSamePoint()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var corner = new Point(9, 0);

        // Act
        var result = map.NextDiagonal(corner, Direction.Up);

        // Assert
        Assert.Equal(corner, result); // Would move to (10, -1) which is outside
    }

    [Fact]
    public void NextDiagonal_AtBottomRightCorner_MovingRightOrDown_ShouldReturnSamePoint()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var corner = new Point(9, 9);

        // Act & Assert
        Assert.Equal(corner, map.NextDiagonal(corner, Direction.Right));
        Assert.Equal(corner, map.NextDiagonal(corner, Direction.Down));
    }

    #endregion

    #region NextDiagonal Tests - Point Outside Map

    [Fact]
    public void NextDiagonal_PointOutsideMap_ShouldNotThrowException()
    {
        // Arrange
        var map = new SmallSquareMap(10);
        var outsidePoint = new Point(15, 15);

        // Act & Assert - should not throw
        var result = map.NextDiagonal(outsidePoint, Direction.Up);
        Assert.Equal(outsidePoint, result);
    }

    #endregion

    #region Edge Cases - Different Map Sizes

    [Fact]
    public void SmallestMap_Size5_ShouldWorkCorrectly()
    {
        // Arrange
        var map = new SmallSquareMap(5);

        // Act & Assert
        Assert.True(map.Exist(new Point(0, 0)));
        Assert.True(map.Exist(new Point(4, 4)));
        Assert.False(map.Exist(new Point(5, 5)));
        Assert.False(map.Exist(new Point(-1, 0)));
    }

    [Fact]
    public void LargestMap_Size20_ShouldWorkCorrectly()
    {
        // Arrange
        var map = new SmallSquareMap(20);

        // Act & Assert
        Assert.True(map.Exist(new Point(0, 0)));
        Assert.True(map.Exist(new Point(19, 19)));
        Assert.False(map.Exist(new Point(20, 20)));
        Assert.False(map.Exist(new Point(-1, 0)));
    }

    #endregion
}
