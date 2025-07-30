using TagLib;

namespace Spider.Utility;

internal static class MediaFileUtility
{
    public static string GetArtist(string path)
    {
        try
        {
            var file = File.Create(path);
            return file.Tag.FirstPerformer ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    public static string GetTitle(string path)
    {
        try
        {
            var file = File.Create(path);
            return file.Tag.Title ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}