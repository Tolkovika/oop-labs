using Simulator;

namespace TestSimulator;

public class RectangleTests
{
    #region Constructor Tests - Normal Cases

    [Fact]
    public void Constructor_WithCorrectOrdering_ShouldSetPropertiesCorrectly()
    {
        // Arrange & Act
        var rect = new Rectangle(1, 2, 5, 8);

        // Assert
        Assert.Equal(1, rect.X1);
        Assert.Equal(2, rect.Y1);
        Assert.Equal(5, rect.X2);
        Assert.Equal(8, rect.Y2);
    }

    [Fact]
    public void Constructor_WithReversedX_ShouldSwapCoordinates()
    {
        // Arrange & Act
        var rect = new Rectangle(5, 2, 1, 8);

        // Assert
        Assert.Equal(1, rect.X1);
        Assert.Equal(2, rect.Y1);
        Assert.Equal(5, rect.X2);
        Assert.Equal(8, rect.Y2);
    }

    [Fact]
    public void Constructor_WithReversedY_ShouldSwapCoordinates()
    {
        // Arrange & Act
        var rect = new Rectangle(1, 8, 5, 2);

        // Assert
        Assert.Equal(1, rect.X1);
        Assert.Equal(2, rect.Y1);
        Assert.Equal(5, rect.X2);
        Assert.Equal(8, rect.Y2);
    }

    [Fact]
    public void Constructor_WithBothReversed_ShouldSwapBothCoordinates()
    {
        // Arrange & Act
        var rect = new Rectangle(5, 8, 1, 2);

        // Assert
        Assert.Equal(1, rect.X1);
        Assert.Equal(2, rect.Y1);
        Assert.Equal(5, rect.X2);
        Assert.Equal(8, rect.Y2);
    }

    #endregion

    #region Constructor Tests - Negative Coordinates

    [Fact]
    public void Constructor_WithNegativeCoordinates_ShouldWorkCorrectly()
    {
        // Arrange & Act
        var rect = new Rectangle(-5, -3, -1, -2);

        // Assert
        Assert.Equal(-5, rect.X1);
        Assert.Equal(-3, rect.Y1);
        Assert.Equal(-1, rect.X2);
        Assert.Equal(-2, rect.Y2);
    }

    [Fact]
    public void Constructor_WithMixedCoordinates_ShouldWorkCorrectly()
    {
        // Arrange & Act
        var rect = new Rectangle(-2, -5, 3, 4);

        // Assert
        Assert.Equal(-2, rect.X1);
        Assert.Equal(-5, rect.Y1);
        Assert.Equal(3, rect.X2);
        Assert.Equal(4, rect.Y2);
    }

    #endregion

    #region Constructor Tests - Degenerate Rectangle Exceptions

    [Fact]
    public void Constructor_WithSameX_ShouldThrowArgumentException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() => new Rectangle(5, 2, 5, 8));
    }

    [Fact]
    public void Constructor_WithSameY_ShouldThrowArgumentException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() => new Rectangle(1, 5, 6, 5));
    }

    [Fact]
    public void Constructor_WithSameXAndY_ShouldThrowArgumentException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() => new Rectangle(3, 3, 3, 3));
    }

    #endregion

    #region Constructor Tests - Point Parameters

    [Fact]
    public void Constructor_WithPoints_ShouldCreateRectangle()
    {
        // Arrange
        var p1 = new Point(1, 2);
        var p2 = new Point(5, 8);

        // Act
        var rect = new Rectangle(p1, p2);

        // Assert
        Assert.Equal(1, rect.X1);
        Assert.Equal(2, rect.Y1);
        Assert.Equal(5, rect.X2);
        Assert.Equal(8, rect.Y2);
    }

    [Fact]
    public void Constructor_WithPointsReversed_ShouldSwapCoordinates()
    {
        // Arrange
        var p1 = new Point(5, 8);
        var p2 = new Point(1, 2);

        // Act
        var rect = new Rectangle(p1, p2);

        // Assert
        Assert.Equal(1, rect.X1);
        Assert.Equal(2, rect.Y1);
        Assert.Equal(5, rect.X2);
        Assert.Equal(8, rect.Y2);
    }

    [Fact]
    public void Constructor_WithPointsOnSameX_ShouldThrowArgumentException()
    {
        // Arrange
        var p1 = new Point(3, 2);
        var p2 = new Point(3, 8);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Rectangle(p1, p2));
    }

    #endregion

    #region Contains Tests - Inside

    [Fact]
    public void Contains_PointInside_ShouldReturnTrue()
    {
        // Arrange
        var rect = new Rectangle(0, 0, 10, 10);
        var point = new Point(5, 5);

        // Act
        var result = rect.Contains(point);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(5, 5)]
    [InlineData(9, 9)]
    [InlineData(3, 7)]
    public void Contains_VariousPointsInside_ShouldReturnTrue(int x, int y)
    {
        // Arrange
        var rect = new Rectangle(0, 0, 10, 10);
        var point = new Point(x, y);

        // Act
        var result = rect.Contains(point);

        // Assert
        Assert.True(result);
    }

    #endregion

    #region Contains Tests - On Edge

    [Fact]
    public void Contains_PointOnX1Edge_ShouldReturnTrue()
    {
        // Arrange
        var rect = new Rectangle(0, 0, 10, 10);
        var point = new Point(0, 5);

        // Act
        var result = rect.Contains(point);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Contains_PointOnX2Edge_ShouldReturnTrue()
    {
        // Arrange
        var rect = new Rectangle(0, 0, 10, 10);
        var point = new Point(10, 5);

        // Act
        var result = rect.Contains(point);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Contains_PointOnY1Edge_ShouldReturnTrue()
    {
        // Arrange
        var rect = new Rectangle(0, 0, 10, 10);
        var point = new Point(5, 0);

        // Act
        var result = rect.Contains(point);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Contains_PointOnY2Edge_ShouldReturnTrue()
    {
        // Arrange
        var rect = new Rectangle(0, 0, 10, 10);
        var point = new Point(5, 10);

        // Act
        var result = rect.Contains(point);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(0, 0)]  // X1, Y1
    [InlineData(10, 0)] // X2, Y1
    [InlineData(0, 10)] // X1, Y2
    [InlineData(10, 10)] // X2, Y2
    public void Contains_PointOnCorner_ShouldReturnTrue(int x, int y)
    {
        // Arrange
        var rect = new Rectangle(0, 0, 10, 10);
        var point = new Point(x, y);

        // Act
        var result = rect.Contains(point);

        // Assert
        Assert.True(result);
    }

    #endregion

    #region Contains Tests - Outside

    [Theory]
    [InlineData(-1, 5)]   // Left of X1
    [InlineData(11, 5)]   // Right of X2
    [InlineData(5, -1)]   // Below Y1
    [InlineData(5, 11)]   // Above Y2
    [InlineData(-1, -1)]  // Outside bottom-left
    [InlineData(11, 11)]  // Outside top-right
    [InlineData(15, 5)]   // Far outside
    public void Contains_PointOutside_ShouldReturnFalse(int x, int y)
    {
        // Arrange
        var rect = new Rectangle(0, 0, 10, 10);
        var point = new Point(x, y);

        // Act
        var result = rect.Contains(point);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region Contains Tests - Negative Coordinates

    [Fact]
    public void Contains_WithNegativeRectangle_ShouldWorkCorrectly()
    {
        // Arrange
        var rect = new Rectangle(-10, -10, -1, -1);

        // Act & Assert
        Assert.True(rect.Contains(new Point(-5, -5)));  // Inside
        Assert.True(rect.Contains(new Point(-10, -10))); // Corner
        Assert.True(rect.Contains(new Point(-1, -1)));   // Corner
        Assert.False(rect.Contains(new Point(0, 0)));    // Outside
        Assert.False(rect.Contains(new Point(-11, -5))); // Outside
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_ShouldReturnCorrectFormat()
    {
        // Arrange
        var rect = new Rectangle(1, 2, 5, 8);

        // Act
        var result = rect.ToString();

        // Assert
        Assert.Equal("(1, 2):(5, 8)", result);
    }

    [Fact]
    public void ToString_WithNegativeCoordinates_ShouldReturnCorrectFormat()
    {
        // Arrange
        var rect = new Rectangle(-5, -3, -1, -2);

        // Act
        var result = rect.ToString();

        // Assert
        Assert.Equal("(-5, -3):(-1, -2)", result);
    }

    [Fact]
    public void ToString_WithReversedInput_ShouldShowCorrectedCoordinates()
    {
        // Arrange
        var rect = new Rectangle(5, 8, 1, 2);

        // Act
        var result = rect.ToString();

        // Assert
        Assert.Equal("(1, 2):(5, 8)", result);
    }

    #endregion
}
