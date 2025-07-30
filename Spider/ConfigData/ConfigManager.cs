using System.IO;
using System.Text.Json;

namespace Spider.ConfigData;

internal static class ConfigManager
{
    public static Config Instance { get; private set; } = new();

    private const string PATH = "config.json";

    public static void Load()
    {
        if (!File.Exists(PATH))
        {
            Instance = new();
            return;
        }

        var json = File.ReadAllText(PATH);
        
        Instance = JsonSerializer.Deserialize<Config>(json) ?? new Config();
    }

    public static void Save()
    {
        var json = JsonSerializer.Serialize(Instance, new JsonSerializerOptions { WriteIndented = true });
        
        File.WriteAllText(PATH, json);
    }
}