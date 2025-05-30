using System.Diagnostics;
using System.IO;

namespace BasicShell.Core;

public static class ProcessRunner
{
    public static async Task<(string Output, string Error, int ExitCode)> RunProgramAsync(string path, string[] arguments)
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = path,
                    Arguments = string.Join(" ", arguments),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            return (output.Trim(), error.Trim(), process.ExitCode);
        }
        catch (Exception ex)
        {
            // Handle exceptions such as file not found or access denied
            return (string.Empty, ex.Message, -1);
        }
    }

    public static string? GetPath(string command)
    {
        if (File.Exists(command))
        {
            return Path.GetFullPath(command);
        }

        string fileName = Path.HasExtension(command) || !OperatingSystem.IsWindows()
            ? command
            : $"{command}.exe";

        string? path = Environment.GetEnvironmentVariable("PATH");
        if (path != null)
        {
            // TODO: Check if the file is executable
            string[] paths = path.Split(Path.PathSeparator);
            foreach (string p in paths)
            {
                string fullPath = Path.Combine(p, fileName);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }
        }

        return null;
    }
}