namespace Simulator;

/// <summary>
/// Static utility class for common validation operations following DRY principle
/// </summary>
public static class Validator
{
    /// <summary>
    /// Limits a value to be within the specified range [min, max]
    /// </summary>
    /// <param name="value">The value to limit</param>
    /// <param name="min">Minimum allowed value</param>
    /// <param name="max">Maximum allowed value</param>
    /// <returns>Value clamped to [min, max]</returns>
    public static int Limiter(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    /// <summary>
    /// Ensures string length is within [min, max] range
    /// </summary>
    /// <param name="value">The string to validate</param>
    /// <param name="min">Minimum allowed length</param>
    /// <param name="max">Maximum allowed length</param>
    /// <param name="placeholder">Character used for padding or truncation marker</param>
    /// <returns>String with length within [min, max]</returns>
    public static string Shortener(string value, int min, int max, char placeholder)
    {
        // If null, empty, or whitespace - create string of placeholders with min length
        if (string.IsNullOrWhiteSpace(value))
        {
            return new string(placeholder, min);
        }

        // If shorter than min - pad to min length
        if (value.Length < min)
        {
            return value.PadRight(min, placeholder);
        }

        // If longer than max - truncate to max-1 and add placeholder
        if (value.Length > max)
        {
            return value.Substring(0, max - 1) + placeholder;
        }

        // Otherwise return unchanged
        return value;
    }
}
