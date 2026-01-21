namespace SimConsole;

/// <summary>
/// Circular buffer for storing recent game events.
/// </summary>
public class EventLog
{
    private readonly Queue<string> _events;
    private readonly int _maxSize;

    public EventLog(int maxSize = 10)
    {
        _maxSize = maxSize;
        _events = new Queue<string>(_maxSize);
    }

    public void Add(string message)
    {
        if (_events.Count >= _maxSize)
        {
            _events.Dequeue();
        }
        _events.Enqueue(message);
    }

    public void AddRange(IEnumerable<string> messages)
    {
        foreach (var msg in messages)
        {
            Add(msg);
        }
    }

    public IReadOnlyList<string> GetRecent(int count = 8)
    {
        return _events.TakeLast(count).ToList();
    }

    public void Clear()
    {
        _events.Clear();
    }
}
