using Simulator;

namespace TestSimulator;

public class ValidatorTests
{
    #region Limiter Tests - Value in Range

    [Theory]
    [InlineData(5, 0, 10, 5)]
    [InlineData(0, 0, 10, 0)]
    [InlineData(10, 0, 10, 10)]
    [InlineData(7, 5, 15, 7)]
    public void Limiter_ValueInRange_ShouldReturnUnchanged(int value, int min, int max, int expected)
    {
        // Arrange, Act
        var result = Validator.Limiter(value, min, max);

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion

    #region Limiter Tests - Value Below Min

    [Theory]
    [InlineData(-5, 0, 10, 0)]
    [InlineData(3, 5, 15, 5)]
    [InlineData(-100, -50, 50, -50)]
    public void Limiter_ValueBelowMin_ShouldReturnMin(int value, int min, int max, int expected)
    {
        // Arrange, Act
        var result = Validator.Limiter(value, min, max);

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion

    #region Limiter Tests - Value Above Max

    [Theory]
    [InlineData(15, 0, 10, 10)]
    [InlineData(100, 5, 50, 50)]
    [InlineData(200, -100, 100, 100)]
    public void Limiter_ValueAboveMax_ShouldReturnMax(int value, int min, int max, int expected)
    {
        // Arrange, Act
        var result = Validator.Limiter(value, min, max);

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion

    #region Limiter Tests - Edge Case Min Equals Max

    [Theory]
    [InlineData(5, 10, 10, 10)]
    [InlineData(10, 10, 10, 10)]
    [InlineData(15, 10, 10, 10)]
    public void Limiter_MinEqualsMax_ShouldAlwaysReturnThatValue(int value, int min, int max, int expected)
    {
        // Arrange, Act
        var result = Validator.Limiter(value, min, max);

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion

    #region Shortener Tests - Null/Empty/Whitespace

    [Theory]
    [InlineData(null, 3, 10, '#', "###")]
    [InlineData("", 5, 10, '*', "*****")]
    [InlineData("   ", 4, 8, '_', "____")]
    [InlineData("\t\n", 3, 10, '-', "---")]
    public void Shortener_NullOrWhitespace_ShouldReturnPlaceholders(string value, int min, int max, char placeholder, string expected)
    {
        // Arrange, Act
        var result = Validator.Shortener(value, min, max, placeholder);

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion

    #region Shortener Tests - Length Below Min (Padding)

    [Theory]
    [InlineData("Hi", 5, 10, '#', "Hi###")]
    [InlineData("A", 3, 10, '*', "A**")]
    [InlineData("Test", 8, 15, '_', "Test____")]
    public void Shortener_LengthBelowMin_ShouldPadWithPlaceholder(string value, int min, int max, char placeholder, string expected)
    {
        // Arrange, Act
        var result = Validator.Shortener(value, min, max, placeholder);

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion

    #region Shortener Tests - Length in Range

    [Theory]
    [InlineData("Hello", 3, 10, '#', "Hello")]
    [InlineData("Test", 3, 5, '*', "Test")]
    [InlineData("Perfect", 5, 10, '_', "Perfect")]
    [InlineData("ExactMin", 8, 15, '-', "ExactMin")]
    [InlineData("ExactMax12", 5, 10, '!', "ExactMax12")]
    public void Shortener_LengthInRange_ShouldReturnUnchanged(string value, int min, int max, char placeholder, string expected)
    {
        // Arrange, Act
        var result = Validator.Shortener(value, min, max, placeholder);

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion

    #region Shortener Tests - Length Above Max (Truncation)

    [Theory]
    [InlineData("VeryLongString", 3, 8, '#', "VeryLon#")]
    [InlineData("TooManyCharacters", 5, 10, '*', "TooManyCh*")]
    [InlineData("ExceedsMaximum", 3, 7, '!', "Exceed!")]
    public void Shortener_LengthAboveMax_ShouldTruncateWithPlaceholder(string value, int min, int max, char placeholder, string expected)
    {
        // Arrange, Act
        var result = Validator.Shortener(value, min, max, placeholder);

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion

    #region Shortener Tests - Unicode and Polish Characters

    [Fact]
    public void Shortener_WithPolishCharacters_ShouldHandleCorrectly()
    {
        // Arrange
        string value = "Zażółć gęślą jaźń";
        int min = 5;
        int max = 20;
        char placeholder = '#';

        // Act
        var result = Validator.Shortener(value, min, max, placeholder);

        // Assert
        Assert.Equal(value, result); // Should remain unchanged as length is in range
    }

    [Fact]
    public void Shortener_PolishCharactersPadding_ShouldWork()
    {
        // Arrange
        string value = "Łód";
        int min = 10;
        int max = 20;
        char placeholder = '*';

        // Act
        var result = Validator.Shortener(value, min, max, placeholder);

        // Assert
        Assert.Equal("Łód*******", result);
    }

    [Fact]
    public void Shortener_PolishCharactersTruncation_ShouldWork()
    {
        // Arrange
        string value = "Zażółć gęślą jaźń bardzo długi tekst";
        int min = 5;
        int max = 10;
        char placeholder = '!';

        // Act
        var result = Validator.Shortener(value, min, max, placeholder);

        // Assert
        Assert.Equal(10, result.Length);
        Assert.EndsWith("!", result);
    }

    [Fact]
    public void Shortener_WithEmojis_ShouldHandleCorrectly()
    {
        // Arrange
        string value = "Test😀";
        int min = 3;
        int max = 10;
        char placeholder = '#';

        // Act
        var result = Validator.Shortener(value, min, max, placeholder);

        // Assert
        Assert.Equal(value, result); // Length in range, should remain unchanged
    }

    #endregion

    #region Shortener Tests - Edge Cases

    [Fact]
    public void Shortener_MinEqualsMax_ExactLength_ShouldReturnUnchanged()
    {
        // Arrange
        string value = "12345";
        int min = 5;
        int max = 5;
        char placeholder = '#';

        // Act
        var result = Validator.Shortener(value, min, max, placeholder);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void Shortener_MinEqualsMax_TooShort_ShouldPad()
    {
        // Arrange
        string value = "123";
        int min = 5;
        int max = 5;
        char placeholder = '#';

        // Act
        var result = Validator.Shortener(value, min, max, placeholder);

        // Assert
        Assert.Equal("123##", result);
    }

    [Fact]
    public void Shortener_MinEqualsMax_TooLong_ShouldTruncate()
    {
        // Arrange
        string value = "1234567";
        int min = 5;
        int max = 5;
        char placeholder = '#';

        // Act
        var result = Validator.Shortener(value, min, max, placeholder);

        // Assert
        Assert.Equal("1234#", result);
        Assert.Equal(5, result.Length);
    }

    [Fact]
    public void Shortener_DifferentPlaceholders_ShouldWork()
    {
        // Arrange & Act
        var result1 = Validator.Shortener("Hi", 5, 10, '.');
        var result2 = Validator.Shortener("Hi", 5, 10, '-');
        var result3 = Validator.Shortener("Hi", 5, 10, '█');

        // Assert
        Assert.Equal("Hi...", result1);
        Assert.Equal("Hi---", result2);
        Assert.Equal("Hi███", result3);
    }

    #endregion
}
