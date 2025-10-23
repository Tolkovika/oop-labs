namespace Simulator;

public static class DirectionParser
{
    public static Direction[] Parse(string? input)
    {
        if (string.IsNullOrEmpty(input)) return Array.Empty<Direction>();

        var list = new List<Direction>(input.Length);
        foreach (char ch in input)
        {
            switch (char.ToUpperInvariant(ch))
            {
                case 'U': list.Add(Direction.Up); break;
                case 'R': list.Add(Direction.Right); break;
                case 'D': list.Add(Direction.Down); break;
                case 'L': list.Add(Direction.Left); break;
                // reszta znaków ignorowana
            }
        }
        return list.ToArray();
    }
}

