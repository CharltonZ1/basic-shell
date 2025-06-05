using System.Diagnostics;
using System.IO;

namespace BasicShell.Core;

public static class ProcessRunner
{
    public static async Task<(string Output, string Error, int ExitCode)> RunProgramAsync(string path, string[] arguments)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = path,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            foreach (var arg in arguments)
            {
                startInfo.ArgumentList.Add(arg);
            }

            var process = new Process { StartInfo = startInfo };

            process.Start();
            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            return (output.Trim(), error.Trim(), process.ExitCode);
        }
        catch (Exception ex)
        {
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