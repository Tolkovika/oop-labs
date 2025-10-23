namespace Simulator;

public class Animals
{
    // prywatne pole z domyślną wartością (żeby nie było warningu)
    private string _description = string.Empty;
    private bool _descriptionSetOnce = false;

    public string Description
    {
        get => _description;
        set
        {
            if (_descriptionSetOnce) return;    
            _description = Normalize(value, maxLen: 15);
            _descriptionSetOnce = true;
        }
    }

    public uint Size { get; set; } = 3;

    // Dogs <3>
    public string Info => $"{Description} <{Size}>";

    private static string Normalize(string? input, int maxLen)
    {
        string s = (input ?? string.Empty).Trim();

        if (s.Length > maxLen)
        {
            s = s[..maxLen].TrimEnd();
        }

        if (s.Length < 3)
        {
            s = s.PadRight(3, '#');
        }

        if (s.Length > 0 && char.IsLetter(s[0]) && char.IsLower(s[0]))
        {
            s = char.ToUpperInvariant(s[0]) + s[1..];
        }

        return s;
    }
}

