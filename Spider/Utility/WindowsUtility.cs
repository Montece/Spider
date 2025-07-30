using Microsoft.VisualBasic.FileIO;
using Spider.ConfigData;
using System.Windows;

namespace Spider.Utility;

internal static class WindowsUtility
{
    public static (double, double) GetScreenBounds()
    {
        double screenWidth;
        double screenHeight;

        if (ConfigManager.Instance.OnlyMainScreen)
        {
            screenWidth = SystemParameters.PrimaryScreenWidth;
            screenHeight = SystemParameters.PrimaryScreenHeight;
        }
        else
        {
            screenWidth = SystemParameters.VirtualScreenWidth;
            screenHeight = SystemParameters.VirtualScreenHeight;
        }

        return (screenWidth, screenHeight);
    }

    public static void DeleteFileAsync(string filepath)
    {
        Task.Run(() =>
        {
            FileSystem.DeleteFile(filepath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
        });
    }
}