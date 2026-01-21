using Simulator.Maps;

namespace Simulator;

/// <summary>
/// Birds class inheriting from Animals with flight capability.
/// Flying birds move 2 positions, non-flying birds use diagonal movement.
/// </summary>
public class Birds : Animals
{
    /// <summary>
    /// Indicates whether the bird can fly
    /// </summary>
    public bool CanFly { get; set; } = true;

    public bool IsEagle => Description?.Contains("Eagle", StringComparison.OrdinalIgnoreCase) == true;
    public bool IsOstrich => Description?.Contains("Ostrich", StringComparison.OrdinalIgnoreCase) == true;

    /// <summary>
    /// Symbol for map visualization.
    /// Flying birds: B, non-flying birds: b
    /// </summary>
    public override char Symbol => CanFly ? 'B' : 'b';

    /// <summary>
    /// Move the bird in the specified direction.
    /// Flying birds move 2 positions in direction.
    /// Non-flying birds use diagonal movement.
    /// </summary>
    public override void Go(Direction direction)
    {
        if (Map == null || Position == null)
        {
            return;
        }

        Point currentPos = Position.Value;
        Point newPos;

        if (CanFly)
        {
            // Flying birds move 2 positions in direction
            Point firstStep = Map.Next(currentPos, direction);
            newPos = Map.Next(firstStep, direction);
        }
        else
        {
            // Non-flying birds use diagonal movement
            newPos = Map.NextDiagonal(currentPos, direction);
        }

        if (!newPos.Equals(currentPos))
        {
            Map.Move(this, currentPos, newPos);
            Position = newPos;
        }
    }

    /// <summary>
    /// Override Info to include flight indicator
    /// Format: Description (fly+) <Size> or Description (fly-) <Size>
    /// </summary>
    public override string Info
    {
        get
        {
            string flyIndicator = CanFly ? "(fly+)" : "(fly-)";
            return $"{Description} {flyIndicator} <{Size}>";
        }
    }
}

