namespace Simulator;

/// <summary>
/// Birds class inheriting from Animals with flight capability
/// </summary>
public class Birds : Animals
{
    /// <summary>
    /// Indicates whether the bird can fly
    /// </summary>
    public bool CanFly { get; set; } = true;

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
