using System.IO;
using System.Text.Json;

namespace Spider;

internal static class ConfigManager
{
    public static AppSettings Instance { get; private set; } = new();

    private const string CONFIG_PATH = "Settings.json";

    public static void Load()
    {
        if (!File.Exists(CONFIG_PATH))
        {
            Instance = new();
            return;
        }

        var json = File.ReadAllText(CONFIG_PATH);
        
        Instance = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
    }

    public static void Save()
    {
        var json = JsonSerializer.Serialize(Instance, new JsonSerializerOptions { WriteIndented = true });
        
        File.WriteAllText(CONFIG_PATH, json);
    }
}