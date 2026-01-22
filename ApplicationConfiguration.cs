using System.Runtime.Versioning;

namespace YellowPoint;

[SupportedOSPlatform("windows")]
internal static class ApplicationConfiguration
{
    public static void Initialize()
    {
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
    }
}
