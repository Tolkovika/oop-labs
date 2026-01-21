using SimCore.Terrain;
using SimCore.Units.Interfaces;

namespace SimCore.Map;

public class GameMap
{
    public int SizeX { get; }
    public int SizeY { get; }
    private readonly Tile[,] _tiles;

    public GameMap(int sizeX, int sizeY)
    {
        SizeX = sizeX;
        SizeY = sizeY;
        _tiles = new Tile[sizeX, sizeY];
        
        // Initialize with default terrain (e.g. Plain)
        // Can be improved later with map generation
        for (int x = 0; x < SizeX; x++)
        {
            for (int y = 0; y < SizeY; y++)
            {
                _tiles[x, y] = new Tile(TerrainType.Plain);
            }
        }
    }

    public Tile At(int x, int y)
    {
        if (!IsValid(x, y)) throw new ArgumentOutOfRangeException($"{x},{y}");
        return _tiles[x, y];
    }
    
    public Tile At(Point p) => At(p.X, p.Y);

    public bool IsValid(int x, int y) => x >= 0 && x < SizeX && y >= 0 && y < SizeY;
    public bool IsValid(Point p) => IsValid(p.X, p.Y);

    public void Place(IMappable unit, Point position)
    {
        if (!IsValid(position)) return;
        
        At(position).Units.Add(unit);
        unit.InitMap(this, position);
    }

    public void Remove(IMappable unit, Point position)
    {
        if (!IsValid(position)) return;
        At(position).Units.Remove(unit);
    }
    
    public void Move(IMappable unit, Point from, Point to)
    {
        if (!IsValid(from) || !IsValid(to)) return;
        
        At(from).Units.Remove(unit);
        At(to).Units.Add(unit);
    }

    // Helper to set terrain for demo
    public void SetTerrain(int x, int y, TerrainType terrain)
    {
        if (IsValid(x, y))
        {
            _tiles[x, y] = new Tile(terrain); // Replaces tile but beware of existing units if any
        }
    }
}
