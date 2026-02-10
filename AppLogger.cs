using System.Text;
using System.Threading;

namespace YellowPoint;

internal static class AppLogger
{
    private const int LogRetentionDays = 30;
    private static readonly object Sync = new();
    private static readonly string LogDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "YellowPoint",
        "logs");
    private static int CleanupComplete;

    public static void LogInfo(string message) => Write("INFO", message, null);

    public static void LogWarning(string message) => Write("WARN", message, null);

    public static void LogException(string context, Exception? exception)
    {
        if (exception is null)
        {
            Write("ERROR", context, null);
            return;
        }

        Write("ERROR", context, exception);
    }

    private static void Write(string level, string message, Exception? exception)
    {
        try
        {
            Directory.CreateDirectory(LogDirectory);
            CleanupExpiredLogs();
            var logPath = Path.Combine(LogDirectory, $"yellowpoint-{DateTime.UtcNow:yyyyMMdd}.log");

            var builder = new StringBuilder();
            builder.Append(DateTimeOffset.UtcNow.ToString("O"));
            builder.Append(" [");
            builder.Append(level);
            builder.Append("] ");
            builder.AppendLine(message);

            if (exception is not null)
            {
                builder.AppendLine(exception.ToString());
            }

            lock (Sync)
            {
                File.AppendAllText(logPath, builder.ToString());
            }
        }
        catch
        {
            // Never throw from logging paths.
        }
    }

    private static void CleanupExpiredLogs()
    {
        if (Interlocked.Exchange(ref CleanupComplete, 1) == 1)
        {
            return;
        }

        var cutoffUtc = DateTime.UtcNow.AddDays(-LogRetentionDays);
        foreach (var path in Directory.EnumerateFiles(LogDirectory, "yellowpoint-*.log"))
        {
            try
            {
                if (File.GetLastWriteTimeUtc(path) < cutoffUtc)
                {
                    File.Delete(path);
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // Ignore individual cleanup failures and continue.
            }
        }
    }
}
