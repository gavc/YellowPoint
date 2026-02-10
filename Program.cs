using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace YellowPoint;

[SupportedOSPlatform("windows")]
static class Program
{
    private const string SingleInstanceMutexName = @"Local\YellowPoint.SingleInstance";

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        using var singleInstanceMutex = new Mutex(true, SingleInstanceMutexName, out var createdNew);
        if (!createdNew)
        {
            MessageBox.Show(
                "YellowPoint is already running.",
                "YellowPoint",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        ConfigureGlobalExceptionHandling();

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        AppLogger.LogInfo("Application started.");
        Application.Run(new MainForm());
        AppLogger.LogInfo("Application exited.");
    }

    private static void ConfigureGlobalExceptionHandling()
    {
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

        Application.ThreadException += (_, args) =>
        {
            AppLogger.LogException("Unhandled UI thread exception.", args.Exception);
            MessageBox.Show(
                "YellowPoint hit an unexpected error. Logs are in %AppData%\\YellowPoint\\logs.",
                "YellowPoint",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        };

        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            var exception = args.ExceptionObject as Exception
                            ?? new Exception($"Unhandled exception object: {args.ExceptionObject}");
            AppLogger.LogException("Unhandled non-UI exception.", exception);
        };

        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            AppLogger.LogException("Unobserved task exception.", args.Exception);
            args.SetObserved();
        };
    }
}
