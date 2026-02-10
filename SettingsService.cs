using System.Text.Json;

namespace YellowPoint;

public sealed class SettingsService
{
    private const string FileName = "settings.json";

    public AppSettings Load()
    {
        var path = GetSettingsPath();
        if (!File.Exists(path))
        {
            return new AppSettings();
        }

        try
        {
            var json = File.ReadAllText(path);
            var settings = JsonSerializer.Deserialize(json, AppSettingsJsonContext.Default.AppSettings) ?? new AppSettings();
            return Sanitize(settings);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            AppLogger.LogException("Failed to load settings. Using defaults.", ex);
            return new AppSettings();
        }
    }

    public bool Save(AppSettings settings)
    {
        var path = GetSettingsPath();
        try
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(Sanitize(settings), AppSettingsJsonContext.Default.AppSettings);
            WriteSettingsAtomically(path, json);
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            AppLogger.LogException("Failed to save settings.", ex);
            return false;
        }
    }

    private static string GetSettingsPath()
    {
        var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "YellowPoint");
        return Path.Combine(folder, FileName);
    }

    private static AppSettings Sanitize(AppSettings settings)
    {
        settings.Diameter = Math.Clamp(settings.Diameter, 10, 200);
        settings.Opacity = Math.Clamp(settings.Opacity, 0.1, 1.0);
        return settings;
    }

    private static void WriteSettingsAtomically(string path, string json)
    {
        var directory = Path.GetDirectoryName(path)
                        ?? throw new InvalidOperationException("Settings directory path is invalid.");
        var tempPath = Path.Combine(directory, $"{FileName}.{Guid.NewGuid():N}.tmp");

        try
        {
            File.WriteAllText(tempPath, json);
            if (File.Exists(path))
            {
                File.Replace(tempPath, path, null);
            }
            else
            {
                File.Move(tempPath, path);
            }
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }
}
