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
        catch (IOException)
        {
            return new AppSettings();
        }
        catch (JsonException)
        {
            return new AppSettings();
        }
    }

    public void Save(AppSettings settings)
    {
        var path = GetSettingsPath();
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
        var json = JsonSerializer.Serialize(settings, AppSettingsJsonContext.Default.AppSettings);
        File.WriteAllText(path, json);
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
}
