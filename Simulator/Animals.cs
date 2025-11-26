namespace Simulator;

public class Animals
{
    private string _description = string.Empty;
    private bool _descriptionSetOnce = false;

    public string Description
    {
        get => _description;
        set
        {
            if (_descriptionSetOnce) return;
            _description = Normalize(value);
            _descriptionSetOnce = true;
        }
    }

    public uint Size { get; set; } = 3;

    /// <summary>
    /// Virtual Info property - can be overridden by derived classes
    /// Format: Description <Size>
    /// </summary>
    public virtual string Info => $"{Description} <{Size}>";

    /// <summary>
    /// Returns string representation in format: ANIMALS: Info
    /// </summary>
    public override string ToString()
    {
        return $"{GetType().Name.ToUpper()}: {Info}";
    }

    private static string Normalize(string? input)
    {
        // First trim the input
        string s = (input ?? string.Empty).Trim();

        // Use Validator.Shortener for length validation (min: 3, max: 15, placeholder: '#')
        s = Validator.Shortener(s, 3, 15, '#');

        // Capitalize first letter, rest lowercase
        if (s.Length > 0 && char.IsLetter(s[0]))
        {
            s = char.ToUpperInvariant(s[0]) + s.Substring(1).ToLowerInvariant();
        }

        return s;
    }
}
