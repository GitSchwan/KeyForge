namespace KeyForge.Models;

public class Theme
{
    public Theme(string themeId, string startColor, string endColor, double angle, string accentColor, string foregroundColor)
    {
        ThemeId = themeId;
        StartColor = startColor;
        EndColor = endColor;
        Angle = angle;
        AccentColor = accentColor;
        ForegroundColor = foregroundColor;
    }
    
    
    public string ThemeId { get; set; }
    public string StartColor { get; set; }
    public string EndColor { get; set; }
    public double Angle { get; set; }
    public string AccentColor { get; set; }
    public string ForegroundColor { get; set; }
    
    
}