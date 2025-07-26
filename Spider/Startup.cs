using System.IO;

namespace Spider;

internal static class Startup
{
    public static void AddToStartupFolder()
    {
        var appPath = Environment.ProcessPath;
        var startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        var shortcutPath = Path.Combine(startupPath, "Spider.lnk");
    
        if (File.Exists(shortcutPath))
        {
            return;
        }

        var shellType = Type.GetTypeFromProgID("WScript.Shell");
        dynamic shell = Activator.CreateInstance(shellType);
        var shortcut = shell.CreateShortcut(shortcutPath);

        shortcut.TargetPath = appPath;
        shortcut.WorkingDirectory = Path.GetDirectoryName(appPath);
        shortcut.WindowStyle = 1;
        shortcut.Description = "Паук 🕷";
        shortcut.Save();
    }

    public static void RemoveFromStartupFolder()
    {
        var startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        var shortcutPath = Path.Combine(startupPath, "Spider.lnk");

        if (File.Exists(shortcutPath))
        {
            File.Delete(shortcutPath);
        }
    }
}