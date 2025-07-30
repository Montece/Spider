using NLog;
#pragma warning disable CS0168 // Variable is declared but never used

namespace Spider;

internal static class Log
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public static void Write(string message, LogLevel level)
    {
        try
        {
            _logger.Log(level, message);
        }
        catch (Exception ex)
        {
            // ignored
        }
    }
}