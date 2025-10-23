namespace Simulator;

public class Creature
{
    private string _name = "Unknown";
    private bool _nameSetOnce = false;

    private int? _level = null;
    private bool _levelSetOnce = false;

    public string Name
    {
        get => _name;
        set
        {
            if (_nameSetOnce) return;
            _name = Normalize(value, maxLen: 25);
            _nameSetOnce = true;
        }
    }

    public int Level
    {
        get => _level ?? 1;
        set
        {
            if (_levelSetOnce) return;
            int v = value;
            if (v < 1) v = 1;
            if (v > 10) v = 10;
            _level = v;
            _levelSetOnce = true;
        }
    }

    public Creature(string name, int level = 1)
    {
        Name = name;
        // klucz: NIE wywołujemy settera, gdy level == 1 (domyślna wartość)
        // dzięki temu initializer { Level = 7 } może się jeszcze wykonać „raz przy inicjacji”
        if (level != 1)
            Level = level;
    }

    public Creature() { }

    public void SayHi() =>
        Console.WriteLine($"Hi! I'm {Name}, level {Level} creature.");

    public string Info => $"{Name} <{Level}>";

    public void Upgrade()
    {
        if (_level is null) _level = 1;
        if (_level < 10) _level++;
    }

    private static string Normalize(string? input, int maxLen)
    {
        string s = (input ?? string.Empty).Trim();

        if (s.Length > maxLen)
            s = s[..maxLen].TrimEnd();

        if (s.Length < 3)
            s = s.PadRight(3, '#');

        if (s.Length > 0 && char.IsLetter(s[0]) && char.IsLower(s[0]))
            s = char.ToUpperInvariant(s[0]) + s[1..];

        return s;
    }
}

