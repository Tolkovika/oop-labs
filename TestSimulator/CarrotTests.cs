using Simulator;
using Simulator.Maps;
using Xunit;

namespace TestSimulator;

/// <summary>
/// Tests for carrot drop/pickup gameplay feature.
/// </summary>
public class CarrotTests
{
    [Fact]
    public void Animal_IsRabbit_ReturnsTrueForRabbits()
    {
        var rabbit = new Animals { Description = "Rabbits", Size = 5 };
        var dog = new Animals { Description = "Dogs", Size = 3 };
        
        Assert.True(rabbit.IsRabbit);
        Assert.False(dog.IsRabbit);
    }
    
    [Fact]
    public void Animal_StartsWithTenCarrots()
    {
        var animal = new Animals { Description = "Rabbits" };
        Assert.Equal(10, animal.Carrots);
    }
    
    [Fact]
    public void Elf_StartsWithZeroCarrots()
    {
        var elf = new Elf("Legolas", 5, 3);
        Assert.Equal(0, elf.CarrotsCarried);
    }
    
    [Fact]
    public void Map_CarrotStorage_WorksCorrectly()
    {
        var map = new SmallSquareMap(5);
        var pos = new Point(2, 2);
        
        // Initially no carrots
        Assert.Equal(0, map.GetCarrots(pos));
        
        // Add carrots
        map.AddCarrots(pos, 3);
        Assert.Equal(3, map.GetCarrots(pos));
        
        // Add more
        map.AddCarrots(pos, 2);
        Assert.Equal(5, map.GetCarrots(pos));
        
        // Set to zero removes
        map.SetCarrots(pos, 0);
        Assert.Equal(0, map.GetCarrots(pos));
    }
    
    [Fact]
    public void Simulation_RabbitDropsCarrot_AfterMove()
    {
        // Arrange
        var map = new SmallSquareMap(5);
        var rabbit = new Animals { Description = "Rabbits", Size = 5 };
        var mappables = new List<IMappable> { rabbit };
        var positions = new List<Point> { new(2, 2) };
        var moves = "r"; // Move right
        
        var sim = new Simulation(map, mappables, positions, moves);
        int initialCarrots = rabbit.Carrots;
        
        // Act
        sim.Turn();
        
        // Assert - rabbit moved to (3,2) and dropped a carrot there
        Assert.Equal(new Point(3, 2), rabbit.Position);
        Assert.Equal(initialCarrots - 1, rabbit.Carrots);
        Assert.Equal(1, map.GetCarrots(new Point(3, 2)));
    }
    
    [Fact]
    public void Simulation_ElfPicksUpCarrots()
    {
        // Arrange
        var map = new SmallSquareMap(5);
        var elf = new Elf("Legolas");
        var mappables = new List<IMappable> { elf };
        var positions = new List<Point> { new(2, 2) };
        var moves = "r"; // Move right
        
        // Pre-place carrots at destination
        map.AddCarrots(new Point(3, 2), 5);
        
        var sim = new Simulation(map, mappables, positions, moves);
        
        // Act
        sim.Turn();
        
        // Assert - elf picked up carrots
        Assert.Equal(new Point(3, 2), elf.Position);
        Assert.Equal(5, elf.CarrotsCarried);
        Assert.Equal(0, map.GetCarrots(new Point(3, 2)));
    }
}
