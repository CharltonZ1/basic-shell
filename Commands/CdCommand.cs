using BasicShell.Interfaces;

namespace BasicShell.Commands;

public class CdCommand : ICommand
{
    public string Name => "cd";

    public Task ExecuteAsync(string[] args, TextWriter output, TextWriter errorOutput)
    {
        if (args.Length != 1)
        {
            errorOutput.WriteLine("Usage: cd <directory>");
            return Task.CompletedTask;
        }

        try
        {
            // Check if the target is the HOME directory
            if (args[0].Contains('~'))
            {
                // Replace ~ with the user's home directory
                string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string homePath = args[0].Replace("~", homeDirectory);

                if (!Directory.Exists(homePath))
                {
                    errorOutput.WriteLine($"cd: {homePath}: No such file or directory");
                    return Task.CompletedTask;
                }

                Environment.CurrentDirectory = homePath;
                return Task.CompletedTask;
            }

            // Else handle absolute or relative paths
            string targetDirectory = args[0];
            if (!Directory.Exists(targetDirectory))
            {
                errorOutput.WriteLine($"cd: {targetDirectory}: No such file or directory");
                return Task.CompletedTask;
            }

            Environment.CurrentDirectory = targetDirectory;
        } 
        catch (Exception ex)
        {
            errorOutput.WriteLine($"Error changing directory: {ex.Message}");
        }

        return Task.CompletedTask;
    }
}