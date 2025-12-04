using Simulator;

namespace TestSimulator;

public class PointTests
{
    #region Constructor and Properties Tests

    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        // Arrange & Act
        var point = new Point(5, 10);

        // Assert
        Assert.Equal(5, point.X);
        Assert.Equal(10, point.Y);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(-5, -10)]
    [InlineData(100, 200)]
    [InlineData(-1, 1)]
    public void Constructor_ShouldHandleVariousCoordinates(int x, int y)
    {
        // Arrange & Act
        var point = new Point(x, y);

        // Assert
        Assert.Equal(x, point.X);
        Assert.Equal(y, point.Y);
    }

    #endregion

    #region ToString Tests

    [Theory]
    [InlineData(0, 0, "(0, 0)")]
    [InlineData(5, 10, "(5, 10)")]
    [InlineData(-3, -7, "(-3, -7)")]
    [InlineData(-1, 5, "(-1, 5)")]
    [InlineData(100, -50, "(100, -50)")]
    public void ToString_ShouldReturnCorrectFormat(int x, int y, string expected)
    {
        // Arrange
        var point = new Point(x, y);

        // Act
        var result = point.ToString();

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion

    #region Next Tests - All Directions

    [Fact]
    public void Next_Up_ShouldDecreaseY()
    {
        // Arrange
        var point = new Point(5, 5);

        // Act
        var result = point.Next(Direction.Up);

        // Assert
        Assert.Equal(new Point(5, 4), result);
    }

    [Fact]
    public void Next_Down_ShouldIncreaseY()
    {
        // Arrange
        var point = new Point(5, 5);

        // Act
        var result = point.Next(Direction.Down);

        // Assert
        Assert.Equal(new Point(5, 6), result);
    }

    [Fact]
    public void Next_Left_ShouldDecreaseX()
    {
        // Arrange
        var point = new Point(5, 5);

        // Act
        var result = point.Next(Direction.Left);

        // Assert
        Assert.Equal(new Point(4, 5), result);
    }

    [Fact]
    public void Next_Right_ShouldIncreaseX()
    {
        // Arrange
        var point = new Point(5, 5);

        // Act
        var result = point.Next(Direction.Right);

        // Assert
        Assert.Equal(new Point(6, 5), result);
    }

    #endregion

    #region Next Tests - Edge Cases

    [Fact]
    public void Next_FromOrigin_ShouldMoveCorrectly()
    {
        // Arrange
        var origin = new Point(0, 0);

        // Act & Assert
        Assert.Equal(new Point(0, -1), origin.Next(Direction.Up));
        Assert.Equal(new Point(0, 1), origin.Next(Direction.Down));
        Assert.Equal(new Point(-1, 0), origin.Next(Direction.Left));
        Assert.Equal(new Point(1, 0), origin.Next(Direction.Right));
    }

    [Theory]
    [InlineData(-10, -10)]
    [InlineData(100, 100)]
    public void Next_WithLargeCoordinates_ShouldWorkCorrectly(int x, int y)
    {
        // Arrange
        var point = new Point(x, y);

        // Act & Assert
        Assert.Equal(new Point(x, y - 1), point.Next(Direction.Up));
        Assert.Equal(new Point(x, y + 1), point.Next(Direction.Down));
        Assert.Equal(new Point(x - 1, y), point.Next(Direction.Left));
        Assert.Equal(new Point(x + 1, y), point.Next(Direction.Right));
    }

    #endregion

    #region NextDiagonal Tests - All Directions

    [Fact]
    public void NextDiagonal_Up_ShouldMoveUpRight()
    {
        // Arrange
        var point = new Point(5, 5);

        // Act
        var result = point.NextDiagonal(Direction.Up);

        // Assert
        // Up + 45° CW = Up-Right: X+1, Y-1
        Assert.Equal(new Point(6, 4), result);
    }

    [Fact]
    public void NextDiagonal_Right_ShouldMoveDownRight()
    {
        // Arrange
        var point = new Point(5, 5);

        // Act
        var result = point.NextDiagonal(Direction.Right);

        // Assert
        // Right + 45° CW = Down-Right: X+1, Y+1
        Assert.Equal(new Point(6, 6), result);
    }

    [Fact]
    public void NextDiagonal_Down_ShouldMoveDownLeft()
    {
        // Arrange
        var point = new Point(5, 5);

        // Act
        var result = point.NextDiagonal(Direction.Down);

        // Assert
        // Down + 45° CW = Down-Left: X-1, Y+1
        Assert.Equal(new Point(4, 6), result);
    }

    [Fact]
    public void NextDiagonal_Left_ShouldMoveUpLeft()
    {
        // Arrange
        var point = new Point(5, 5);

        // Act
        var result = point.NextDiagonal(Direction.Left);

        // Assert
        // Left + 45° CW = Up-Left: X-1, Y-1
        Assert.Equal(new Point(4, 4), result);
    }

    #endregion

    #region NextDiagonal Tests - Edge Cases

    [Fact]
    public void NextDiagonal_FromOrigin_ShouldMoveCorrectly()
    {
        // Arrange
        var origin = new Point(0, 0);

        // Act & Assert
        Assert.Equal(new Point(1, -1), origin.NextDiagonal(Direction.Up));
        Assert.Equal(new Point(1, 1), origin.NextDiagonal(Direction.Right));
        Assert.Equal(new Point(-1, 1), origin.NextDiagonal(Direction.Down));
        Assert.Equal(new Point(-1, -1), origin.NextDiagonal(Direction.Left));
    }

    [Theory]
    [InlineData(-5, -5)]
    [InlineData(50, -30)]
    [InlineData(-20, 40)]
    public void NextDiagonal_WithNegativeCoordinates_ShouldWorkCorrectly(int x, int y)
    {
        // Arrange
        var point = new Point(x, y);

        // Act & Assert
        Assert.Equal(new Point(x + 1, y - 1), point.NextDiagonal(Direction.Up));
        Assert.Equal(new Point(x + 1, y + 1), point.NextDiagonal(Direction.Right));
        Assert.Equal(new Point(x - 1, y + 1), point.NextDiagonal(Direction.Down));
        Assert.Equal(new Point(x - 1, y - 1), point.NextDiagonal(Direction.Left));
    }

    #endregion

    #region Equality Tests (bonus - struct comparison)

    [Fact]
    public void Points_WithSameCoordinates_ShouldBeEqual()
    {
        // Arrange
        var point1 = new Point(5, 10);
        var point2 = new Point(5, 10);

        // Act & Assert
        Assert.Equal(point1, point2);
    }

    [Fact]
    public void Points_WithDifferentCoordinates_ShouldNotBeEqual()
    {
        // Arrange
        var point1 = new Point(5, 10);
        var point2 = new Point(5, 11);

        // Act & Assert
        Assert.NotEqual(point1, point2);
    }

    #endregion
}
