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

    public string Info => $"{Description} <{Size}>";

    private static string Normalize(string? input)
    {
        // First trim the input
        string s = (input ?? string.Empty).Trim();

        // Use Validator.Shortener for length validation (min: 3, max: 15, placeholder: '#')
        s = Validator.Shortener(s, 3, 15, '#');

        // Capitalize first letter if it's a lowercase letter
        if (s.Length > 0 && char.IsLetter(s[0]) && char.IsLower(s[0]))
        {
            s = char.ToUpperInvariant(s[0]) + s[1..];
        }

        return s;
    }
}
