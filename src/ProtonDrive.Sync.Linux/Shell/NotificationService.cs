using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ProtonDrive.Sync.Linux.Shell;

/// <summary>
/// Linux shell integration for desktop notifications using D-Bus
/// </summary>
public class NotificationService
{
    /// <summary>
    /// Show a desktop notification using notify-send or D-Bus
    /// </summary>
    public static async Task ShowNotificationAsync(string title, string message, NotificationPriority priority = NotificationPriority.Normal)
    {
        try
        {
            // Use notify-send command line tool (available on most Linux distros)
            var urgency = priority switch
            {
                NotificationPriority.Low => "low",
                NotificationPriority.Critical => "critical",
                _ => "normal"
            };

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "notify-send",
                    Arguments = $"--urgency={urgency} \"{title}\" \"{message}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            await process.WaitForExitAsync();
        }
        catch (Exception ex)
        {
            // Fallback: log to console if notification fails
            Console.WriteLine($"[{priority}] {title}: {message}");
            Console.WriteLine($"Notification error: {ex.Message}");
        }
    }
}

public enum NotificationPriority
{
    Low,
    Normal,
    Critical
}
