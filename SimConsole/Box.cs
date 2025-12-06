namespace SimConsole;

/// <summary>
/// Box-drawing characters for creating borders and frames in console.
/// Uses Unicode Box Drawing characters (U+2500 to U+257F).
/// </summary>
public static class Box
{
    /// <summary>Horizontal line: ─</summary>
    public const char Horizontal = '\u2500';
    
    /// <summary>Vertical line: │</summary>
    public const char Vertical = '\u2502';
    
    /// <summary>Cross intersection: ┼</summary>
    public const char Cross = '\u253C';
    
    /// <summary>Top-left corner: ┌</summary>
    public const char TopLeft = '\u250C';
    
    /// <summary>Top-right corner: ┐</summary>
    public const char TopRight = '\u2510';
    
    /// <summary>Top middle connector: ┬</summary>
    public const char TopMid = '\u252C';
    
    /// <summary>Bottom-left corner: └</summary>
    public const char BottomLeft = '\u2514';
    
    /// <summary>Bottom middle connector: ┴</summary>
    public const char BottomMid = '\u2534';
    
    /// <summary>Bottom-right corner: ┘</summary>
    public const char BottomRight = '\u2518';
    
    /// <summary>Middle-left connector: ├</summary>
    public const char MidLeft = '\u251C';
    
    /// <summary>Middle-right connector: ┤</summary>
    public const char MidRight = '\u2524';
}
