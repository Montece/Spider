namespace Spider;

internal sealed class AppSettings
{
    public double Speed { get; set; } = 1d;
    public bool StartWithWindows { get; set; } = false;
    public bool OnlyMainScreen { get; set; } = false;
}