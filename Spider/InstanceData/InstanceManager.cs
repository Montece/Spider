using System.IO;
using System.Text.Json;

namespace Spider.InstanceData;

internal static class InstanceManager
{
    public static Instance Instance { get; private set; } = new();

    private const string PATH = "instance.json";

    public static void Load()
    {
        if (!File.Exists(PATH))
        {
            Instance = new();
            return;
        }

        var json = File.ReadAllText(PATH);
        
        Instance = JsonSerializer.Deserialize<Instance>(json) ?? new Instance();
    }

    public static void Save()
    {
        var json = JsonSerializer.Serialize(Instance, new JsonSerializerOptions { WriteIndented = true });
        
        File.WriteAllText(PATH, json);
    }
}